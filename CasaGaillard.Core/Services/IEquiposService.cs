using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Services;

public interface IEquiposService
{
    Task<IReadOnlyList<EquipoFilaViewModel>> GetAllAsync(int? instalacionId, int? vehiculoId, bool incluirBajas);
    Task<EquipoDetailsViewModel?> GetDetailsAsync(int id);
    Task<IReadOnlyList<SelectListItem>> GetInstalacionesAsync(int? selectedId);
    Task<IReadOnlyList<SelectListItem>> GetVehiculosAsync(int? selectedId);
    Task<IReadOnlyList<SelectListItem>> GetEquiposPadreAsync(int instalacionId, int? equipoId, int? selectedId = null);
    Task<EquipoFormViewModel?> GetFormAsync(int? id, int? instalacionId = null);
    Task<EquipoFormViewModel> PrepareFormAsync(EquipoFormViewModel model);
    Task<IReadOnlyDictionary<string, string>> ValidateAsync(EquipoFormViewModel model);
    Task<bool> SaveAsync(EquipoFormViewModel model);
    Task DarDeBajaAsync(int id);
}
