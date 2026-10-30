using System.Collections.Generic;

namespace CasaGaillard.Core.Models
{
    public class VehiculosIndexViewModel
    {
        public IEnumerable<Vehiculo> PagedList { get; set; } = new List<Vehiculo>();
        public string? Matricula { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Tipo { get; set; }
        public IEnumerable<string> TiposDisponibles { get; set; } = new List<string>();
        public bool IncluirBajas { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalCount { get; set; }
        public int PageCount => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < PageCount;
    }
}
