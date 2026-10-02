using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CasaGaillard.Models.ViewModels
{
    public class CostesMantenimientoViewModel
    {
        [Display(Name = "Desde"), DataType(DataType.Date)] public DateTime? FechaDesde { get; set; }
        [Display(Name = "Hasta"), DataType(DataType.Date)] public DateTime? FechaHasta { get; set; }
        [Display(Name = "Instalación")] public int? InstalacionId { get; set; }
        [Display(Name = "Equipo")] public int? EquipoId { get; set; }
        public IEnumerable<SelectListItem> Instalaciones { get; set; }
        public IEnumerable<SelectListItem> Equipos { get; set; }
        public IList<CosteActivoResumenViewModel> Resumen { get; set; } = new List<CosteActivoResumenViewModel>();
        public IList<CosteIntervencionViewModel> Intervenciones { get; set; } = new List<CosteIntervencionViewModel>();
    }

    public class CosteActivoResumenViewModel
    {
        public int? InstalacionId { get; set; }
        public string InstalacionCodigo { get; set; }
        public string InstalacionNombre { get; set; }
        public int? EquipoId { get; set; }
        public string EquipoCodigo { get; set; }
        public string EquipoNombre { get; set; }
        public int NumeroIntervenciones { get; set; }
        public decimal HorasEmpleadas { get; set; }
        public decimal CosteManoObra { get; set; }
        public decimal CosteRepuestos { get; set; }
        public int RepuestosSinCoste { get; set; }
        public decimal CosteTotal { get; set; }
    }

    public class CosteIntervencionViewModel
    {
        public int Id { get; set; }
        public int TareaMantenimientoId { get; set; }
        public string TareaCodigo { get; set; }
        public string TareaTitulo { get; set; }
        public string InstalacionNombre { get; set; }
        public string EquipoCodigo { get; set; }
        public string EquipoNombre { get; set; }
        public string Tecnico { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Descripcion { get; set; }
        public decimal? HorasEmpleadas { get; set; }
        public decimal CosteManoObra { get; set; }
        public decimal CosteRepuestos { get; set; }
        public int RepuestosSinCoste { get; set; }
        public decimal CosteTotal { get; set; }
    }
}
