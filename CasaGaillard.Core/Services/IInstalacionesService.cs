using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IInstalacionesService
{
    Task<IReadOnlyList<InstalacionFilaViewModel>> GetAllAsync(bool incluirBajas);
    Task<InstalacionFormViewModel?> GetFormAsync(int? id);
    Task<InstalacionFormViewModel> PrepareFormAsync(InstalacionFormViewModel model);
    Task<IReadOnlyDictionary<string, string>> ValidateAsync(InstalacionFormViewModel model);
    Task<bool> SaveAsync(InstalacionFormViewModel model);
    Task DarDeBajaAsync(int id);
}
