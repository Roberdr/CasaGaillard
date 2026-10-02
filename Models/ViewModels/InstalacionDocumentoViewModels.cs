using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CasaGaillard.Models.ViewModels
{
    public class InstalacionDocumentoFilaViewModel
    {
        public int Id { get; set; }
        public int InstalacionId { get; set; }
        public string InstalacionCodigo { get; set; }
        public string InstalacionNombre { get; set; }
        public string Nombre { get; set; }
        public string TipoDocumento { get; set; }
        public string CodigoPlano { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public DateTime FechaAlta { get; set; }
    }

    public class InstalacionDocumentoFormViewModel
    {
        [Required] public int InstalacionId { get; set; }
        [StringLength(100), Display(Name = "Tipo de documento")] public string TipoDocumento { get; set; }
        [StringLength(50), Display(Name = "Código de plano")] public string CodigoPlano { get; set; }
        [Display(Name = "Fecha del documento"), DataType(DataType.Date)] public DateTime? FechaDocumento { get; set; }
        [Display(Name = "Archivo")] public HttpPostedFileBase Archivo { get; set; }
        public string InstalacionCodigo { get; set; }
        public string InstalacionNombre { get; set; }
        public IEnumerable<InstalacionDocumentoFilaViewModel> Documentos { get; set; }
    }
}
