using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TipoAccesorioMetaData))]
    public partial class TipoAccesorio
    {
    }
    public class TipoAccesorioMetaData
    {
        [Display(Name = "Tipo de Accesorio")]
        public string TipoAccesorio1 { get; set; }
    }
}