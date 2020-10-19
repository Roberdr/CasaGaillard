using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(OperacíonMetaData))]
    public partial class Operacíon
    {
    }
    public class OperacíonMetaData
    {
        [Display(Name = "Fecha Operación")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FechaOperacion { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Duración")]
        public Nullable<System.TimeSpan> Duracion { get; set; }
    }
}