using CasaGaillard.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaGaillard.Core.Services
{
    public interface ICrudVehiculosService
    {
        Task<VehiculosIndexViewModel> GetAllAsync(
            string? matricula,
            string? marca,
            string? modelo,
            string? tipo,
            bool incluirBajas,
            int page,
            int pageSize = 8);
        Task<Vehiculo?> GetByIdAsync(int id);
        Task<IReadOnlyList<TipoVehiculo>> GetTiposVehiculoAsync();
        Task<IReadOnlyList<Entidad>> GetTalleresAsync();
        Task AddAsync(Vehiculo vehiculo);
        Task UpdateAsync(Vehiculo vehiculo);
        Task DeleteAsync(int id);
    }
}
