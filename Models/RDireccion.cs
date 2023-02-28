using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(DireccionMetaData))]
    public partial class Direccion
    {
    }
    public class DireccionMetaData
    {

        [Display(Name = "Dirección")]
        public string NombreVia { get; set; }

        [Display(Name = "Número")]
        public Nullable<int> Numero { get; set; }

        [Display(Name = "Código Postal")]
        public string CP { get; set; }

        [Display(Name = "País")]
        public string Pais { get; set; }

    }
}