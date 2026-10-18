using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class EquiposService : IEquiposService
{
    private static readonly string[] ElementTypes = ["SUBSISTEMA", "EQUIPO", "COMPONENTE", "INSTALACION_AUXILIAR"];
    private readonly AppDbContext _db;
    public EquiposService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<EquipoFilaViewModel>> GetAllAsync(int? instalacionId, bool incluirBajas) =>
        await _db.Database.SqlQuery<EquipoFilaViewModel>($@"
            WITH Arbol AS (
                SELECT e.Id,e.InstalacionId,e.EquipoPadreId,e.Codigo,e.Nombre,e.TipoElemento,e.Fabricante,e.Modelo,e.NumeroSerie,e.Activo,0 AS Nivel,
                    CAST(RIGHT(REPLICATE('0',10)+CONVERT(varchar(10),e.Id),10) AS varchar(max)) AS Orden
                FROM gaillard.Equipo e WHERE e.EquipoPadreId IS NULL
                UNION ALL
                SELECT e.Id,e.InstalacionId,e.EquipoPadreId,e.Codigo,e.Nombre,e.TipoElemento,e.Fabricante,e.Modelo,e.NumeroSerie,e.Activo,a.Nivel+1,
                    CAST(a.Orden+'/'+RIGHT(REPLICATE('0',10)+CONVERT(varchar(10),e.Id),10) AS varchar(max))
                FROM gaillard.Equipo e JOIN Arbol a ON a.Id=e.EquipoPadreId
            )
            SELECT a.Id,a.InstalacionId,i.Nombre AS InstalacionNombre,a.Nivel,a.TipoElemento,p.Nombre AS PadreNombre,
                a.Codigo,a.Nombre,a.Fabricante,a.Modelo,a.NumeroSerie,a.Activo
            FROM Arbol a JOIN gaillard.Instalacion i ON i.Id=a.InstalacionId
            LEFT JOIN gaillard.Equipo p ON p.Id=a.EquipoPadreId
            WHERE ({instalacionId} IS NULL OR a.InstalacionId={instalacionId}) AND ({incluirBajas}=1 OR a.Activo=1)
            ORDER BY i.Nombre,a.Orden OPTION (MAXRECURSION 100)").ToListAsync();

    public async Task<IReadOnlyList<SelectListItem>> GetInstalacionesAsync(int? selectedId) =>
        await GetOptionsAsync("SELECT Id,CONCAT(Codigo,N' - ',Nombre) AS Texto FROM gaillard.Instalacion WHERE Estado<>N'FUERA_SERVICIO' ORDER BY Nombre", selectedId);

    public async Task<EquipoFormViewModel?> GetFormAsync(int? id, int? instalacionId = null)
    {
        var model = new EquipoFormViewModel { InstalacionId = instalacionId ?? 0 };
        if (id.HasValue)
        {
            var rows = await _db.Database.SqlQuery<FormData>($@"SELECT Id,InstalacionId,EquipoPadreId,TipoElemento,Codigo,Nombre,Descripcion,
                Fabricante,Modelo,NumeroSerie,Potencia,Caudal,Presion,Voltaje,FechaInstalacion,FechaBaja,Activo,Observaciones
                FROM gaillard.Equipo WHERE Id={id.Value}").ToListAsync();
            var row = rows.SingleOrDefault(); if (row is null) return null;
            model = new EquipoFormViewModel { Id=row.Id,InstalacionId=row.InstalacionId,EquipoPadreId=row.EquipoPadreId,TipoElemento=row.TipoElemento,
                Codigo=row.Codigo,Nombre=row.Nombre,Descripcion=row.Descripcion,Fabricante=row.Fabricante,Modelo=row.Modelo,NumeroSerie=row.NumeroSerie,
                Potencia=row.Potencia,Caudal=row.Caudal,Presion=row.Presion,Voltaje=row.Voltaje,FechaInstalacion=row.FechaInstalacion,
                FechaBaja=row.FechaBaja,Activo=row.Activo,Observaciones=row.Observaciones };
        }
        return await PrepareFormAsync(model);
    }

    public async Task<EquipoFormViewModel> PrepareFormAsync(EquipoFormViewModel model)
    {
        model.Instalaciones = await GetInstalacionesAsync(model.InstalacionId == 0 ? null : model.InstalacionId);
        model.EquiposPadre = await GetOptionsAsync($"SELECT Id,CONCAT(i.Nombre,N' / ',e.Codigo,N' - ',e.Nombre) AS Texto FROM gaillard.Equipo e JOIN gaillard.Instalacion i ON i.Id=e.InstalacionId WHERE e.Activo=1 AND e.InstalacionId={model.InstalacionId} AND e.Id<>{model.Id} ORDER BY e.Nombre", model.EquipoPadreId);
        model.TiposElemento = ElementTypes.Select(type => new SelectListItem(type.Replace('_',' '),type,type==model.TipoElemento)).ToList();
        return model;
    }

    public async Task<IReadOnlyDictionary<string,string>> ValidateAsync(EquipoFormViewModel model)
    {
        var errors = new Dictionary<string,string>();
        if (!ElementTypes.Contains(model.TipoElemento ?? string.Empty)) errors["TipoElemento"]="Selecciona un tipo de elemento válido.";
        var installation = await _db.Database.SqlQuery<CountRow>($"SELECT COUNT(1) AS Value FROM gaillard.Instalacion WHERE Id={model.InstalacionId} AND Estado<>N'FUERA_SERVICIO'").SingleAsync();
        if (installation.Value==0) errors["InstalacionId"]="Selecciona una instalación disponible.";
        if (model.FechaInstalacion.HasValue && model.FechaBaja.HasValue && model.FechaBaja<model.FechaInstalacion) errors["FechaBaja"]="La fecha de baja no puede ser anterior a la instalación.";
        var code=(model.Codigo??string.Empty).Trim();
        var duplicate=await _db.Database.SqlQuery<CountRow>($"SELECT COUNT(1) AS Value FROM gaillard.Equipo WHERE InstalacionId={model.InstalacionId} AND Codigo={code} AND Id<>{model.Id}").SingleAsync();
        if(duplicate.Value>0) errors["Codigo"]="Ya existe un equipo con ese código en la instalación.";
        if(model.EquipoPadreId.HasValue)
        {
            if(model.EquipoPadreId.Value==model.Id) errors["EquipoPadreId"]="Un equipo no puede ser su propio equipo padre.";
            else { var parent=await _db.Database.SqlQuery<CountRow>($"SELECT COUNT(1) AS Value FROM gaillard.Equipo WHERE Id={model.EquipoPadreId.Value} AND InstalacionId={model.InstalacionId} AND Activo=1").SingleAsync(); if(parent.Value==0) errors["EquipoPadreId"]="El equipo padre debe estar activo y pertenecer a la misma instalación."; }
        }
        if(model.Id>0)
        {
            var current=await _db.Database.SqlQuery<CurrentEquipment>($"SELECT InstalacionId,(SELECT COUNT(1) FROM gaillard.Equipo child WHERE child.EquipoPadreId=e.Id) AS Hijos FROM gaillard.Equipo e WHERE Id={model.Id}").ToListAsync();
            var row=current.SingleOrDefault();
            if(row is null) errors["Id"]="El equipo ya no existe.";
            else if(row.Hijos>0 && row.InstalacionId!=model.InstalacionId) errors["InstalacionId"]="No se puede cambiar la instalación mientras el equipo tenga subsistemas asociados.";
            if(model.EquipoPadreId.HasValue)
            {
                var cycle=await _db.Database.SqlQuery<CountRow>($@"WITH Antecesores AS (SELECT Id,EquipoPadreId FROM gaillard.Equipo WHERE Id={model.EquipoPadreId.Value}
                    UNION ALL SELECT e.Id,e.EquipoPadreId FROM gaillard.Equipo e JOIN Antecesores a ON e.Id=a.EquipoPadreId)
                    SELECT COUNT(1) AS Value FROM Antecesores WHERE Id={model.Id} OPTION (MAXRECURSION 100)").SingleAsync();
                if(cycle.Value>0) errors["EquipoPadreId"]="La jerarquía elegida crearía un ciclo.";
            }
        }
        return errors;
    }

    public async Task<bool> SaveAsync(EquipoFormViewModel model)
    {
        var descripcion=Normalize(model.Descripcion);var fabricante=Normalize(model.Fabricante);var modelo=Normalize(model.Modelo);var serie=Normalize(model.NumeroSerie);var observaciones=Normalize(model.Observaciones);
        if(model.Id==0)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($@"INSERT INTO gaillard.Equipo
                (InstalacionId,EquipoPadreId,TipoElemento,Codigo,Nombre,Descripcion,Fabricante,Modelo,NumeroSerie,Potencia,Caudal,Presion,Voltaje,FechaInstalacion,FechaBaja,Activo,Observaciones)
                VALUES ({model.InstalacionId},{model.EquipoPadreId},{model.TipoElemento},{model.Codigo.Trim()},{model.Nombre.Trim()},{descripcion},{fabricante},{modelo},{serie},
                {model.Potencia},{model.Caudal},{model.Presion},{model.Voltaje},{model.FechaInstalacion},{model.FechaBaja},{model.Activo},{observaciones})");return true;
        }
        var affected=await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Equipo SET InstalacionId={model.InstalacionId},EquipoPadreId={model.EquipoPadreId},TipoElemento={model.TipoElemento},
            Codigo={model.Codigo.Trim()},Nombre={model.Nombre.Trim()},Descripcion={descripcion},Fabricante={fabricante},Modelo={modelo},NumeroSerie={serie},Potencia={model.Potencia},
            Caudal={model.Caudal},Presion={model.Presion},Voltaje={model.Voltaje},FechaInstalacion={model.FechaInstalacion},FechaBaja={model.FechaBaja},Activo={model.Activo},
            Observaciones={observaciones},UpdatedAt=SYSDATETIME() WHERE Id={model.Id}");return affected>0;
    }

    public async Task DarDeBajaAsync(int id) => await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Equipo SET Activo=0,
        FechaBaja=CASE WHEN FechaInstalacion IS NULL OR FechaInstalacion<=CONVERT(date,GETDATE()) THEN COALESCE(FechaBaja,CONVERT(date,GETDATE())) ELSE FechaBaja END,
        UpdatedAt=SYSDATETIME() WHERE Id={id}");

    private async Task<IReadOnlyList<SelectListItem>> GetOptionsAsync(string sql,int? selected) => (await _db.Database.SqlQueryRaw<Option>(sql).ToListAsync()).Select(x=>new SelectListItem(x.Texto,x.Id.ToString(),x.Id==selected)).ToList();
    private static string? Normalize(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
    private sealed class Option{public int Id{get;set;}public string Texto{get;set;}=string.Empty;}
    private sealed class CountRow{public int Value{get;set;}}
    private sealed class CurrentEquipment{public int InstalacionId{get;set;}public int Hijos{get;set;}}
    private sealed class FormData{public int Id{get;set;}public int InstalacionId{get;set;}public int? EquipoPadreId{get;set;}public string TipoElemento{get;set;}=string.Empty;public string Codigo{get;set;}=string.Empty;public string Nombre{get;set;}=string.Empty;public string? Descripcion{get;set;}public string? Fabricante{get;set;}public string? Modelo{get;set;}public string? NumeroSerie{get;set;}public decimal? Potencia{get;set;}public decimal? Caudal{get;set;}public decimal? Presion{get;set;}public decimal? Voltaje{get;set;}public DateTime? FechaInstalacion{get;set;}public DateTime? FechaBaja{get;set;}public bool Activo{get;set;}public string? Observaciones{get;set;}}
}
