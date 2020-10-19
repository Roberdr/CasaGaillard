using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TipoViaMetaData))]
    public partial class TipoVia
    {
    }
    public class TipoViaMetaData
    {
        [Display(Name = "Tipo Vía")]
        public string TipoVia1 { get; set; }
    }
}