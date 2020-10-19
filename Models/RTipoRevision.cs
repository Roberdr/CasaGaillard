using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TipoRevisionMetaData))]
    public partial class TipoRevision
    {
    }
    public class TipoRevisionMetaData
    {
        [Display(Name = "Revisión")]
        public string Revision { get; set; }
    }
}