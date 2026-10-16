using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<VehiculosIndexViewModel> GetAllAsync(
            string? matricula,
            string? marca,
            string? modelo,
            string? tipo,
            bool incluirBajas,
            int page,
            int pageSize = 8)
        {
            var query = _db.Set<Vehiculo>()
                .Include(v => v.TipoVehiculo)
                .Include(v => v.Taller)
                .OrderBy(v => v.MatriculaVehiculo)
                .AsQueryable();

            if (!incluirBajas)
                query = query.Where(v => v.Baja != true);

            if (!string.IsNullOrWhiteSpace(matricula))
                query = query.Where(v => v.MatriculaVehiculo != null && v.MatriculaVehiculo.Contains(matricula));

            if (!string.IsNullOrWhiteSpace(marca))
                query = query.Where(v => v.Marca != null && v.Marca.Contains(marca));

            if (!string.IsNullOrWhiteSpace(modelo))
                query = query.Where(v => v.Modelo != null && v.Modelo.Contains(modelo));

            if (!string.IsNullOrWhiteSpace(tipo))
                query = query.Where(v => v.TipoVehiculo != null && v.TipoVehiculo.Vehiculo == tipo);

            var totalCount = await query.CountAsync();
            var safePageSize = pageSize > 0 ? pageSize : 8;
            var pageCount = (int)System.Math.Ceiling(totalCount / (double)safePageSize);
            var safePage = System.Math.Clamp(page, 1, System.Math.Max(pageCount, 1));
            var items = await query
                .Skip((safePage - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync();
            var tiposDisponibles = await _db.Set<TipoVehiculo>()
                .Where(t => t.Vehiculo != null)
                .OrderBy(t => t.Vehiculo)
                .Select(t => t.Vehiculo!)
                .Distinct()
                .ToListAsync();

            return new VehiculosIndexViewModel
            {
                PagedList = items,
                Matricula = matricula,
                Marca = marca,
                Modelo = modelo,
                Tipo = tipo,
                TiposDisponibles = tiposDisponibles,
                IncluirBajas = incluirBajas,
                PageNumber = safePage,
                PageSize = safePageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Vehiculo?> GetByIdAsync(int id)
        {
            return await _db.Set<Vehiculo>()
                .Include(v => v.TipoVehiculo)
                .Include(v => v.Taller)
                .FirstOrDefaultAsync(v => v.ID == id);
        }

        public async Task<IReadOnlyList<TipoVehiculo>> GetTiposVehiculoAsync()
        {
            return await _db.Set<TipoVehiculo>()
                .OrderBy(tipo => tipo.Vehiculo)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Entidad>> GetTalleresAsync()
        {
            return await _db.Set<Entidad>()
                .OrderBy(taller => taller.NombreEntidad)
                .ToListAsync();
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
