using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(PersonasEntidadMetaData))]
    public partial class PersonasEntidad
    {
    }
    public class PersonasEntidadMetaData
    {
        [Display(Name = "Fecha de Alta")]
        public Nullable<System.DateTime> FechaAlta { get; set; }

        [Display(Name = "Fecha de Baja")]
        public Nullable<System.DateTime> FechaBaja { get; set; }
    }
}