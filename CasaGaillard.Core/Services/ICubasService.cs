using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface ICubasService
{
    Task<IReadOnlyList<CubaFilaViewModel>> GetAllAsync(bool incluirBajas);
    Task<CubaDetalleViewModel?> GetByIdAsync(int id);
    Task<CubaFormViewModel?> GetFormAsync(int? id);
    Task<bool> SaveAsync(CubaFormViewModel model, bool create);
    Task<bool> DeleteAsync(int id);
    Task<(string Path, string ContentType)?> GetImageAsync(int id, string fileName);
}
