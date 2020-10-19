using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TipoVehiculoMetaData))]
    public partial class TipoVehiculo
    {
    }
    public class TipoVehiculoMetaData
    {
        [Display(Name = "Vehículo")]
        public string Vehiculo { get; set; }
    }
}