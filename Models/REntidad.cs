using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(EntidadMetaData))]
    public partial class Entidad
    {
    }
    public class EntidadMetaData
    {
        [Display(Name = "Entidad")]
        public string NombreEntidad { get; set; }
    }
}