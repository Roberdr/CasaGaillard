using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasaGaillard.Models
{
    [Table("TareasMantenimiento")]
    public class TareaMantenimiento
    {
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Asunto")]
        public string Titulo { get; set; }

        [Required]
        [StringLength(2000)]
        [Display(Name = "Descripción de la avería")]
        public string Descripcion { get; set; }

        [StringLength(150)]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [StringLength(150)]
        [Display(Name = "Equipo / instalación")]
        public string Equipo { get; set; }

        [StringLength(150)]
        [Display(Name = "Detectado por")]
        public string DetectadoPor { get; set; }

        [StringLength(150)]
        [Display(Name = "Contacto")]
        public string Contacto { get; set; }

        [StringLength(20)]
        [Display(Name = "Prioridad")]
        public string Prioridad { get; set; }

        [StringLength(30)]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [StringLength(150)]
        [Display(Name = "Asignada a")]
        public string AsignadaA { get; set; }

        [StringLength(150)]
        [Display(Name = "Empresa exterior")]
        public string EmpresaExterior { get; set; }

        [StringLength(2000)]
        [Display(Name = "Acciones a realizar")]
        public string AccionesARealizar { get; set; }

        [StringLength(4000)]
        [Display(Name = "Accesorios necesarios")]
        public string AccesoriosNecesarios { get; set; }

        [StringLength(2000)]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [StringLength(100)]
        public string CreadoPor { get; set; }

        [StringLength(100)]
        public string ActualizadoPor { get; set; }

        public DateTime FechaDeteccion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
