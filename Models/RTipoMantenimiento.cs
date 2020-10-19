using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TipoMantenimientoMetaData))]
    public partial class TipoMantenimiento
    {
    }
    public class TipoMantenimientoMetaData
    {
        [Display(Name = "Operación")]
        public string Operacion { get; set; }
    }
}