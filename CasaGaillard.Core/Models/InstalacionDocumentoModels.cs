using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CasaGaillard.Core.Models;

public sealed class InstalacionOpcionViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public sealed class InstalacionDocumentoFilaViewModel
{
    public int Id { get; set; }
    public int InstalacionId { get; set; }
    public string InstalacionCodigo { get; set; } = string.Empty;
    public string InstalacionNombre { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? TipoDocumento { get; set; }
    public string? CodigoPlano { get; set; }
    public DateTime? FechaDocumento { get; set; }
    public DateTime FechaAlta { get; set; }
}

public sealed class InstalacionDocumentoFormViewModel
{
    public int InstalacionId { get; set; }
    public string InstalacionCodigo { get; set; } = string.Empty;
    public string InstalacionNombre { get; set; } = string.Empty;

    [StringLength(100), Display(Name = "Tipo de documento")]
    public string? TipoDocumento { get; set; }

    [StringLength(50), Display(Name = "Código de plano")]
    public string? CodigoPlano { get; set; }

    [Display(Name = "Fecha del documento"), DataType(DataType.Date)]
    public DateTime? FechaDocumento { get; set; }

    [Display(Name = "Archivo")]
    public IFormFile? Archivo { get; set; }

    public IReadOnlyList<InstalacionDocumentoFilaViewModel> Documentos { get; set; } = [];
}
