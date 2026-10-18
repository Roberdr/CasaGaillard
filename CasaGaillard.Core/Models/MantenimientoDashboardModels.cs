namespace CasaGaillard.Core.Models;

public sealed class MantenimientoDashboardViewModel
{
    public IReadOnlyList<DashboardRevisionCubaViewModel> RevisionesCubas { get; set; } = [];
    public IReadOnlyList<DashboardRevisionVehiculoViewModel> RevisionesVehiculos { get; set; } = [];
    public IReadOnlyList<TareaMantenimientoResumenViewModel> Tareas { get; set; } = [];
}

public sealed class DashboardRevisionCubaViewModel
{
    public int CubaId { get; set; }
    public string MatriculaCuba { get; set; } = "";
    public DateTime? ValidaHasta { get; set; }
    public string? DescripcionProxima { get; set; }
}

public sealed class DashboardRevisionVehiculoViewModel
{
    public string MatriculaVehiculo { get; set; } = "";
    public string TipoRevision { get; set; } = "";
    public DateTime? Caducidad { get; set; }
}
