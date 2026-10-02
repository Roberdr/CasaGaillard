using System.Collections.Generic;
using System.Threading.Tasks;
using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services
{
    public interface IIntervencionesService
    {
        Task<IEnumerable<IntervencionViewModel>> GetAllAsync();
        Task<IntervencionViewModel?> GetByIdAsync(int id);
        Task CreateAsync(IntervencionViewModel model);
        Task UpdateAsync(int id, IntervencionViewModel model);
        Task AddRepuestoAsync(int intervenId, int repuestoId);
        Task RemoveRepuestoAsync(int id);
    }
}
