using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TelefonoPersonaMetaData))]
    public partial class TelefonoPersona
    {
    }
    public class TelefonoPersonaMetaData
    {
        [Display(Name = "Teléfono")]
        public string TelefonoPersona1 { get; set; }
    }
}