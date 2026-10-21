using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IFotosActivosService
{
    Task<InstalacionEquipoFotosViewModel?> GetAsync(string tipo, int id);
    Task<(string PhysicalPath, string ContentType)?> GetFotoAsync(int id);
    Task<bool> UploadAsync(string tipo, int id, IReadOnlyList<Microsoft.AspNetCore.Http.IFormFile> files, IReadOnlyList<string?> observaciones, CancellationToken cancellationToken = default);
    Task<(bool Exists, string Tipo, int ActivoId, string? Ruta)> DeleteAsync(int fotoId);
    string? ResolvePath(string relativePath, string tipo, int id);
}
