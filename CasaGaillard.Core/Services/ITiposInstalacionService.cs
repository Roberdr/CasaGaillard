using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface ITiposInstalacionService
{
    Task<IReadOnlyList<TipoInstalacionCatalogoViewModel>> GetAllAsync();
    Task<TipoInstalacionCatalogoViewModel?> GetAsync(int id);
    Task<bool> NombreExisteAsync(string nombre, int id);
    Task SaveAsync(TipoInstalacionCatalogoViewModel model);
    Task<bool> UpdateAsync(TipoInstalacionCatalogoViewModel model);
    Task<(bool Deleted, bool Exists)> DeleteAsync(int id);
}
