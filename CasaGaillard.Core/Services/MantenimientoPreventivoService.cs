using System.Data;
using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class MantenimientoPreventivoService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MantenimientoPreventivoService> _logger;

    public MantenimientoPreventivoService(IServiceScopeFactory scopeFactory, ILogger<MantenimientoPreventivoService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var generated = await GenerateUpcomingTasksAsync(stoppingToken);
                if (generated > 0) _logger.LogInformation("Generadas {NumeroTareas} tareas preventivas.", generated);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "No se pudieron generar las tareas preventivas.");
            }

            try { await Task.Delay(TimeSpan.FromHours(24), stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
        }
    }

    private async Task<int> GenerateUpcomingTasksAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var horizon = DateTime.Today.AddDays(30);
        var plans = await db.Database.SqlQueryRaw<PlanGeneracionRow>(@"SELECT p.PlanId,p.Nombre,p.Descripcion,p.PeriodicidadDias,p.ProximaEjecucion,
                p.EquipoId,p.AccesorioId,p.AccesorioGrupoID,e.InstalacionId,
                COALESCE(NULLIF(LTRIM(RTRIM(e.Nombre)),N''),e.Codigo,NULLIF(LTRIM(RTRIM(a.Nombre)),N''),N'Activo de mantenimiento') AS ActivoNombre
            FROM gaillard.PlanMantenimiento p WITH (UPDLOCK,ROWLOCK)
            LEFT JOIN gaillard.Equipo e ON e.Id=p.EquipoId
            LEFT JOIN gaillard.Accesorio a ON a.ID=p.AccesorioId
            WHERE p.Activo=1 AND p.ProximaEjecucion IS NOT NULL AND p.ProximaEjecucion<=@p0
                AND COALESCE(p.FrecuenciaUnidad,N'')<>N'Horas de uso'", horizon)
            .ToListAsync(cancellationToken);

        var generated = 0;
        foreach (var plan in plans)
        {
            if (plan.PeriodicidadDias <= 0) continue;
            var next = plan.ProximaEjecucion!.Value.Date;
            var today = DateTime.Today;
            while (next < today) next = next.AddDays(plan.PeriodicidadDias);

            while (next <= horizon)
            {
                var exists = await db.TareasMantenimiento.AnyAsync(task =>
                    task.PlanMantenimientoId == plan.PlanId && task.FechaProgramada == next, cancellationToken);
                if (!exists)
                {
                    var now = DateTime.Now;
                    var title = $"Mantenimiento preventivo: {plan.Nombre}";
                    var task = new TareaMantenimiento
                    {
                        Codigo = $"PM-{plan.PlanId}-{next:yyyyMMdd}",
                        Titulo = title.Length <= 150 ? title : title[..150],
                        Descripcion = string.IsNullOrWhiteSpace(plan.Descripcion) ? plan.Nombre : plan.Descripcion,
                        Equipo = plan.ActivoNombre,
                        InstalacionId = plan.InstalacionId,
                        EquipoId = plan.EquipoId,
                        PlanMantenimientoId = plan.PlanId,
                        AccesoriosNecesarios = plan.AccesorioId?.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        Prioridad = "Media",
                        Estado = "Nueva",
                        FechaDeteccion = now,
                        FechaProgramada = next,
                        FechaCreacion = now,
                        CreadoPor = "Sistema preventivo"
                    };
                    db.TareasMantenimiento.Add(task);
                    await db.SaveChangesAsync(cancellationToken);
                    db.TareaMantenimientoHistorial.Add(new TareaMantenimientoHistorial
                    {
                        TareaMantenimientoID = task.ID,
                        Usuario = "Sistema preventivo",
                        Accion = "Creada",
                        Fecha = DateTime.UtcNow,
                        CambiosJson = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            Codigo = task.Codigo,
                            Titulo = task.Titulo,
                            Estado = task.Estado,
                            FechaProgramada = task.FechaProgramada,
                            PlanMantenimientoId = task.PlanMantenimientoId
                        })
                    });
                    await db.SaveChangesAsync(cancellationToken);
                    generated++;
                }
                next = next.AddDays(plan.PeriodicidadDias);
            }

            await db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE gaillard.PlanMantenimiento SET ProximaEjecucion={next} WHERE PlanId={plan.PlanId}", cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return generated;
    }

    private sealed class PlanGeneracionRow
    {
        public int PlanId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int PeriodicidadDias { get; set; }
        public DateTime? ProximaEjecucion { get; set; }
        public int? EquipoId { get; set; }
        public int? AccesorioId { get; set; }
        public int? AccesorioGrupoID { get; set; }
        public int? InstalacionId { get; set; }
        public string? ActivoNombre { get; set; }
    }
}
