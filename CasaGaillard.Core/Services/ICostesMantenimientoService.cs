using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services;

public interface ICostesMantenimientoService
{
    Task<CostesMantenimientoViewModel> GetAsync(DateTime? desde,DateTime? hasta,int? instalacionId,int? equipoId);
}
