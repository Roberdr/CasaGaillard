using System.ComponentModel.DataAnnotations;

namespace CasaGaillard.Models
{
    [MetadataType(typeof(TipoGrupoMetaData))]
    public partial class TipoGrupo
    {
    }
    public class TipoGrupoMetaData
    {
        [Display(Name = "Nombre Grupo")]
        public string NombreGrupo { get; set; }
    }
}