using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CasaGaillard.Models.ViewModels
{
    public class AccesorioFotoViewModel
    {
        public int ID { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class DetalleAccesorioFormViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Característica")]
        public int? CaracteristicaAccesorioID { get; set; }
        public string CaracteristicaNombre { get; set; }

        [Display(Name = "Cantidad")]
        public int? Cantidad { get; set; }

        [Display(Name = "Medida")]
        public decimal? Medida { get; set; }

        [Display(Name = "Unidad")]
        public int? UnidadID { get; set; }
        public string UnidadNombre { get; set; }

        [Display(Name = "Tipo")]
        public string Tipo { get; set; }

        public bool Eliminar { get; set; }
    }

    public class AccesorioFormViewModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Tipo de accesorio")]
        public int? TipoAccesorioID { get; set; }

        [Required]
        [Display(Name = "Material")]
        public int? MaterialID { get; set; }

        [Display(Name = "Detalles")]
        public List<DetalleAccesorioFormViewModel> Detalles { get; set; } = new List<DetalleAccesorioFormViewModel>();

        [Display(Name = "Fotos del accesorio")]
        public IEnumerable<HttpPostedFileBase> FotosAdjuntas { get; set; }

        public IEnumerable<SelectListItem> TiposAccesorio { get; set; }
        public IEnumerable<SelectListItem> Materiales { get; set; }
        public IEnumerable<SelectListItem> Caracteristicas { get; set; }
        public IEnumerable<SelectListItem> Unidades { get; set; }
        public IEnumerable<AccesorioFotoViewModel> FotosExistentes { get; set; }
    }
}
