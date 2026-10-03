using CasaGaillard.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaGaillard.Core.Services
{
    public interface ICrudVehiculosService
    {
        Task<IEnumerable<Vehiculo>> GetAllAsync(bool incluirBajas = false, int page = 1);
        Task<Vehiculo?> GetByIdAsync(int id);
        Task AddAsync(Vehiculo vehiculo);
        Task UpdateAsync(Vehiculo vehiculo);
        Task DeleteAsync(int id);
    }
}
