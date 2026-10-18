using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaGaillard.Core.Areas.Mantenimiento.Controllers;

[Area("Mantenimiento"),Authorize(Roles=AppRoles.LecturaMantenimiento)]
public sealed class CostesMantenimientoController:Controller
{
    private readonly ICostesMantenimientoService _service;
    public CostesMantenimientoController(ICostesMantenimientoService service)=>_service=service;
    public async Task<IActionResult> Index(DateTime? fechaDesde,DateTime? fechaHasta,int? instalacionId,int? equipoId)
    {
        if(fechaDesde.HasValue&&fechaHasta.HasValue&&fechaHasta.Value.Date<fechaDesde.Value.Date)ModelState.AddModelError(nameof(fechaHasta),"La fecha hasta no puede ser anterior a la fecha desde.");
        var model=await _service.GetAsync(fechaDesde,fechaHasta,instalacionId,equipoId);
        if(!ModelState.IsValid){model.Resumen=[];model.Intervenciones=[];}
        return View(model);
    }
}
