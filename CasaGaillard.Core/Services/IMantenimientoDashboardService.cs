using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IMantenimientoDashboardService
{
    Task<MantenimientoDashboardViewModel> GetAsync();
}
