using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IPlanesMantenimientoService
{
    Task<IReadOnlyList<PlanMantenimientoFilaViewModel>> GetAllAsync();
    Task<PlanMantenimientoDetalleViewModel?> GetDetailsAsync(int id);
    Task<PlanMantenimientoFormViewModel?> GetFormAsync(int? id);
    Task<PlanMantenimientoFormViewModel> PrepareFormAsync(PlanMantenimientoFormViewModel model);
    Task<IReadOnlyDictionary<string, string>> ValidateAsync(PlanMantenimientoFormViewModel model);
    Task<bool> SaveAsync(PlanMantenimientoFormViewModel model);
    Task<bool> DeleteAsync(int id);
}
