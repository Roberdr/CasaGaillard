using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(CargoMetaData))]
    public partial class Cargo
    {
    }
    public class CargoMetaData
    {
        [Display(Name = "Cargo")]
        public string Cargo1 { get; set; }
    }
}