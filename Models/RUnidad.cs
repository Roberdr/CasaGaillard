using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(UnidadMetaData))]
    public partial class Unidad
    {
    }
    public class UnidadMetaData
    {
        [Display(Name = "Unidad")]
        public string Unidad1 { get; set; }
    }
}