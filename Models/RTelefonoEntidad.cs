using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TelefonoEntidadMetaData))]
    public partial class TelefonoEntidad
    {
    }
    public class TelefonoEntidadMetaData
    {
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
    }
}