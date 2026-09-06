using System;
using System.Collections.Generic;
using System.Linq;

namespace CasaGaillard.Models.ViewModels
{
    public class AccesoriosIndexViewModel
    {
        public IEnumerable<CasaGaillard.Models.Accesorio> Accesorios { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> TiposAccesorio { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Materiales { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Familias { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Subfamilias { get; set; }

        public int? TipoAccesorioId { get; set; }
        public int? MaterialId { get; set; }
        public int? FamiliaId { get; set; }
        public int? SubfamiliaId { get; set; }
        public string Query { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize <= 0 ? 1 : PageSize));
        // Map accesorio ID to a representative photo ID (if any)
        public System.Collections.Generic.Dictionary<int, int?> FotoIdByAccesorio { get; set; } = new System.Collections.Generic.Dictionary<int, int?>();
    }
}
