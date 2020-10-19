using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(CaracteristicaAccesorioMetaData))]
    public partial class CaracteristicaAccesorio
    {
    }
    public class CaracteristicaAccesorioMetaData
    {
        [Display(Name = "Característica")]
        public string CaracteristicaAccesorio1 { get; set; }
    }
}