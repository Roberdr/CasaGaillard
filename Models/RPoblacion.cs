using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(PoblacionMetaData))]
    public partial class Poblacion
    {
    }
    public class PoblacionMetaData
    {
        [Display(Name = "Población")]
        public string NombrePoblacion { get; set; }

        [Display(Name = "Autonomía")]
        public string Autonomia { get; set; }

        [Display(Name = "País")]
        public string Pais { get; set; }
    }
}