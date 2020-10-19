using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(CombustibleMetaData))]
    public partial class Combustible
    {
    }
    public class CombustibleMetaData
    {
        [Display(Name = "Combustible")]
        public string Combustible1 { get; set; }
    }
}