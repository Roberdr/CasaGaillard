using Microsoft.AspNetCore.Http;

namespace CasaGaillard.Core.Services;

public static class TareaFotoValidator
{
    private static readonly HashSet<string> ExtensionesPermitidas=new(StringComparer.OrdinalIgnoreCase){".jpg",".jpeg",".png",".gif",".webp",".bmp"};
    public static bool TipoPermitido(IFormFile file) => file.ContentType.StartsWith("image/",StringComparison.OrdinalIgnoreCase)&&ExtensionesPermitidas.Contains(Path.GetExtension(file.FileName));
    public static bool TamanoPermitido(IFormFile file) => file.Length<=10L*1024*1024;
}
