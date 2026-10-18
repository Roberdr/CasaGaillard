using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;

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
        var photos = await _db.Database.SqlQueryRaw<InstalacionEquipoFotoFilaViewModel>(@"SELECT ID AS Id,NombreOriginal,FechaCreacion
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

    public async Task<bool> UploadAsync(string tipo, int id, IReadOnlyList<Microsoft.AspNetCore.Http.IFormFile> files, CancellationToken cancellationToken = default)
    {
        var folder = Path.Combine(_environment.ContentRootPath, "App_Data", "InstalacionEquipoFotos", tipo, id.ToString());
        Directory.CreateDirectory(folder);
        var pending = new List<PendingPhoto>();
        try
        {
            foreach (var file in files)
            {
                var extension = Path.GetExtension(Path.GetFileName(file.FileName)).ToLowerInvariant();
                var name = Guid.NewGuid().ToString("N") + extension;
                var physicalPath = Path.Combine(folder, name);
                await using (var stream = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    await file.CopyToAsync(stream, cancellationToken);
                pending.Add(new PendingPhoto(physicalPath, $"~/App_Data/InstalacionEquipoFotos/{tipo}/{id}/{name}", Path.GetFileName(file.FileName), file.ContentType));
            }
            foreach (var photo in pending)
                await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO dbo.InstalacionEquipoFotos (TipoActivo,ActivoID,RutaArchivo,NombreOriginal,ContentType,FechaCreacion)
                    VALUES (@p0,@p1,@p2,@p3,@p4,@p5)", [tipo, id, photo.RelativePath, photo.Name, photo.ContentType, DateTime.Now], cancellationToken);
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
        if (tipo is not ("Instalacion" or "Equipo")) return null;
        var prefix = $"~/App_Data/InstalacionEquipoFotos/{tipo}/{id}/";
        if (string.IsNullOrWhiteSpace(relativePath) || !relativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;
        var root = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "InstalacionEquipoFotos", tipo, id.ToString()));
        var path = Path.GetFullPath(Path.Combine(root, relativePath[prefix.Length..].Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar)));
        return path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? path : null;
    }

    private async Task<string?> GetTitleAsync(string tipo, int id)
    {
        var sql = tipo == "Instalacion"
            ? "SELECT CONCAT(Codigo,N' - ',Nombre) AS Value FROM gaillard.Instalacion WHERE Id=@p0"
            : "SELECT CONCAT(Codigo,N' - ',Nombre) AS Value FROM gaillard.Equipo WHERE Id=@p0";
        return (await _db.Database.SqlQueryRaw<string>(sql, id).ToListAsync()).FirstOrDefault();
    }
    private sealed class FotoRow { public string RutaArchivo { get; set; } = string.Empty; public string TipoActivo { get; set; } = string.Empty; public int ActivoID { get; set; } public string? ContentType { get; set; } }
    private sealed record PendingPhoto(string PhysicalPath, string RelativePath, string Name, string ContentType);
}
