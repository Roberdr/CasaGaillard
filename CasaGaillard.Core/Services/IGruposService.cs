using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IGruposService
{
    Task<IReadOnlyList<GrupoFilaViewModel>> GetAllAsync();
    Task<GrupoFormViewModel?> GetFormAsync(int? id);
    Task<string?> ValidateAsync(GrupoFormViewModel model);
    Task SaveAsync(GrupoFormViewModel model, bool create);
    Task<bool> DeleteAsync(int id);
}
