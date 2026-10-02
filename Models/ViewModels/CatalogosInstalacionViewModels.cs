using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models.ViewModels
{
    public class TipoInstalacionCatalogoViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Nombre { get; set; }
        [StringLength(500)] public string Descripcion { get; set; }
        public int InstalacionesAsociadas { get; set; }
    }

    public class UbicacionCatalogoViewModel
    {
        public int UbicacionId { get; set; }
        [StringLength(50)] public string Codigo { get; set; }
        [Required, StringLength(150)] public string Nombre { get; set; }
        [StringLength(300)] public string Descripcion { get; set; }
        public int InstalacionesAsociadas { get; set; }
        public int AccesoriosAsociados { get; set; }
    }

    public class CatalogoInstalacionIndexViewModel<T>
    {
        public IList<T> Elementos { get; set; } = new List<T>();
    }
}
