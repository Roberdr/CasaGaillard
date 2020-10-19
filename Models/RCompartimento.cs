using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(CompartimentoMetaData))]
    public partial class Compartimento
    {
    }
    public class CompartimentoMetaData
    {
        [Display(Name = "Número")]
        public Nullable<int> Numero { get; set; }
    }
}