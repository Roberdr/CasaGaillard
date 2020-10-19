using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(MaterialMetaData))]
    public partial class Material
    {
    }
    public class MaterialMetaData
    {
        [Display(Name = "Material")]
        public string Material1 { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}