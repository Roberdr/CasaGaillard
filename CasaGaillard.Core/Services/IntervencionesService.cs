using System.Collections.Generic;
using System.Threading.Tasks;
using CasaGaillard.Core.Models;
using CasaGaillard.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services
{
    public class IntervencionesService : IIntervencionesService
    {
        private readonly AppDbContext _db;

        public IntervencionesService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<IntervencionViewModel>> GetAllAsync()
        {
            return await _db.Intervenciones.AsNoTracking().ToListAsync();
        }

        public async Task<IntervencionViewModel?> GetByIdAsync(int id)
        {
            return await _db.Intervenciones.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateAsync(IntervencionViewModel model)
        {
            await _db.Intervenciones.AddAsync(model);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, IntervencionViewModel model)
        {
            // map changes
            var existing = await _db.Intervenciones.FindAsync(id);
            if (existing == null) return;
            existing.Titulo = model.Titulo;
            existing.Descripcion = model.Descripcion;
            await _db.SaveChangesAsync();
        }

        public Task AddRepuestoAsync(int intervenId, int repuestoId)
        {
            // TODO: implement relation with Repuestos entity
            return Task.CompletedTask;
        }

        public Task RemoveRepuestoAsync(int id)
        {
            // TODO: implement removal logic
            return Task.CompletedTask;
        }
    }
}
