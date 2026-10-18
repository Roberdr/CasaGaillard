using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IFamiliasService
{
    Task<FamiliasSubfamiliasIndexViewModel> GetIndexAsync();
    Task<FamiliaFormViewModel?> GetFamiliaAsync(int id);
    Task<bool> FamiliaNombreExisteAsync(string nombre, int id);
    Task SaveFamiliaAsync(FamiliaFormViewModel model);
    Task<bool> UpdateFamiliaAsync(FamiliaFormViewModel model);
    Task<(bool Deleted, bool Exists)> DeleteFamiliaAsync(int id);
    Task<SubfamiliaFormViewModel?> GetSubfamiliaAsync(int id);
    Task<IReadOnlyList<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetFamiliasAsync(int? selectedId);
    Task<bool> FamiliaExisteAsync(int id);
    Task<bool> SubfamiliaNombreExisteAsync(int familiaId, string nombre, int id);
    Task SaveSubfamiliaAsync(SubfamiliaFormViewModel model);
    Task<bool> UpdateSubfamiliaAsync(SubfamiliaFormViewModel model);
    Task<(bool Deleted, bool Exists)> DeleteSubfamiliaAsync(int id);
}
