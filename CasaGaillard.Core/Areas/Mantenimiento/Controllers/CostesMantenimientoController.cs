using CasaGaillard.Core.Models;
using CasaGaillard.Core.Security;
using CasaGaillard.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

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

    public async Task<IActionResult> Exportar(DateTime? fechaDesde,DateTime? fechaHasta,int? instalacionId,int? equipoId)
    {
        if(fechaDesde.HasValue&&fechaHasta.HasValue&&fechaHasta.Value.Date<fechaDesde.Value.Date) return BadRequest("El rango de fechas no es válido.");
        var model=await _service.GetAsync(fechaDesde,fechaHasta,instalacionId,equipoId);
        var sb=new StringBuilder();
        sb.AppendLine("Fecha;Código tarea;Tarea;Instalación;Equipo;Técnico;Horas;Mano de obra;Repuestos;Total");
        foreach(var item in model.Intervenciones)
            sb.AppendLine(string.Join(';',new[]{item.FechaInicio.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture),CsvReportFormatter.Field(item.TareaCodigo),CsvReportFormatter.Field(item.TareaTitulo),CsvReportFormatter.Field(item.InstalacionNombre),CsvReportFormatter.Field(item.EquipoNombre),CsvReportFormatter.Field(item.Tecnico),item.HorasEmpleadas?.ToString(CultureInfo.InvariantCulture)??"",item.CosteManoObra.ToString(CultureInfo.InvariantCulture),item.CosteRepuestos.ToString(CultureInfo.InvariantCulture),item.CosteTotal.ToString(CultureInfo.InvariantCulture)}));
        var bytes=Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes,"text/csv; charset=utf-8",$"costes-mantenimiento-{DateTime.Today:yyyyMMdd}.csv");
    }

}
