using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(SeguroMetaData))]
    public partial class Seguro
    {
    }
    public class SeguroMetaData
    {
        [Display(Name = "Compañía")]
        public string Compania { get; set; }

        [Display(Name = "Núm. Póliza")]
        public string NumPoliza { get; set; }

        [Display(Name = "Fecha Inicio")]
        public Nullable<System.DateTime> FechaInicio { get; set; }

        [Display(Name = "Meses Cobertura")]
        public Nullable<int> PeriodoCubiertoMeses { get; set; }

        [Display(Name = "Fecha Vencimiento")]
        public Nullable<System.DateTime> FechaVto { get; set; }
    }
}