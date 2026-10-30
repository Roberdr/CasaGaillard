using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IRevisionesService
{
    Task<RevisionesIndexViewModel> GetIndexAsync(int? cubaId, string? matriculaCuba, bool incluirBajas);
    Task<RevisionDetalleViewModel?> GetDetailsAsync(int id);
    Task<RevisionFormViewModel?> GetFormAsync(int? id, int? cubaId = null);
    Task<RevisionFormViewModel> PrepareFormAsync(RevisionFormViewModel model);
    Task<IReadOnlyDictionary<string, string>> ValidateAsync(RevisionFormViewModel model, bool creating);
    Task<bool> SaveAsync(RevisionFormViewModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateReservaAsync(int id, DateTime? fechaReservada);
}
