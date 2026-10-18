using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;

namespace CasaGaillard.Core.Services;

public sealed class CubasService : ICubasService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;
    private static readonly FileExtensionContentTypeProvider ContentTypes = new();
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    public CubasService(AppDbContext db, IWebHostEnvironment environment) { _db = db; _environment = environment; }

    public async Task<IReadOnlyList<CubaFilaViewModel>> GetAllAsync(bool incluirBajas) => await _db.Database.SqlQueryRaw<CubaFilaViewModel>(@"SELECT c.ID Id,c.MatriculaCuba Matricula,c.NumCuadro,c.Codigo,
        v.MatriculaVehiculo Vehiculo,m.Material1 Material,c.PesoBruto,c.Tara,COALESCE(NULLIF(c.PesoMaxProducto,0),c.PesoBruto-c.Tara) PesoMaxProducto,
        c.UpdatedAt,CAST(COALESCE(c.Baja,0) AS bit) Baja FROM gaillard.Cuba c
        LEFT JOIN gaillard.Vehiculo v ON v.ID=c.PlataformaID LEFT JOIN gaillard.Material m ON m.ID=c.MaterialExteriorID
        WHERE @p0=1 OR COALESCE(c.Baja,0)=0 ORDER BY c.MatriculaCuba", incluirBajas ? 1 : 0).ToListAsync();

    public async Task<CubaDetalleViewModel?> GetByIdAsync(int id)
    {
        var rows = await _db.Database.SqlQueryRaw<CubaDetalleViewModel>(@"SELECT c.ID Id,c.MatriculaCuba Matricula,c.NumCuadro,c.Codigo,v.MatriculaVehiculo Vehiculo,m.Material1 Material,
            c.PesoBruto,c.Tara,COALESCE(NULLIF(c.PesoMaxProducto,0),c.PesoBruto-c.Tara) PesoMaxProducto,c.UpdatedAt,CAST(COALESCE(c.Baja,0) AS bit) Baja,
            c.Constructor,c.NumFabricacion,c.NumHomologacion,c.FechaConstruccion,c.PaisFabricacion,c.NumTipoIMO,c.PaisAprobacion,c.Autoridad,c.CodigoDiseno,c.PruebaHidraulica,
            c.PresionServicioADR,c.PresionServicioIMO,c.PresionExterior,c.PresionTaradoValvulas,c.TemperaturaCalculoReferencia,c.EspesorCuerpo,c.EspesorFondo,c.EspesorEquivalente,
            c.TipoForro,c.NumAprobacionCSC,c.Modelo,c.PesoMaxApilamiento,c.CargaRigidez,c.PresionPrueba,c.TemperaturaMinCarga,c.Longitud,c.Ancho,c.Alto,
            c.NumAprobacionIMDG,c.[NumAprobacionADR/RID] NumAprobacionADR_RID,c.UNPortableTank,c.NumAprobacion
            FROM gaillard.Cuba c LEFT JOIN gaillard.Vehiculo v ON v.ID=c.PlataformaID LEFT JOIN gaillard.Material m ON m.ID=c.MaterialExteriorID WHERE c.ID=@p0", id).ToListAsync();
        var detail = rows.FirstOrDefault();
        if (detail is null) return null;
        detail.Compartimentos = await _db.Database.SqlQueryRaw<CubaCompartimentoViewModel>(@"SELECT co.Numero,co.Capacidad,m.Descripcion Material
            FROM gaillard.Compartimento co LEFT JOIN gaillard.Material m ON m.ID=co.MaterialInteriorID
            WHERE co.CubaID=@p0 ORDER BY co.Numero", id).ToListAsync();
        var imageDirectory = GetImageDirectory(detail.Matricula);
        if (Directory.Exists(imageDirectory))
            detail.Fotos = Directory.EnumerateFiles(imageDirectory).Where(path => ImageExtensions.Contains(Path.GetExtension(path)))
                .Select(Path.GetFileName).Where(name => !string.IsNullOrWhiteSpace(name)).OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList()!;
        return detail;
    }

    public async Task<(string Path, string ContentType)?> GetImageAsync(int id, string fileName)
    {
        var rows = await _db.Database.SqlQueryRaw<MatriculaRow>("SELECT MatriculaCuba FROM gaillard.Cuba WHERE ID=@p0", id).ToListAsync();
        if (rows.Count == 0 || string.IsNullOrWhiteSpace(fileName) || Path.GetFileName(fileName) != fileName ||
            !ImageExtensions.Contains(Path.GetExtension(fileName))) return null;
        var root = Path.GetFullPath(GetImageRoot());
        var folder = Path.GetFullPath(Path.Combine(root, rows[0].MatriculaCuba));
        var path = Path.GetFullPath(Path.Combine(folder, fileName));
        if (!folder.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
            !path.StartsWith(folder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !File.Exists(path)) return null;
        ContentTypes.TryGetContentType(path, out var contentType);
        return (path, contentType ?? "application/octet-stream");
    }

    private string GetImageDirectory(string matricula) => Path.Combine(GetImageRoot(), matricula);
    private string GetImageRoot()
    {
        var siblingContent = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "Content", "images"));
        var localContent = Path.Combine(_environment.ContentRootPath, "Content", "images");
        return Directory.Exists(siblingContent) ? siblingContent : localContent;
    }

    public async Task<CubaFormViewModel?> GetFormAsync(int? id)
    {
        var m = new CubaFormViewModel();
        if (id.HasValue)
        {
            var rows = await _db.Database.SqlQueryRaw<CubaFormRow>(@"SELECT c.ID Id,c.MatriculaCuba Matricula,c.NumCuadro,c.Codigo,v.MatriculaVehiculo Vehiculo,m.Material1 Material,
                c.PesoBruto,c.Tara,c.PesoMaxProducto,c.UpdatedAt,CAST(COALESCE(c.Baja,0) AS bit) Baja,c.MaterialExteriorID MaterialId,c.PlataformaID PlataformaId,
                c.Constructor,c.NumFabricacion,c.NumHomologacion,c.FechaConstruccion,c.PaisFabricacion,c.NumTipoIMO,c.PaisAprobacion,c.Autoridad,c.CodigoDiseno,c.PruebaHidraulica,
                c.PresionServicioADR,c.PresionServicioIMO,c.PresionExterior,c.PresionTaradoValvulas,c.TemperaturaCalculoReferencia,c.EspesorCuerpo,c.EspesorFondo,c.EspesorEquivalente,
                c.TipoForro,c.NumAprobacionCSC,c.Modelo,c.PesoMaxApilamiento,c.CargaRigidez,c.PresionPrueba,c.TemperaturaMinCarga,c.Longitud,c.Ancho,c.Alto,
                c.NumAprobacionIMDG,c.[NumAprobacionADR/RID] NumAprobacionADR_RID,c.UNPortableTank,c.NumAprobacion FROM gaillard.Cuba c
                LEFT JOIN gaillard.Vehiculo v ON v.ID=c.PlataformaID LEFT JOIN gaillard.Material m ON m.ID=c.MaterialExteriorID WHERE c.ID=@p0", id.Value).ToListAsync();
            if (rows.Count == 0) return null;
            CopyProperties(rows[0], m);
            m.MaterialId = rows[0].MaterialId;
            m.PlataformaId = rows[0].PlataformaId;
        }
        var materials = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID,Material1 Texto FROM gaillard.Material ORDER BY Material1").ToListAsync();
        var vehicles = await _db.Database.SqlQueryRaw<OptionRow>("SELECT ID,MatriculaVehiculo Texto FROM gaillard.Vehiculo WHERE (Baja IS NULL OR Baja=0) OR ID=@p0 ORDER BY MatriculaVehiculo", (object?)m.PlataformaId ?? DBNull.Value).ToListAsync();
        m.Materiales = materials.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.Texto ?? "", x.Id.ToString(), x.Id == m.MaterialId)).ToList();
        m.Vehiculos = vehicles.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(x.Texto ?? "", x.Id.ToString(), x.Id == m.PlataformaId)).ToList();
        return m;
    }

    public async Task<bool> SaveAsync(CubaFormViewModel m, bool create)
    {
        var valid = await _db.Database.SqlQueryRaw<int>(@"SELECT COUNT(*) AS Value FROM gaillard.Cuba WHERE MatriculaCuba=@p0 AND ID<>@p1", m.Matricula.Trim(), create ? 0 : m.Id).SingleAsync();
        if (valid > 0) return false;
        if (m.MaterialId.HasValue && await _db.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM gaillard.Material WHERE ID=@p0", m.MaterialId.Value).SingleAsync() == 0) return false;
        if (m.PlataformaId.HasValue && await _db.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM gaillard.Vehiculo WHERE ID=@p0", m.PlataformaId.Value).SingleAsync() == 0) return false;
        var maxWeight = m.PesoMaxProducto ?? (m.PesoBruto.HasValue && m.Tara.HasValue ? m.PesoBruto - m.Tara : null);
        if (create)
            await _db.Database.ExecuteSqlInterpolatedAsync($@"INSERT INTO gaillard.Cuba(MatriculaCuba,NumCuadro,Codigo,Constructor,NumFabricacion,NumHomologacion,FechaConstruccion,PaisFabricacion,NumTipoIMO,PaisAprobacion,Autoridad,CodigoDiseno,PruebaHidraulica,PresionServicioADR,PresionServicioIMO,PresionExterior,PresionTaradoValvulas,TemperaturaCalculoReferencia,PesoBruto,Tara,PesoMaxProducto,MaterialExteriorID,EspesorCuerpo,EspesorFondo,EspesorEquivalente,TipoForro,NumAprobacionCSC,Modelo,PesoMaxApilamiento,CargaRigidez,PresionPrueba,TemperaturaMinCarga,PlataformaID,Longitud,Ancho,Alto,UpdatedAt,CreatedAt,UpdatedBy,CreatedBy,NumAprobacionIMDG,[NumAprobacionADR/RID],UNPortableTank,NumAprobacion,Baja)
                VALUES({m.Matricula.Trim()},{m.NumCuadro},{m.Codigo},{m.Constructor},{m.NumFabricacion},{m.NumHomologacion},{m.FechaConstruccion?.Date},{m.PaisFabricacion},{m.NumTipoIMO},{m.PaisAprobacion},{m.Autoridad},{m.CodigoDiseno},{m.PruebaHidraulica?.Date},{m.PresionServicioADR},{m.PresionServicioIMO},{m.PresionExterior},{m.PresionTaradoValvulas},{m.TemperaturaCalculoReferencia},{m.PesoBruto},{m.Tara},{maxWeight},{m.MaterialId},{m.EspesorCuerpo},{m.EspesorFondo},{m.EspesorEquivalente},{m.TipoForro},{m.NumAprobacionCSC},{m.Modelo},{m.PesoMaxApilamiento},{m.CargaRigidez},{m.PresionPrueba},{m.TemperaturaMinCarga},{m.PlataformaId},{m.Longitud},{m.Ancho},{m.Alto},GETDATE(),GETDATE(),NULL,NULL,{m.NumAprobacionIMDG},{m.NumAprobacionADR_RID},{m.UNPortableTank},{m.NumAprobacion},{m.Baja})");
        else
            await _db.Database.ExecuteSqlInterpolatedAsync($@"UPDATE gaillard.Cuba SET MatriculaCuba={m.Matricula.Trim()},NumCuadro={m.NumCuadro},Codigo={m.Codigo},Constructor={m.Constructor},NumFabricacion={m.NumFabricacion},NumHomologacion={m.NumHomologacion},FechaConstruccion={m.FechaConstruccion?.Date},PaisFabricacion={m.PaisFabricacion},NumTipoIMO={m.NumTipoIMO},PaisAprobacion={m.PaisAprobacion},Autoridad={m.Autoridad},CodigoDiseno={m.CodigoDiseno},PruebaHidraulica={m.PruebaHidraulica?.Date},PresionServicioADR={m.PresionServicioADR},PresionServicioIMO={m.PresionServicioIMO},PresionExterior={m.PresionExterior},PresionTaradoValvulas={m.PresionTaradoValvulas},TemperaturaCalculoReferencia={m.TemperaturaCalculoReferencia},PesoBruto={m.PesoBruto},Tara={m.Tara},PesoMaxProducto={maxWeight},MaterialExteriorID={m.MaterialId},EspesorCuerpo={m.EspesorCuerpo},EspesorFondo={m.EspesorFondo},EspesorEquivalente={m.EspesorEquivalente},TipoForro={m.TipoForro},NumAprobacionCSC={m.NumAprobacionCSC},Modelo={m.Modelo},PesoMaxApilamiento={m.PesoMaxApilamiento},CargaRigidez={m.CargaRigidez},PresionPrueba={m.PresionPrueba},TemperaturaMinCarga={m.TemperaturaMinCarga},PlataformaID={m.PlataformaId},Longitud={m.Longitud},Ancho={m.Ancho},Alto={m.Alto},UpdatedAt=GETDATE(),NumAprobacionIMDG={m.NumAprobacionIMDG},[NumAprobacionADR/RID]={m.NumAprobacionADR_RID},UNPortableTank={m.UNPortableTank},NumAprobacion={m.NumAprobacion},Baja={m.Baja} WHERE ID={m.Id}");
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dependencies = await _db.Database.SqlQueryRaw<int>(@"SELECT (SELECT COUNT(*) FROM gaillard.Revision WHERE CubaID=@p0)+
            (SELECT COUNT(*) FROM gaillard.Compartimento WHERE CubaID=@p0)+ (SELECT COUNT(*) FROM gaillard.Grupo WHERE CubaID=@p0)+
            (SELECT COUNT(*) FROM gaillard.Operacion WHERE CubaID=@p0) AS Value", id).SingleAsync();
        if (dependencies != 0) return false;
        return await _db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM gaillard.Cuba WHERE ID={id}") > 0;
    }

    private sealed class OptionRow { public int Id { get; set; } public string? Texto { get; set; } }
    private sealed class MatriculaRow { public string MatriculaCuba { get; set; } = ""; }
    private static void CopyProperties(object source, object destination)
    {
        var targetProperties = destination.GetType().GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name);
        foreach (var sourceProperty in source.GetType().GetProperties())
            if (targetProperties.TryGetValue(sourceProperty.Name, out var target) && target.PropertyType == sourceProperty.PropertyType)
                target.SetValue(destination, sourceProperty.GetValue(source));
    }
    private sealed class CubaFormRow : CubaDetalleViewModel { public int? MaterialId { get; set; } public int? PlataformaId { get; set; } }
}
