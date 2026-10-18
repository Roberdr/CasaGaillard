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
        [Display(Name = "Matrícula")]
        public string? MatriculaVehiculo { get; set; }
        [Display(Name = "Tipo de vehículo")]
        public int? TipoVehiculoID { get; set; }
        [Display(Name = "Modelo del tacógrafo")]
        public string? ModeloTacografo { get; set; }
        [Display(Name = "P.M.A.")]
        public int? Pma { get; set; }
        [Display(Name = "T.A.R.A.")]
        public int? Tara { get; set; }
        [Display(Name = "Fecha de compra"), DataType(DataType.Date)]
        public DateTime? FechaCompra { get; set; }
        [Display(Name = "Taller habitual")]
        public int? TallerHabitualID { get; set; }
        [Display(Name = "Dado de baja")]
        public bool? Baja { get; set; }

        // Navigation props (minimal)
        public TipoVehiculo? TipoVehiculo { get; set; }
        public Entidad? Taller { get; set; }
    }
}
