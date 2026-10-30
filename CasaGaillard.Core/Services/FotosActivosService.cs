using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CasaGaillard.Core.Services;

public sealed class FotosActivosService : IFotosActivosService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;
    public FotosActivosService(AppDbContext db, IWebHostEnvironment environment) { _db = db; _environment = environment; }

    public async Task<InstalacionEquipoFotosViewModel?> GetAsync(string tipo, int id)
    {
        var title = await GetTitleAsync(tipo, id);
        if (title is null) return null;
        if (tipo == "Cuba") await ImportLegacyCubaPhotosAsync(id);
        var photos = await _db.Database.SqlQueryRaw<InstalacionEquipoFotoFilaViewModel>(@"SELECT ID AS Id,NombreOriginal,Observacion,FechaCreacion
            FROM dbo.InstalacionEquipoFotos WHERE TipoActivo=@p0 AND ActivoID=@p1 ORDER BY FechaCreacion DESC,ID DESC", tipo, id).ToListAsync();
        return new InstalacionEquipoFotosViewModel { Tipo = tipo, ActivoId = id, Titulo = title, Fotos = photos };
    }

    public async Task<(string PhysicalPath, string ContentType)?> GetFotoAsync(int id)
    {
        var rows = await _db.Database.SqlQueryRaw<FotoRow>("SELECT RutaArchivo,TipoActivo,ActivoID,ContentType FROM dbo.InstalacionEquipoFotos WHERE ID=@p0", id).ToListAsync();
        if (rows.Count == 0) return null;
        var path = ResolvePath(rows[0].RutaArchivo, rows[0].TipoActivo, rows[0].ActivoID);
        return path is null ? null : (path, rows[0].ContentType ?? "application/octet-stream");
    }

    public async Task<bool> UploadAsync(string tipo, int id, IReadOnlyList<Microsoft.AspNetCore.Http.IFormFile> files, IReadOnlyList<string?> observaciones, CancellationToken cancellationToken = default)
    {
        var folder = Path.Combine(_environment.ContentRootPath, "App_Data", "InstalacionEquipoFotos", tipo, id.ToString());
        Directory.CreateDirectory(folder);
        var pending = new List<PendingPhoto>();
        try
        {
            for (var index = 0; index < files.Count; index++)
            {
                var file = files[index];
                var extension = Path.GetExtension(Path.GetFileName(file.FileName)).ToLowerInvariant();
                var name = Guid.NewGuid().ToString("N") + extension;
                var physicalPath = Path.Combine(folder, name);
                await using (var stream = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    await file.CopyToAsync(stream, cancellationToken);
                pending.Add(new PendingPhoto(physicalPath, $"~/App_Data/InstalacionEquipoFotos/{tipo}/{id}/{name}", Path.GetFileName(file.FileName), file.ContentType, CaptionAt(observaciones, index)));
            }
            foreach (var photo in pending)
                await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO dbo.InstalacionEquipoFotos (TipoActivo,ActivoID,RutaArchivo,NombreOriginal,ContentType,FechaCreacion,Observacion)
                    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)", [tipo, id, photo.RelativePath, photo.Name, photo.ContentType, DateTime.Now, ObservationParameter("@p6", photo.Observacion)], cancellationToken);
            return true;
        }
        catch
        {
            foreach (var photo in pending) if (File.Exists(photo.PhysicalPath)) File.Delete(photo.PhysicalPath);
            throw;
        }
    }

    public async Task<(bool Exists, string Tipo, int ActivoId, string? Ruta)> DeleteAsync(int fotoId)
    {
        var rows = await _db.Database.SqlQueryRaw<FotoRow>("SELECT RutaArchivo,TipoActivo,ActivoID,ContentType FROM dbo.InstalacionEquipoFotos WHERE ID=@p0", fotoId).ToListAsync();
        if (rows.Count == 0) return (false, string.Empty, 0, null);
        var row = rows[0];
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM dbo.InstalacionEquipoFotos WHERE ID=@p0", fotoId);
        var path = ResolvePath(row.RutaArchivo, row.TipoActivo, row.ActivoID);
        if (path is not null && File.Exists(path)) File.Delete(path);
        return (true, row.TipoActivo, row.ActivoID, path);
    }

    public string? ResolvePath(string relativePath, string tipo, int id)
    {
        if (tipo is not ("Instalacion" or "Equipo" or "Vehiculo" or "Cuba")) return null;
        var prefix = $"~/App_Data/InstalacionEquipoFotos/{tipo}/{id}/";
        if (string.IsNullOrWhiteSpace(relativePath) || !relativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;
        var root = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "InstalacionEquipoFotos", tipo, id.ToString()));
        var path = Path.GetFullPath(Path.Combine(root, relativePath[prefix.Length..].Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar)));
        return path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? path : null;
    }

    private async Task<string?> GetTitleAsync(string tipo, int id)
    {
        var sql = tipo switch
        {
            "Instalacion" => "SELECT CONCAT(Codigo,N' - ',Nombre) AS Value FROM gaillard.Instalacion WHERE Id=@p0",
            "Equipo" => "SELECT CONCAT(Codigo,N' - ',Nombre) AS Value FROM gaillard.Equipo WHERE Id=@p0",
            "Vehiculo" => "SELECT CONCAT(COALESCE(Marca,N''),N' ',COALESCE(Modelo,N''),N' - ',COALESCE(MatriculaVehiculo,N'')) AS Value FROM gaillard.Vehiculo WHERE ID=@p0",
            "Cuba" => "SELECT MatriculaCuba AS Value FROM gaillard.Cuba WHERE ID=@p0",
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
        return (await _db.Database.SqlQueryRaw<string>(sql, id).ToListAsync()).FirstOrDefault();
    }

    private async Task ImportLegacyCubaPhotosAsync(int cubaId)
    {
        var registrations = await _db.Database.SqlQueryRaw<LegacyCubaPhotoRow>(@"SELECT MatriculaCuba AS Matricula FROM gaillard.Cuba WHERE ID=@p0", cubaId).ToListAsync();
        var matricula = registrations.FirstOrDefault()?.Matricula;
        if (string.IsNullOrWhiteSpace(matricula)) return;

        var legacyRoot = Directory.Exists(Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "Content", "images")))
            ? Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "Content", "images"))
            : Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "Content", "images"));
        var legacyFolder = Path.GetFullPath(Path.Combine(legacyRoot, matricula));
        if (!legacyFolder.StartsWith(legacyRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !Directory.Exists(legacyFolder)) return;

        var knownNames = (await _db.Database.SqlQueryRaw<OriginalNameRow>(@"SELECT NombreOriginal AS Nombre FROM dbo.InstalacionEquipoFotos WHERE TipoActivo=N'Cuba' AND ActivoID=@p0", cubaId).ToListAsync())
            .Select(row => row.Nombre).Where(name => !string.IsNullOrWhiteSpace(name)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var importedNames = (await _db.Database.SqlQueryRaw<OriginalNameRow>(@"SELECT NombreArchivo AS Nombre FROM dbo.CubaFotoImportadas WHERE CubaId=@p0", cubaId).ToListAsync())
            .Select(row => row.Nombre).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var captions = await _db.Database.SqlQueryRaw<int>("SELECT CASE WHEN OBJECT_ID(N'dbo.CubaFotoObservaciones', N'U') IS NULL THEN 0 ELSE 1 END AS Value").SingleAsync() == 1
            ? await _db.Database.SqlQueryRaw<LegacyCubaCaptionRow>("SELECT NombreArchivo AS Nombre,Observacion FROM dbo.CubaFotoObservaciones WHERE CubaId=@p0", cubaId).ToListAsync()
            : [];
        var captionsByName = captions.ToDictionary(row => row.Nombre, row => row.Observacion, StringComparer.OrdinalIgnoreCase);
        var targetFolder = Path.Combine(_environment.ContentRootPath, "App_Data", "InstalacionEquipoFotos", "Cuba", cubaId.ToString());

        foreach (var sourcePath in Directory.EnumerateFiles(legacyFolder))
        {
            var originalName = Path.GetFileName(sourcePath);
            var extension = Path.GetExtension(originalName).ToLowerInvariant();
            if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(extension, StringComparer.OrdinalIgnoreCase) || importedNames.Contains(originalName)) continue;
            if (knownNames.Contains(originalName))
            {
                await MarkLegacyCubaPhotoImportedAsync(cubaId, originalName);
                importedNames.Add(originalName);
                continue;
            }

            Directory.CreateDirectory(targetFolder);
            var storedName = Guid.NewGuid().ToString("N") + extension;
            var targetPath = Path.Combine(targetFolder, storedName);
            try
            {
                File.Copy(sourcePath, targetPath, overwrite: false);
                var caption = captionsByName.GetValueOrDefault(originalName);
                var contentType = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (!contentType.TryGetContentType(originalName, out var mediaType)) mediaType = "application/octet-stream";
                await using var transaction = await _db.Database.BeginTransactionAsync();
                await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO dbo.InstalacionEquipoFotos (TipoActivo,ActivoID,RutaArchivo,NombreOriginal,ContentType,FechaCreacion,Observacion)
                    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)", ["Cuba", cubaId, $"~/App_Data/InstalacionEquipoFotos/Cuba/{cubaId}/{storedName}", originalName, mediaType, File.GetCreationTime(sourcePath), ObservationParameter("@p6", caption)]);
                await MarkLegacyCubaPhotoImportedAsync(cubaId, originalName);
                await transaction.CommitAsync();
                knownNames.Add(originalName);
                importedNames.Add(originalName);
            }
            catch
            {
                if (File.Exists(targetPath)) File.Delete(targetPath);
                throw;
            }
        }
    }

    private Task<int> MarkLegacyCubaPhotoImportedAsync(int cubaId, string fileName) =>
        _db.Database.ExecuteSqlRawAsync(@"IF NOT EXISTS (SELECT 1 FROM dbo.CubaFotoImportadas WHERE CubaId=@p0 AND NombreArchivo=@p1)
            INSERT INTO dbo.CubaFotoImportadas (CubaId,NombreArchivo,ImportadaEn) VALUES (@p0,@p1,@p2)", [cubaId, fileName, DateTime.Now]);

    private sealed class FotoRow { public string RutaArchivo { get; set; } = string.Empty; public string TipoActivo { get; set; } = string.Empty; public int ActivoID { get; set; } public string? ContentType { get; set; } }
    private sealed class LegacyCubaPhotoRow { public string? Matricula { get; set; } }
    private sealed class OriginalNameRow { public string? Nombre { get; set; } }
    private sealed class LegacyCubaCaptionRow { public string Nombre { get; set; } = string.Empty; public string? Observacion { get; set; } }
    private static string? CaptionAt(IReadOnlyList<string?> captions, int index) => index < captions.Count && !string.IsNullOrWhiteSpace(captions[index]) ? captions[index]!.Trim() : null;
    private static object Db(string? value) => (object?)value ?? DBNull.Value;
    private static SqlParameter ObservationParameter(string name, string? value) => new(name, SqlDbType.NVarChar, 500)
    {
        Value = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim()
    };
    private sealed record PendingPhoto(string PhysicalPath, string RelativePath, string Name, string ContentType, string? Observacion);
}
