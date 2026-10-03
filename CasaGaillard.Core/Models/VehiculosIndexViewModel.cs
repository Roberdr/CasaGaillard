using System.Collections.Generic;

namespace CasaGaillard.Core.Models
{
    public class VehiculosIndexViewModel
    {
        public IEnumerable<Vehiculo> PagedList { get; set; } = new List<Vehiculo>();
    }
}
