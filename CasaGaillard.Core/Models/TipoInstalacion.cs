using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models;

public class TipoInstalacion
{
    public int Id { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Instalacion> Instalaciones { get; set; } = new List<Instalacion>();
}
