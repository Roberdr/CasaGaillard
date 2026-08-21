using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaGaillard.Models
{
    [Table("AccesorioFotos")]
    public class AccesorioFoto
    {
        public int ID { get; set; }

        [Required]
        public int AccesorioID { get; set; }

        [Required]
        [StringLength(260)]
        public string RutaArchivo { get; set; }

        [StringLength(255)]
        public string NombreOriginal { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
