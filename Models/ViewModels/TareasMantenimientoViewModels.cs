using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CasaGaillard.Models.ViewModels
{
    public class TareaMantenimientoResumenViewModel
    {
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Ubicacion { get; set; }
        public string Equipo { get; set; }
        public string Prioridad { get; set; }
        public string Estado { get; set; }
        public string AsignadaA { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class TareaMantenimientoFormViewModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Asunto")]
        public string Titulo { get; set; }

        [Required]
        [Display(Name = "Descripción de la avería")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Display(Name = "Equipo / instalación")]
        public string Equipo { get; set; }

        [Display(Name = "Detectado por")]
        public string DetectadoPor { get; set; }

        [Display(Name = "Contacto")]
        public string Contacto { get; set; }

        [Display(Name = "Prioridad")]
        public string Prioridad { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Asignada a")]
        public string AsignadaA { get; set; }

        [Display(Name = "Empresa exterior")]
        public string EmpresaExterior { get; set; }

        [Display(Name = "Acciones a realizar")]
        [DataType(DataType.MultilineText)]
        public string AccionesARealizar { get; set; }

        [Display(Name = "Accesorios necesarios")]
        public string[] AccesoriosSeleccionados { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }

        [Display(Name = "Fotos de la avería")]
        public IEnumerable<HttpPostedFileBase> FotosAdjuntas { get; set; }

        public IEnumerable<SelectListItem> Prioridades { get; set; }
        public IEnumerable<SelectListItem> Estados { get; set; }
        public IEnumerable<SelectListItem> Empleados { get; set; }
        public IEnumerable<SelectListItem> Accesorios { get; set; }

        public string Codigo { get; set; }
        public DateTime FechaDeteccion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string CreadoPor { get; set; }
        public string ActualizadoPor { get; set; }
    }

    public class TareaMantenimientoGestionViewModel
    {
        public int ID { get; set; }

        public string Codigo { get; set; }

        [Display(Name = "Asunto")]
        public string Titulo { get; set; }

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Display(Name = "Equipo / instalación")]
        public string Equipo { get; set; }

        [Display(Name = "Detectado por")]
        public string DetectadoPor { get; set; }

        [Display(Name = "Contacto")]
        public string Contacto { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Prioridad")]
        public string Prioridad { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Asignada a")]
        public string AsignadaA { get; set; }

        [Display(Name = "Empresa exterior")]
        public string EmpresaExterior { get; set; }

        [Display(Name = "Acciones a realizar")]
        [DataType(DataType.MultilineText)]
        public string AccionesARealizar { get; set; }

        [Display(Name = "Accesorios necesarios")]
        public string[] AccesoriosSeleccionados { get; set; }

        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string Observaciones { get; set; }

        [Display(Name = "Fotos de la avería")]
        public IEnumerable<HttpPostedFileBase> FotosAdjuntas { get; set; }

        public IEnumerable<SelectListItem> Prioridades { get; set; }
        public IEnumerable<SelectListItem> Estados { get; set; }
        public IEnumerable<SelectListItem> Empleados { get; set; }
        public IEnumerable<SelectListItem> Accesorios { get; set; }
    }
}
