using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IUbicacionesService
{
    Task<IReadOnlyList<UbicacionCatalogoViewModel>> GetAllAsync();
    Task<UbicacionCatalogoViewModel?> GetAsync(int id);
    Task<bool> CodigoExisteAsync(string? codigo, int id);
    Task SaveAsync(UbicacionCatalogoViewModel model);
    Task<bool> UpdateAsync(UbicacionCatalogoViewModel model);
    Task<(bool Deleted, bool Exists)> DeleteAsync(int id);
}
