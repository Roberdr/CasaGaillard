namespace CasaGaillard.Core.Models;

public sealed class PlanificadorEstado
{
    public string Clave { get; set; } = string.Empty;
    public string Json { get; set; } = string.Empty;
    public DateTime ActualizadoUtc { get; set; }
    public string? ActualizadoPor { get; set; }
}
