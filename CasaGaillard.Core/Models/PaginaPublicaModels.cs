namespace CasaGaillard.Core.Models;

public sealed class PaginaInicioViewModel
{
    public int TotalProductos { get; set; }
    public int TotalCompartimentos { get; set; }
    public int TotalServicios { get; set; }
    public IReadOnlyList<ProductoStockPublicoViewModel> ProductosDestacados { get; set; } = [];
    public IReadOnlyList<ServicioPublicoViewModel> Servicios { get; set; } = [];
    public IReadOnlyList<SeguridadPublicaViewModel> RecomendacionesSeguridad { get; set; } = [];
    public bool CatalogoNoDisponible { get; set; }
}

public sealed class PaginaStockViewModel
{
    public IReadOnlyList<ProductoStockPublicoViewModel> Productos { get; set; } = [];
    public bool CatalogoNoDisponible { get; set; }
}

public sealed class ProductoStockPublicoViewModel
{
    public string Nombre { get; set; } = "";
    public string? Iupac { get; set; }
    public string? Formula { get; set; }
    public string? NumeroUN { get; set; }
    public int? Grupo { get; set; }
    public int TotalCompartimentos { get; set; }
    public int? CapacidadTotal { get; set; }
    public string? MaterialPredominante { get; set; }
}

public sealed class ServicioPublicoViewModel { public string Titulo { get; set; } = ""; public string Descripcion { get; set; } = ""; public string Detalle { get; set; } = ""; }
public sealed class SeguridadPublicaViewModel { public string Titulo { get; set; } = ""; public string Descripcion { get; set; } = ""; public string Nivel { get; set; } = ""; }
