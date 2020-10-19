using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(RevisionMetaData))]
    public partial class Revision
    {
    }
    public class RevisionMetaData
    {
        [Display(Name = "Fecha de Revisión")]
        public System.DateTime FechaRevision { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Válida Hasta")]
        public Nullable<System.DateTime> ValidaHasta { get; set; }

        [Display(Name = "Descripción Próxima")]
        public string DescripcionProxima { get; set; }
    }
}