using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CasaGaillard.Core.Areas.Pagina.Controllers;

[Area("Pagina")]
public sealed class PaginaController : Controller
{
    private readonly IPaginaPublicaService _service;
    public PaginaController(IPaginaPublicaService service) => _service=service;
    public async Task<IActionResult> Index() => View(await _service.GetHomeAsync());
    public async Task<IActionResult> Tienda() => View(await _service.GetStockAsync());
    public IActionResult Servicios() => View(_service.GetServices());
    public IActionResult Seguridad() => View(_service.GetSafetyRecommendations());
    public IActionResult Contacto() => View();
    public IActionResult AcercaDe() => View();
    [HttpGet]
    public async Task<IActionResult> Imagen(string archivo)
    {
        var image=await _service.GetImageAsync(archivo);
        return image is null ? NotFound() : PhysicalFile(image.Value.Path,image.Value.ContentType);
    }
}
