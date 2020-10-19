using System;
using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(PlataformaMetaData))]
    public partial class Plataforma
    {
    }
    public class PlataformaMetaData
    {
        [Display(Name = "Plataforma")]
        public string MatriculaPlataforma { get; set; }

        [Display(Name = "P.M.A.")]
        public Nullable<int> Pma { get; set; }
    }
}