using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaGaillard.Models
{
    [Table("TareaMantenimientoFotos")]
    public class TareaMantenimientoFoto
    {
        public int ID { get; set; }

        [Required]
        public int TareaMantenimientoID { get; set; }

        [Required]
        [StringLength(260)]
        public string RutaArchivo { get; set; }

        [StringLength(255)]
        public string NombreOriginal { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public DateTime FechaCreacion { get; set; }

        [ForeignKey("TareaMantenimientoID")]
        public virtual TareaMantenimiento TareaMantenimiento { get; set; }
    }
}
