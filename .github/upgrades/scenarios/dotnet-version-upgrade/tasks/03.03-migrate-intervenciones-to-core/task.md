# 03.03-migrate-intervenciones-to-core: Migrar Intervenciones a CasaGaillard.Core

## Objective
Copiar y adaptar el controlador Areas/Mantenimiento/Controllers/IntervencionesController.cs a CasaGaillard.Core como controlador MVC/Pages compatible con ASP.NET Core.

## Scope
- Crear controlador esqueleto en CasaGaillard.Core con las acciones Index, Create, Edit, Details, AddRepuesto, RemoveRepuesto
- Mantener firmas asincrónicas: Task<IActionResult> o ActionResult<T>
- Sustituir retornos legacy (HttpNotFound, HttpStatusCodeResult) por NotFound/BadRequest/StatusCode
- No migrar acceso a datos completo en esta iteración: dejar TODOs para inyección de DbContext y llamadas SQL; usar stubs que compilen

## Done when
- Nuevo controlador compilable en CasaGaillard.Core
- `dotnet build CasaGaillard.Core` pasa sin errores
- task.md y progress-details.md creados y commitados

## Initial research
- IntervencionesController original usa System.Web.Mvc y EF6 calls (Database.SqlQuery, ExecuteSqlCommandAsync).
- Plan: implement controller skeleton that returns minimal content while preserving action names; later tasks will implement data access.

