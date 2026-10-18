using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"), Authorize(Roles = AppRoles.LecturaMantenimiento)]
public sealed class HomeController : Controller
{
    private readonly IMantenimientoDashboardService _service;
    public HomeController(IMantenimientoDashboardService service) => _service = service;
    public async Task<IActionResult> Index() => View(await _service.GetAsync());
}
