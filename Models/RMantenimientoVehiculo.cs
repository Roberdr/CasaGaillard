using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(MantenimientoVehiculoMetaData))]
    public partial class MantenimientoVehiculo
    {
    }
    public class MantenimientoVehiculoMetaData
    {
        [Display(Name = "Fecha Mantenimiento")]
        public Nullable<System.DateTime> FechaMantenimiento { get; set; }

        [Display(Name = "Detalle Mantenimiento")]
        public string DetalleMantenimiento { get; set; }

        [Display(Name = "Kilómetros")]
        public Nullable<int> Kilometros { get; set; }

    }
}