using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaGaillard.Models
{
    [Table("TareaMantenimientoAccesorios")]
    public class TareaMantenimientoAccesorio
    {
        public int ID { get; set; }

        [Required]
        public int TareaMantenimientoID { get; set; }

        [Required]
        public int AccesorioID { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        [ForeignKey("TareaMantenimientoID")]
        public virtual TareaMantenimiento TareaMantenimiento { get; set; }
    }
}
