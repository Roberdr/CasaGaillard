using System.Collections.Generic;

namespace CasaGaillard.Models.ViewModels
{
    public class PaginaInicioViewModel
    {
        public int TotalProductos { get; set; }
        public int TotalCompartimentos { get; set; }
        public int TotalServicios { get; set; }
        public IList<ProductoStockPublicoViewModel> ProductosDestacados { get; set; }
        public IList<ServicioPublicoViewModel> Servicios { get; set; }
        public IList<SeguridadPublicaViewModel> RecomendacionesSeguridad { get; set; }
    }

    public class PaginaStockViewModel
    {
        public IList<ProductoStockPublicoViewModel> Productos { get; set; }
    }

    public class ProductoStockPublicoViewModel
    {
        public string Nombre { get; set; }
        public string Iupac { get; set; }
        public string Formula { get; set; }
        public string NumeroUN { get; set; }
        public int? Grupo { get; set; }
        public int TotalCompartimentos { get; set; }
        public int? CapacidadTotal { get; set; }
        public string MaterialPredominante { get; set; }
    }

    public class ServicioPublicoViewModel
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Detalle { get; set; }
    }

    public class SeguridadPublicaViewModel
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Nivel { get; set; }
    }
}
