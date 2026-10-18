using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IAccesoriosService
{
    Task<AccesoriosIndexViewModel> GetIndexAsync(int? tipoId, int? materialId, int? familiaId, int? subfamiliaId, string? query, int page, int pageSize);
    Task<AccesorioDetailsViewModel?> GetDetailsAsync(int id);
    Task<AccesorioFormViewModel?> GetFormAsync(int? id, int? intervencionId = null);
    Task<IReadOnlyList<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetSubfamiliasAsync(int? familiaId, int? selectedId);
    Task<int> SaveAsync(AccesorioFormViewModel model, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id);
    Task<(string RutaArchivo, string? ContentType)?> GetFotoAsync(int id);
    string? ResolveFotoPath(string rutaArchivo);
}
