using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Core.Models
{
    public class Instalacion
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public int? TipoInstalacionId { get; set; }
        public int? UbicacionId { get; set; }
        public int? ResponsablePersonaId { get; set; }
        public DateTime? FechaPuestaMarcha { get; set; }
        public DateTime? FechaBaja { get; set; }
        public byte Criticidad { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? CodigoPlano { get; set; }
        public bool CriticaProduccion { get; set; }
        public bool CriticaSeguridad { get; set; }
        public string? Observaciones { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public TipoInstalacion? TipoInstalacion { get; set; }
        public Ubicacion? Ubicacion { get; set; }
    }
}
