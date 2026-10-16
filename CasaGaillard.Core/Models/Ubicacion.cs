using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public class Ubicacion
{
    public int UbicacionId { get; set; }

    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    public ICollection<Instalacion> Instalaciones { get; set; } = new List<Instalacion>();
}
