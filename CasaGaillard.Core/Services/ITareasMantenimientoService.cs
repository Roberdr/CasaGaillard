using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface ITareasMantenimientoService
{
    Task<TareasMantenimientoIndexViewModel> GetAllAsync(string? busqueda, string? estado, string? prioridad, int pagina, DateTime? fechaDesde = null, DateTime? fechaHasta = null, string? equipo = null, string? responsable = null);
    Task<TareaMantenimientoDetailsViewModel?> GetDetailsAsync(int id);
    Task<TareaMantenimientoFoto?> GetFotoAsync(int id);
    Task<TareaMantenimientoFormViewModel?> GetFormAsync(int? id);
    Task<TareaMantenimientoFormViewModel> PrepareFormAsync(TareaMantenimientoFormViewModel model);
    Task<int> CreateAsync(TareaMantenimientoFormViewModel model, string? userName);
    Task<bool> UpdateAsync(TareaMantenimientoFormViewModel model, string? userName);
    Task<TareaMantenimientoGestionViewModel?> GetGestionAsync(int id);
    Task<TareaMantenimientoGestionViewModel> PrepareGestionAsync(TareaMantenimientoGestionViewModel model);
    Task<bool> UpdateGestionAsync(TareaMantenimientoGestionViewModel model, string? userName);
    Task<bool> ClaimAsync(int id, string userName);
    Task AddFotoAsync(int tareaId, string fileName, string originalName, string contentType, string? observacion, DateTime uploadedAt);
}
