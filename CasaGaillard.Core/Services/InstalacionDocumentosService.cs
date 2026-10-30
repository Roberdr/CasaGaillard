using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class InstalacionDocumentosService : IInstalacionDocumentosService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;
    public InstalacionDocumentosService(AppDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    public async Task<InstalacionDocumentoFormViewModel?> GetAsync(int instalacionId)
    {
        var instalacion = await GetInstalacionAsync(instalacionId);
        if (instalacion is null) return null;
        return new InstalacionDocumentoFormViewModel
        {
            InstalacionId = instalacion.Id,
            InstalacionCodigo = instalacion.Codigo,
            InstalacionNombre = instalacion.Nombre,
            Documentos = await GetDocumentsAsync(instalacionId)
        };
    }

    public async Task<bool> SaveAsync(InstalacionDocumentoFormViewModel model, CancellationToken cancellationToken = default)
    {
        var file = model.Archivo!;
        var extension = Path.GetExtension(Path.GetFileName(file.FileName)).ToLowerInvariant();
        var folder = Path.Combine(_environment.ContentRootPath, "App_Data", "InstalacionesDocumentos", model.InstalacionId.ToString());
        Directory.CreateDirectory(folder);
        var physicalPath = Path.Combine(folder, Guid.NewGuid().ToString("N") + extension);
        var storedPath = $"~/App_Data/InstalacionesDocumentos/{model.InstalacionId}/{Path.GetFileName(physicalPath)}";

        await using (var output = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            await file.CopyToAsync(output, cancellationToken);
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"INSERT INTO gaillard.InstalacionDocumento
                (InstalacionId, Nombre, RutaArchivo, TipoDocumento, CodigoPlano, FechaDocumento)
                VALUES (@p0,@p1,@p2,@p3,@p4,@p5)",
                [model.InstalacionId, Path.GetFileName(file.FileName), storedPath, SqlValue(model.TipoDocumento), SqlValue(model.CodigoPlano), SqlValue(model.FechaDocumento)]);
        }
        catch
        {
            if (File.Exists(physicalPath)) File.Delete(physicalPath);
            throw;
        }
        return true;
    }

    public async Task<(string Nombre, string RutaArchivo)?> GetFileAsync(int id)
    {
        var rows = await _db.Database.SqlQueryRaw<DocumentoArchivo>(@"SELECT Nombre,RutaArchivo
            FROM gaillard.InstalacionDocumento WHERE Id=@p0", id).ToListAsync();
        return rows.Count == 0 ? null : (rows[0].Nombre, rows[0].RutaArchivo);
    }

    public async Task<int?> DeleteAsync(int id)
    {
        var file = await GetFileAsync(id);
        if (file is null) return null;
        var instalacionId = await _db.Database.SqlQueryRaw<int>(@"SELECT InstalacionId AS Value
            FROM gaillard.InstalacionDocumento WHERE Id=@p0", id).SingleAsync();
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM gaillard.InstalacionDocumento WHERE Id=@p0", id);
        var path = ResolveStoredPath(file.Value.RutaArchivo);
        if (path is not null && File.Exists(path)) File.Delete(path);
        return instalacionId;
    }

    public string? ResolveStoredPath(string storedPath)
    {
        const string prefix = "~/App_Data/InstalacionesDocumentos/";
        if (string.IsNullOrWhiteSpace(storedPath) || !storedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;
        var root = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "InstalacionesDocumentos"));
        var relative = storedPath[prefix.Length..].Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        var path = Path.GetFullPath(Path.Combine(root, relative));
        return path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? path : null;
    }

    private async Task<InstalacionOpcionViewModel?> GetInstalacionAsync(int id) =>
        (await _db.Database.SqlQueryRaw<InstalacionOpcionViewModel>(@"SELECT Id,Codigo,Nombre FROM gaillard.Instalacion WHERE Id=@p0", id).ToListAsync()).FirstOrDefault();

    private async Task<IReadOnlyList<InstalacionDocumentoFilaViewModel>> GetDocumentsAsync(int id) =>
        await _db.Database.SqlQueryRaw<InstalacionDocumentoFilaViewModel>(@"SELECT d.Id,d.InstalacionId,
            i.Codigo AS InstalacionCodigo,i.Nombre AS InstalacionNombre,d.Nombre,d.TipoDocumento,d.CodigoPlano,d.FechaDocumento,d.FechaAlta
            FROM gaillard.InstalacionDocumento d INNER JOIN gaillard.Instalacion i ON i.Id=d.InstalacionId
            WHERE d.InstalacionId=@p0 ORDER BY d.FechaAlta DESC,d.Nombre", id).ToListAsync();

    private static object SqlValue(object? value) => value switch
    {
        null => DBNull.Value,
        string text when string.IsNullOrWhiteSpace(text) => DBNull.Value,
        string text => text.Trim(),
        _ => value
    };
    private sealed class DocumentoArchivo { public string Nombre { get; set; } = string.Empty; public string RutaArchivo { get; set; } = string.Empty; }
}
