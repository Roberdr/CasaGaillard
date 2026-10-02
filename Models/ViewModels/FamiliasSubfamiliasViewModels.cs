using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models.ViewModels
{
    public class FamiliasSubfamiliasIndexViewModel
    {
        public IList<FamiliaFilaViewModel> Familias { get; set; } = new List<FamiliaFilaViewModel>();
        public IList<SubfamiliaFilaViewModel> Subfamilias { get; set; } = new List<SubfamiliaFilaViewModel>();
    }

    public class FamiliaFilaViewModel
    {
        public int FamiliaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int NumeroSubfamilias { get; set; }
        public int NumeroAccesorios { get; set; }
    }

    public class SubfamiliaFilaViewModel
    {
        public int SubfamiliaId { get; set; }
        public int FamiliaId { get; set; }
        public string FamiliaNombre { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int NumeroAccesorios { get; set; }
        public int NumeroTipos { get; set; }
    }

    public class FamiliaFormViewModel
    {
        public int FamiliaId { get; set; }
        [Required, StringLength(150)] public string Nombre { get; set; }
        [StringLength(500)] public string Descripcion { get; set; }
    }

    public class SubfamiliaFormViewModel
    {
        public int SubfamiliaId { get; set; }
        [Required, Display(Name = "Familia")] public int? FamiliaId { get; set; }
        [Required, StringLength(150)] public string Nombre { get; set; }
        [StringLength(500)] public string Descripcion { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Familias { get; set; }
    }
}
