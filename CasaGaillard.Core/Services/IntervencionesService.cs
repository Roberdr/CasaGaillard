using System.Collections.Generic;
using System.Threading.Tasks;
using CasaGaillard.Core.Models;

namespace CasaGaillard.Core.Services
{
    public class IntervencionesService : IIntervencionesService
    {
        public IntervencionesService()
        {
            // TODO: inject DbContext or repository when available
        }

        public Task<IEnumerable<IntervencionViewModel>> GetAllAsync()
        {
            // return empty list for now
            IEnumerable<IntervencionViewModel> empty = new List<IntervencionViewModel>();
            return Task.FromResult(empty);
        }

        public Task<IntervencionViewModel?> GetByIdAsync(int id)
        {
            return Task.FromResult<IntervencionViewModel?>(null);
        }

        public Task CreateAsync(IntervencionViewModel model)
        {
            // stub: do nothing
            return Task.CompletedTask;
        }

        public Task UpdateAsync(int id, IntervencionViewModel model)
        {
            // stub: do nothing
            return Task.CompletedTask;
        }

        public Task AddRepuestoAsync(int intervenId, int repuestoId)
        {
            return Task.CompletedTask;
        }

        public Task RemoveRepuestoAsync(int id)
        {
            return Task.CompletedTask;
        }
    }
}
