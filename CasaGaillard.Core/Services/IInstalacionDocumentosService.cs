using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IInstalacionDocumentosService
{
    Task<InstalacionDocumentoFormViewModel?> GetAsync(int instalacionId);
    Task<bool> SaveAsync(InstalacionDocumentoFormViewModel model, CancellationToken cancellationToken = default);
    Task<(string Nombre, string RutaArchivo)?> GetFileAsync(int id);
    Task<int?> DeleteAsync(int id);
    string? ResolveStoredPath(string storedPath);
}
