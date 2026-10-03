using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models
{
    public class Vehiculo
    {
        public int ID { get; set; }
        [StringLength(100)]
        public string? Marca { get; set; }
        [StringLength(100)]
        public string? Modelo { get; set; }
        [StringLength(20)]
        public string? MatriculaVehiculo { get; set; }
        public int? TipoVehiculoID { get; set; }
        public string? ModeloTacografo { get; set; }
        public decimal? Pma { get; set; }
        public decimal? Tara { get; set; }
        public DateTime? FechaCompra { get; set; }
        public int? TallerHabitualID { get; set; }
        public bool? Baja { get; set; }

        // Navigation props (minimal)
        public TipoVehiculo? TipoVehiculo { get; set; }
        public Entidad? Taller { get; set; }
    }
}
