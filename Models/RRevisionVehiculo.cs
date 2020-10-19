using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(RevisionVehiculoMetaData))]
    public partial class RevisionVehiculo
    {
    }
    public class RevisionVehiculoMetaData
    {
        [Display(Name = "Fecha Revisión")]
        public Nullable<System.DateTime> FechaRevision { get; set; }
    }
}