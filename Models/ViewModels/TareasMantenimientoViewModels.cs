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
        public int? InstalacionId { get; set; }
        public int? EquipoId { get; set; }
        public int? CubaId { get; set; }
        public int? GrupoId { get; set; }
        public int? VehiculoId { get; set; }
        public int? PlanMantenimientoId { get; set; }
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

        [Display(Name = "Instalación")]
        public int? InstalacionId { get; set; }

        [Display(Name = "Equipo")]
        public int? EquipoId { get; set; }

        [Display(Name = "Cisterna")]
        public int? CubaId { get; set; }

        [Display(Name = "Grupo de cisterna")]
        public int? GrupoId { get; set; }

        [Display(Name = "Vehículo")]
        public int? VehiculoId { get; set; }

        [Display(Name = "Plan de mantenimiento")]
        public int? PlanMantenimientoId { get; set; }

        [Display(Name = "Detectado por")]
        public string DetectadoPor { get; set; }
        public int? DetectadoPorPersonaID { get; set; }

        [Display(Name = "Contacto")]
        public string Contacto { get; set; }
        public int? ContactoPersonaID { get; set; }

        [Display(Name = "Prioridad")]
        public string Prioridad { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Asignada a")]
        public string AsignadaA { get; set; }
        public int? AsignadaAPersonaID { get; set; }

        [Display(Name = "Empresa exterior")]
        public string EmpresaExterior { get; set; }
        public int? EmpresaExteriorEntidadID { get; set; }

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
        public IEnumerable<SelectListItem> PersonasDetectadoPor { get; set; }
        public IEnumerable<SelectListItem> PersonasContacto { get; set; }
        public IEnumerable<SelectListItem> PersonasAsignadaA { get; set; }
        public IEnumerable<SelectListItem> Entidades { get; set; }
        public IEnumerable<SelectListItem> Accesorios { get; set; }
        public IEnumerable<SelectListItem> Instalaciones { get; set; }
        public IEnumerable<SelectListItem> Equipos { get; set; }
        public IEnumerable<SelectListItem> Cubas { get; set; }
        public IEnumerable<SelectListItem> Grupos { get; set; }
        public IEnumerable<SelectListItem> Vehiculos { get; set; }
        public IEnumerable<SelectListItem> PlanesMantenimiento { get; set; }

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

        [Display(Name = "Instalación")]
        public int? InstalacionId { get; set; }

        [Display(Name = "Equipo")]
        public int? EquipoId { get; set; }

        [Display(Name = "Cisterna")]
        public int? CubaId { get; set; }

        [Display(Name = "Grupo de cisterna")]
        public int? GrupoId { get; set; }

        [Display(Name = "Vehículo")]
        public int? VehiculoId { get; set; }

        [Display(Name = "Plan de mantenimiento")]
        public int? PlanMantenimientoId { get; set; }

        [Display(Name = "Detectado por")]
        public string DetectadoPor { get; set; }
        public int? DetectadoPorPersonaID { get; set; }

        [Display(Name = "Contacto")]
        public string Contacto { get; set; }
        public int? ContactoPersonaID { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Prioridad")]
        public string Prioridad { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Asignada a")]
        public string AsignadaA { get; set; }
        public int? AsignadaAPersonaID { get; set; }

        [Display(Name = "Empresa exterior")]
        public string EmpresaExterior { get; set; }
        public int? EmpresaExteriorEntidadID { get; set; }

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
        public IEnumerable<SelectListItem> PersonasDetectadoPor { get; set; }
        public IEnumerable<SelectListItem> PersonasContacto { get; set; }
        public IEnumerable<SelectListItem> PersonasAsignadaA { get; set; }
        public IEnumerable<SelectListItem> Entidades { get; set; }
        public IEnumerable<SelectListItem> Accesorios { get; set; }
        public IEnumerable<SelectListItem> Instalaciones { get; set; }
        public IEnumerable<SelectListItem> Equipos { get; set; }
        public IEnumerable<SelectListItem> PlanesMantenimiento { get; set; }
    }
}
