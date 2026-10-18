using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IIntervencionesService
{
    Task<IntervencionesIndexViewModel?> GetIndexAsync(int? tareaId);
    Task<TareaMantenimientoResumen?> GetTareaSummaryAsync(int id);
    Task<IntervencionFormViewModel?> GetCreateAsync(int tareaId);
    Task<IntervencionFormViewModel?> GetForEditAsync(int id);
    Task<IntervencionDetailsViewModel?> GetDetailsAsync(int id, int? nuevoAccesorioId = null);
    Task<IReadOnlyList<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetPersonasAsync(int? selectedId);
    Task<IReadOnlyList<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetAccesoriosAsync(int selectedId);
    Task<bool> SaveCreateAsync(IntervencionFormViewModel model, string? creadoPor);
    Task<bool> SaveUpdateAsync(IntervencionFormViewModel model);
    Task<bool> AddRepuestoAsync(IntervencionRepuestoFormViewModel model);
    Task<int?> RemoveRepuestoAsync(int id);
}
