using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CasaGaillard.Models.ViewModels
{
    public class IntervencionFilaViewModel
    {
        public int Id { get; set; }
        public int TareaMantenimientoId { get; set; }
        public string TareaCodigo { get; set; }
        public string TareaTitulo { get; set; }
        public string Tecnico { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Descripcion { get; set; }
        public decimal? HorasEmpleadas { get; set; }
        public decimal? CosteManoObra { get; set; }
    }

    public class IntervencionFormViewModel
    {
        public int Id { get; set; }
        [Required]
        public int TareaMantenimientoId { get; set; }
        [StringLength(150)] public string Tecnico { get; set; }
        [Display(Name = "Técnico")]
        [Required(ErrorMessage = "Selecciona un técnico.")]
        public int? TecnicoPersonaID { get; set; }
        public IEnumerable<SelectListItem> Personas { get; set; }
        [Required, Display(Name = "Inicio"), DataType(DataType.DateTime)] public DateTime FechaInicio { get; set; }
        [Display(Name = "Fin"), DataType(DataType.DateTime)] public DateTime? FechaFin { get; set; }
        [Required, StringLength(2000), DataType(DataType.MultilineText)] public string Descripcion { get; set; }
        [Range(typeof(decimal), "0", "99999999")] public decimal? HorasEmpleadas { get; set; }
        [Range(typeof(decimal), "0", "9999999999")] public decimal? CosteManoObra { get; set; }
        public string TareaCodigo { get; set; }
        public string TareaTitulo { get; set; }
    }

    public class IntervencionRepuestoViewModel
    {
        public int Id { get; set; }
        public int IntervencionId { get; set; }
        public int AccesorioId { get; set; }
        public string AccesorioNombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? CosteUnitario { get; set; }
        public string Observaciones { get; set; }
    }

    public class IntervencionRepuestoFormViewModel
    {
        [Required] public int IntervencionId { get; set; }
        [Required, Display(Name = "Repuesto")] public int AccesorioId { get; set; }
        [Range(typeof(decimal), "0,001", "999999999")] public decimal Cantidad { get; set; } = 1;
        [Range(typeof(decimal), "0", "9999999999"), Display(Name = "Coste unitario")] public decimal? CosteUnitario { get; set; }
        [StringLength(500)] public string Observaciones { get; set; }
        public IEnumerable<SelectListItem> Accesorios { get; set; }
    }
}
