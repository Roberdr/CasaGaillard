using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Services;

public interface IEquiposService
{
    Task<IReadOnlyList<EquipoFilaViewModel>> GetAllAsync(int? instalacionId, bool incluirBajas);
    Task<IReadOnlyList<SelectListItem>> GetInstalacionesAsync(int? selectedId);
    Task<EquipoFormViewModel?> GetFormAsync(int? id, int? instalacionId = null);
    Task<EquipoFormViewModel> PrepareFormAsync(EquipoFormViewModel model);
    Task<IReadOnlyDictionary<string, string>> ValidateAsync(EquipoFormViewModel model);
    Task<bool> SaveAsync(EquipoFormViewModel model);
    Task DarDeBajaAsync(int id);
}
