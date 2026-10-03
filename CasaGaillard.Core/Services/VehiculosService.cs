using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaGaillard.Core.Services
{
    public class VehiculosService : ICrudVehiculosService
    {
        private readonly AppDbContext _db;

        public VehiculosService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Vehiculo>> GetAllAsync(bool incluirBajas = false, int page = 1)
        {
            var query = _db.Set<Vehiculo>()
                .Include(v => v.TipoVehiculo)
                .Include(v => v.Taller)
                .OrderBy(v => v.MatriculaVehiculo)
                .AsQueryable();

            if (!incluirBajas)
                query = query.Where(v => v.Baja != true);

            return await query.ToListAsync();
        }

        public async Task<Vehiculo?> GetByIdAsync(int id)
        {
            return await _db.Set<Vehiculo>()
                .Include(v => v.TipoVehiculo)
                .Include(v => v.Taller)
                .FirstOrDefaultAsync(v => v.ID == id);
        }

        public async Task AddAsync(Vehiculo vehiculo)
        {
            _db.Set<Vehiculo>().Add(vehiculo);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vehiculo vehiculo)
        {
            _db.Set<Vehiculo>().Update(vehiculo);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var v = await _db.Set<Vehiculo>().FindAsync(id);
            if (v != null)
            {
                _db.Set<Vehiculo>().Remove(v);
                await _db.SaveChangesAsync();
            }
        }
    }
}
