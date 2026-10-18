using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class AccesoriosService : IAccesoriosService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;
    public AccesoriosService(AppDbContext db, IWebHostEnvironment environment) { _db = db; _environment = environment; }

    private const string FromSql = @"FROM gaillard.Accesorio a
        INNER JOIN gaillard.TipoAccesorio t ON t.ID=a.TipoAccesorioID
        INNER JOIN gaillard.Material m ON m.ID=a.MaterialID
        LEFT JOIN gaillard.Familia f ON f.FamiliaId=a.FamiliaId
        LEFT JOIN gaillard.Subfamilia s ON s.SubfamiliaId=a.SubfamiliaId
        LEFT JOIN gaillard.Ubicacion u ON u.UbicacionId=a.UbicacionId
        OUTER APPLY (SELECT TOP (1) af.ID FROM dbo.AccesorioFotos af WHERE af.AccesorioID=a.ID ORDER BY af.FechaCreacion DESC,af.ID DESC) foto";
    private const string FilterSql = @" WHERE (@p0 IS NULL OR a.TipoAccesorioID=@p0) AND (@p1 IS NULL OR a.MaterialID=@p1)
        AND (@p2 IS NULL OR a.FamiliaId=@p2) AND (@p3 IS NULL OR a.SubfamiliaId=@p3)
        AND (@p4 IS NULL OR ISNULL(a.Nombre,N'') LIKE N'%'+@p4+N'%' OR ISNULL(a.Descripcion,N'') LIKE N'%'+@p4+N'%')";

    public async Task<AccesoriosIndexViewModel> GetIndexAsync(int? tipoId, int? materialId, int? familiaId, int? subfamiliaId, string? query, int page, int pageSize)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 50);
        var q = string.IsNullOrWhiteSpace(query) ? null : query.Trim();
        object[] args = [Db(tipoId), Db(materialId), Db(familiaId), Db(subfamiliaId), Db(q)];
        var total = await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value " + FromSql + FilterSql, args).SingleAsync();
        var rows = await _db.Database.SqlQueryRaw<AccesorioFilaViewModel>(@"SELECT a.ID AS Id,t.TipoAccesorio AS TipoNombre,a.Nombre,a.Descripcion,
            m.Material AS MaterialNombre,f.Nombre AS FamiliaNombre,s.Nombre AS SubfamiliaNombre,u.Nombre AS UbicacionNombre,a.FechaAlta,a.Activo,foto.ID AS FotoId " +
            FromSql + FilterSql + " ORDER BY a.ID OFFSET @p5 ROWS FETCH NEXT @p6 ROWS ONLY",
            [.. args, (page - 1) * pageSize, pageSize]).ToListAsync();

        var model = new AccesoriosIndexViewModel
        {
            Accesorios = rows, TipoAccesorioId = tipoId, MaterialId = materialId, FamiliaId = familiaId, SubfamiliaId = subfamiliaId,
            Query = q, Page = page, PageSize = pageSize, TotalItems = total,
            TiposAccesorio = await OptionsAsync("SELECT ID AS Id,TipoAccesorio AS Nombre FROM gaillard.TipoAccesorio ORDER BY TipoAccesorio", tipoId),
            Materiales = await OptionsAsync("SELECT ID AS Id,Material AS Nombre FROM gaillard.Material ORDER BY Material", materialId),
            Familias = await OptionsAsync("SELECT FamiliaId AS Id,Nombre FROM gaillard.Familia ORDER BY Nombre", familiaId),
            Subfamilias = await OptionsAsync("SELECT SubfamiliaId AS Id,Nombre FROM gaillard.Subfamilia WHERE (@p0 IS NULL OR FamiliaId=@p0) ORDER BY Nombre", subfamiliaId, Db(familiaId))
        };
        return model;
    }

    public async Task<AccesorioDetailsViewModel?> GetDetailsAsync(int id)
    {
        var accesorio = (await _db.Database.SqlQueryRaw<AccesorioFilaViewModel>(@"SELECT a.ID AS Id,t.TipoAccesorio AS TipoNombre,a.Nombre,a.Descripcion,
            m.Material AS MaterialNombre,f.Nombre AS FamiliaNombre,s.Nombre AS SubfamiliaNombre,u.Nombre AS UbicacionNombre,a.FechaAlta,a.Activo,CAST(NULL AS int) AS FotoId
            FROM gaillard.Accesorio a INNER JOIN gaillard.TipoAccesorio t ON t.ID=a.TipoAccesorioID
            INNER JOIN gaillard.Material m ON m.ID=a.MaterialID LEFT JOIN gaillard.Familia f ON f.FamiliaId=a.FamiliaId
            LEFT JOIN gaillard.Subfamilia s ON s.SubfamiliaId=a.SubfamiliaId LEFT JOIN gaillard.Ubicacion u ON u.UbicacionId=a.UbicacionId
            WHERE a.ID=@p0", id).ToListAsync()).FirstOrDefault();
        if (accesorio is null) return null;
        return new AccesorioDetailsViewModel
        {
            Accesorio = accesorio,
            Detalles = await _db.Database.SqlQueryRaw<DetalleAccesorioFilaViewModel>(@"SELECT d.ID AS Id,c.CaracteristicaAccesorio AS Caracteristica,d.Cantidad,d.Medida,u.Unidad AS Unidad,d.Tipo
                FROM gaillard.DetalleAccesorio d LEFT JOIN gaillard.CaracteristicaAccesorio c ON c.ID=d.CaracteristicaAccesorioID
                LEFT JOIN gaillard.Unidad u ON u.ID=d.UnidadID WHERE d.AccesorioID=@p0 ORDER BY d.ID", id).ToListAsync(),
            Fotos = await _db.Database.SqlQueryRaw<AccesorioFotoViewModel>(@"SELECT ID,NombreOriginal,ContentType,FechaCreacion FROM dbo.AccesorioFotos WHERE AccesorioID=@p0 ORDER BY FechaCreacion DESC,ID DESC", id).ToListAsync()
        };
    }

    public async Task<AccesorioFormViewModel?> GetFormAsync(int? id, int? intervencionId = null)
    {
        AccesorioFormViewModel? model;
        if (id.HasValue)
        {
            var row = (await _db.Database.SqlQueryRaw<AccesorioFormRow>(@"SELECT ID,Nombre,Descripcion,TipoAccesorioID AS TipoAccesorioId,
                MaterialID AS MaterialId,FamiliaId,SubfamiliaId,UbicacionId,FechaAlta,Activo FROM gaillard.Accesorio WHERE ID=@p0", id.Value).ToListAsync()).FirstOrDefault();
            if (row is null) return null;
            model = new AccesorioFormViewModel { Id = row.Id, Nombre = row.Nombre, Descripcion = row.Descripcion, TipoAccesorioId = row.TipoAccesorioId,
                MaterialId = row.MaterialId, FamiliaId = row.FamiliaId, SubfamiliaId = row.SubfamiliaId, UbicacionId = row.UbicacionId, FechaAlta = row.FechaAlta, Activo = row.Activo };
            model.Detalles = await _db.Database.SqlQueryRaw<DetalleAccesorioFormViewModel>(@"SELECT ID,CaracteristicaAccesorioID AS CaracteristicaId,Cantidad,Medida,
                UnidadID AS UnidadId,Tipo,CAST(0 AS bit) AS Eliminar FROM gaillard.DetalleAccesorio WHERE AccesorioID=@p0 ORDER BY ID", id.Value).ToListAsync();
            model.FotosExistentes = await _db.Database.SqlQueryRaw<AccesorioFotoViewModel>(@"SELECT ID AS Id,NombreOriginal,ContentType,FechaCreacion FROM dbo.AccesorioFotos WHERE AccesorioID=@p0 ORDER BY FechaCreacion DESC", id.Value).ToListAsync();
        }
        else model = new AccesorioFormViewModel { FechaAlta = DateTime.Today, IntervencionId = intervencionId };

        if (model.Detalles.Count == 0) model.Detalles.AddRange(Enumerable.Range(0, 4).Select(_ => new DetalleAccesorioFormViewModel()));
        model.TiposAccesorio = await OptionsAsync("SELECT ID AS Id,TipoAccesorio AS Nombre FROM gaillard.TipoAccesorio ORDER BY TipoAccesorio", model.TipoAccesorioId);
        model.Materiales = await OptionsAsync("SELECT ID AS Id,Material AS Nombre FROM gaillard.Material ORDER BY Material", model.MaterialId);
        model.Familias = await OptionsAsync("SELECT FamiliaId AS Id,Nombre FROM gaillard.Familia ORDER BY Nombre", model.FamiliaId);
        model.Subfamilias = await GetSubfamiliasAsync(model.FamiliaId, model.SubfamiliaId);
        model.Ubicaciones = await OptionsAsync("SELECT UbicacionId AS Id,Nombre FROM gaillard.Ubicacion ORDER BY Nombre", model.UbicacionId);
        model.Caracteristicas = await OptionsAsync("SELECT ID AS Id,CaracteristicaAccesorio AS Nombre FROM gaillard.CaracteristicaAccesorio ORDER BY CaracteristicaAccesorio", null);
        model.Unidades = await OptionsAsync("SELECT ID AS Id,Unidad AS Nombre FROM gaillard.Unidad ORDER BY Unidad", null);
        return model;
    }

    public Task<IReadOnlyList<SelectListItem>> GetSubfamiliasAsync(int? familiaId, int? selectedId) =>
        OptionsAsync("SELECT SubfamiliaId AS Id,Nombre FROM gaillard.Subfamilia WHERE (@p0 IS NULL OR FamiliaId=@p0) ORDER BY Nombre", selectedId, Db(familiaId));

    public async Task<int> SaveAsync(AccesorioFormViewModel model, CancellationToken cancellationToken = default)
    {
        List<PreparedPhoto> preparedFiles = await StorePhotosAsync(model.Id, model.FotosAdjuntas, cancellationToken);
        try
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            if (model.Id == 0)
            {
                model.Id = (await _db.Database.SqlQueryRaw<int>(@"INSERT INTO gaillard.Accesorio
                    (TipoAccesorioID,MaterialID,Nombre,Descripcion,FamiliaId,SubfamiliaId,UbicacionId,FechaAlta,Activo)
                    OUTPUT INSERTED.ID AS Value VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)",
                    [model.TipoAccesorioId!.Value, model.MaterialId!.Value, Db(model.Nombre), Db(model.Descripcion), Db(model.FamiliaId), Db(model.SubfamiliaId), Db(model.UbicacionId), Db(model.FechaAlta), model.Activo]).ToListAsync(cancellationToken)).Single();
            }
            else
            {
                var changed = await _db.Database.ExecuteSqlRawAsync(@"UPDATE gaillard.Accesorio SET TipoAccesorioID=@p0,MaterialID=@p1,Nombre=@p2,Descripcion=@p3,
                    FamiliaId=@p4,SubfamiliaId=@p5,UbicacionId=@p6,FechaAlta=@p7,Activo=@p8 WHERE ID=@p9",
                    [model.TipoAccesorioId!.Value, model.MaterialId!.Value, Db(model.Nombre), Db(model.Descripcion), Db(model.FamiliaId), Db(model.SubfamiliaId), Db(model.UbicacionId), Db(model.FechaAlta), model.Activo, model.Id], cancellationToken);
                if (changed == 0)
                {
                    await tx.RollbackAsync(cancellationToken);
                    foreach (var file in preparedFiles) if (File.Exists(file.PhysicalPath)) File.Delete(file.PhysicalPath);
                    return 0;
                }
            }
            if (preparedFiles.Count > 0) preparedFiles = MovePhotosToAccessoryFolder(preparedFiles, model.Id);
            await SaveDetailsAsync(model.Id, model.Detalles, cancellationToken);
            foreach (var file in preparedFiles)
                await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO dbo.AccesorioFotos (AccesorioID,RutaArchivo,NombreOriginal,ContentType,FechaCreacion)
                    VALUES (@p0,@p1,@p2,@p3,@p4)", [model.Id, file.StoredPath, file.Name, file.ContentType, DateTime.Now], cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return model.Id;
        }
        catch
        {
            foreach (var file in preparedFiles) if (File.Exists(file.PhysicalPath)) File.Delete(file.PhysicalPath);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var files = await _db.Database.SqlQueryRaw<FotoArchivo>("SELECT RutaArchivo,NULL AS ContentType FROM dbo.AccesorioFotos WHERE AccesorioID=@p0", id).ToListAsync();
        await using var tx = await _db.Database.BeginTransactionAsync();
        var exists = await _db.Database.SqlQueryRaw<int>("SELECT COUNT(1) AS Value FROM gaillard.Accesorio WHERE ID=@p0", id).SingleAsync() > 0;
        if (!exists) return false;
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM gaillard.DetalleAccesorio WHERE AccesorioID=@p0", id);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM gaillard.AccesorioGrupo WHERE AccesorioID=@p0", id);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.AccesorioFotos WHERE AccesorioID=@p0", id);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM gaillard.Accesorio WHERE ID=@p0", id);
        await tx.CommitAsync();
        foreach (var file in files)
        {
            var path = ResolveFotoPath(file.RutaArchivo);
            if (path is not null && File.Exists(path)) File.Delete(path);
        }
        return true;
    }

    private async Task SaveDetailsAsync(int accesorioId, IReadOnlyList<DetalleAccesorioFormViewModel> details, CancellationToken cancellationToken)
    {
        var current = await _db.Database.SqlQueryRaw<int>("SELECT ID AS Value FROM gaillard.DetalleAccesorio WHERE AccesorioID=@p0", accesorioId).ToListAsync(cancellationToken);
        foreach (var detail in details)
        {
            var empty = !detail.CaracteristicaId.HasValue && !detail.Cantidad.HasValue && !detail.Medida.HasValue && !detail.UnidadId.HasValue && string.IsNullOrWhiteSpace(detail.Tipo);
            if (detail.Id > 0)
            {
                if (!current.Contains(detail.Id)) continue;
                if (detail.Eliminar)
                    await _db.Database.ExecuteSqlRawAsync("DELETE FROM gaillard.DetalleAccesorio WHERE ID=@p0 AND AccesorioID=@p1", [detail.Id, accesorioId], cancellationToken);
                else if (!empty)
                    await _db.Database.ExecuteSqlRawAsync(@"UPDATE gaillard.DetalleAccesorio SET CaracteristicaAccesorioID=@p0,Cantidad=@p1,Medida=@p2,UnidadID=@p3,Tipo=@p4 WHERE ID=@p5 AND AccesorioID=@p6",
                        [Db(detail.CaracteristicaId), Db(detail.Cantidad), Db(detail.Medida), Db(detail.UnidadId), Db(detail.Tipo), detail.Id, accesorioId], cancellationToken);
            }
            else if (!empty)
                await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO gaillard.DetalleAccesorio (AccesorioID,CaracteristicaAccesorioID,Cantidad,Medida,UnidadID,Tipo)
                    VALUES (@p0,@p1,@p2,@p3,@p4,@p5)", [accesorioId, Db(detail.CaracteristicaId), Db(detail.Cantidad), Db(detail.Medida), Db(detail.UnidadId), Db(detail.Tipo)], cancellationToken);
        }
    }

    private async Task<List<PreparedPhoto>> StorePhotosAsync(int accesorioId, IReadOnlyList<Microsoft.AspNetCore.Http.IFormFile> files, CancellationToken cancellationToken)
    {
        var prepared = new List<PreparedPhoto>();
        if (files.Count == 0) return prepared;
        var folder = Path.Combine(_environment.ContentRootPath, "App_Data", "AccesoriosFotos", accesorioId > 0 ? accesorioId.ToString() : "pending-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            foreach (var file in files.Where(f => f.Length > 0))
            {
                var extension = Path.GetExtension(Path.GetFileName(file.FileName));
                var name = Guid.NewGuid().ToString("N") + extension.ToLowerInvariant();
                var path = Path.Combine(folder, name);
                await using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None)) await file.CopyToAsync(stream, cancellationToken);
                var relative = $"~/App_Data/AccesoriosFotos/{(accesorioId > 0 ? accesorioId.ToString() : Path.GetFileName(folder))}/{name}";
                prepared.Add(new PreparedPhoto(path, relative, Path.GetFileName(file.FileName), file.ContentType));
            }
            return prepared;
        }
        catch { foreach (var file in prepared) if (File.Exists(file.PhysicalPath)) File.Delete(file.PhysicalPath); throw; }
    }

    private static object Db(object? value) => value switch { null => DBNull.Value, string text when string.IsNullOrWhiteSpace(text) => DBNull.Value, string text => text.Trim(), _ => value };
    private sealed record PreparedPhoto(string PhysicalPath, string StoredPath, string Name, string ContentType);

    private List<PreparedPhoto> MovePhotosToAccessoryFolder(List<PreparedPhoto> files, int accesorioId)
    {
        var folder = Path.Combine(_environment.ContentRootPath, "App_Data", "AccesoriosFotos", accesorioId.ToString());
        Directory.CreateDirectory(folder);
        var moved = new List<PreparedPhoto>();
        foreach (var file in files)
        {
            var name = Path.GetFileName(file.PhysicalPath);
            var target = Path.Combine(folder, name);
            File.Move(file.PhysicalPath, target);
            moved.Add(file with { PhysicalPath = target, StoredPath = $"~/App_Data/AccesoriosFotos/{accesorioId}/{name}" });
        }
        return moved;
    }

    public async Task<(string RutaArchivo, string? ContentType)?> GetFotoAsync(int id)
    {
        var rows = await _db.Database.SqlQueryRaw<FotoArchivo>("SELECT RutaArchivo,ContentType FROM dbo.AccesorioFotos WHERE ID=@p0", id).ToListAsync();
        return rows.Count == 0 ? null : (rows[0].RutaArchivo, rows[0].ContentType);
    }

    public string? ResolveFotoPath(string rutaArchivo)
    {
        const string prefix = "~/App_Data/AccesoriosFotos/";
        if (string.IsNullOrWhiteSpace(rutaArchivo) || !rutaArchivo.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;
        var root = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "AccesoriosFotos"));
        var path = Path.GetFullPath(Path.Combine(root, rutaArchivo[prefix.Length..].Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar)));
        return path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? path : null;
    }

    private async Task<IReadOnlyList<SelectListItem>> OptionsAsync(string sql, int? selected, params object[] args)
    {
        var options = await _db.Database.SqlQueryRaw<Option>(sql, args).ToListAsync();
        return options.Select(x => new SelectListItem(x.Nombre, x.Id.ToString(), x.Id == selected)).ToList();
    }
    private sealed class Option { public int Id { get; set; } public string Nombre { get; set; } = string.Empty; }
    private sealed class FotoArchivo { public string RutaArchivo { get; set; } = string.Empty; public string? ContentType { get; set; } }
    private sealed class AccesorioFormRow
    {
        public int Id { get; set; } public string? Nombre { get; set; } public string? Descripcion { get; set; }
        public int? TipoAccesorioId { get; set; } public int? MaterialId { get; set; } public int? FamiliaId { get; set; }
        public int? SubfamiliaId { get; set; } public int? UbicacionId { get; set; } public DateTime? FechaAlta { get; set; } public bool Activo { get; set; }
    }
}
