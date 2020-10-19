using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(PersonaMetaData))]
    public partial class Persona
    {
    }
    public class PersonaMetaData
    {
        [Display(Name = "Nombre")]
        public string NombrePersona { get; set; }

        [Display(Name = "1er Apellido")]
        public string Apellido1 { get; set; }

        [Display(Name = "2º Apellido")]
        public string Apellido2 { get; set; }
    }
}