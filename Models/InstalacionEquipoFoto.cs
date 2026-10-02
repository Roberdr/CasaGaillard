using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaGaillard.Models
{
    [Table("InstalacionEquipoFotos")]
    public class InstalacionEquipoFoto
    {
        public int ID { get; set; }

        [Required, StringLength(20)]
        public string TipoActivo { get; set; }

        public int ActivoID { get; set; }

        [Required, StringLength(260)]
        public string RutaArchivo { get; set; }

        [StringLength(255)]
        public string NombreOriginal { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
