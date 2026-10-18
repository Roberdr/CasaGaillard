using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface IPaginaPublicaService
{
    Task<PaginaInicioViewModel> GetHomeAsync();
    Task<PaginaStockViewModel> GetStockAsync();
    IReadOnlyList<ServicioPublicoViewModel> GetServices();
    IReadOnlyList<SeguridadPublicaViewModel> GetSafetyRecommendations();
    Task<(string Path, string ContentType)?> GetImageAsync(string archivo);
}
