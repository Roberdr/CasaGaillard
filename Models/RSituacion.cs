using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(SituacionMetaData))]
    public partial class Situacion
    {
    }
    public class SituacionMetaData
    {
        [Display(Name = "Número Lado Cuba")]
        public Nullable<int> LadoCubaNumero { get; set; }

        [Display(Name = "Lado Cuba")]
        public string LadoCubaNombre { get; set; }

        [Display(Name = "Número Situación Lado")]
        public Nullable<int> SituacionLadoNumero { get; set; }

        [Display(Name = "Situación Lado Cuba ")]
        public string SituacionLadoLetra { get; set; }
    }
}