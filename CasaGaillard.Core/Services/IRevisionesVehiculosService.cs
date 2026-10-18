using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IRevisionesVehiculosService
{
    Task<IReadOnlyList<RevisionVehiculoFilaViewModel>> GetAllAsync(string? matricula, bool incluirBajas);
    Task<IReadOnlyList<RevisionVehiculoFilaViewModel>> GetProximasAsync();
    Task<RevisionVehiculoFormViewModel?> GetFormAsync(int? id, int? vehiculoId, int? tipoId);
    Task<bool> SaveAsync(RevisionVehiculoFormViewModel model, bool create);
    Task<bool> DeleteAsync(int id);
}
