using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CasaGaillard.Core.Services;

public interface ITiposAccesorioService
{
    Task<IReadOnlyList<TipoAccesorioFilaViewModel>> GetAllAsync();
    Task<IReadOnlyList<SelectListItem>> GetSubfamiliasAsync(int? selectedId);
    Task<bool> SubfamiliaExisteAsync(int id);
    Task<TipoAccesorioFilaViewModel?> FindByNameAsync(string nombre);
    Task<int> CreateAsync(TipoAccesorioFormViewModel model);
}
