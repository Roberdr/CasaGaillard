namespace CasaGaillard.Core.Models;

public sealed class CalendarioMantenimientoViewModel
{
    public DateTime Mes { get; set; }
    public IReadOnlyList<CalendarioDiaViewModel> Dias { get; set; } = [];
}

public sealed class CalendarioDiaViewModel
{
    public DateTime Fecha { get; set; }
    public IReadOnlyList<TareaMantenimientoResumenViewModel> Tareas { get; set; } = [];
}

public sealed class MantenimientoDashboardViewModel
{
    public IReadOnlyList<DashboardRevisionCubaViewModel> RevisionesCubas { get; set; } = [];
    public IReadOnlyList<DashboardRevisionVehiculoViewModel> RevisionesVehiculos { get; set; } = [];
    public IReadOnlyList<TareaMantenimientoResumenViewModel> Tareas { get; set; } = [];
    public int TareasPendientes { get; set; }
    public int TareasVencidas { get; set; }
    public int TareasCompletadasMes { get; set; }
    public decimal CosteMes { get; set; }
    public decimal CosteAnio { get; set; }
}

public sealed class DashboardRevisionCubaViewModel
{
    public int RevisionId { get; set; }
    public int CubaId { get; set; }
    public string MatriculaCuba { get; set; } = "";
    public DateTime? ValidaHasta { get; set; }
    public string? DescripcionProxima { get; set; }
    public DateTime? FechaReservada { get; set; }
}

public sealed class DashboardRevisionVehiculoViewModel
{
    public int RevisionId { get; set; }
    public string MatriculaVehiculo { get; set; } = "";
    public string TipoRevision { get; set; } = "";
    public DateTime? Caducidad { get; set; }
    public DateTime? FechaReservada { get; set; }
}
