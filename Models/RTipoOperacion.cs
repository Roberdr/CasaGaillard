using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TipoOperacionMetaData))]
    public partial class TipoOperacion
    {
    }
    public class TipoOperacionMetaData
    {
        [Display(Name = "Cargo")]
        public string Operacion { get; set; }
    }
}