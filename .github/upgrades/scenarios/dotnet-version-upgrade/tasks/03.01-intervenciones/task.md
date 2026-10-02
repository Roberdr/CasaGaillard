# 03.01-intervenciones: Convertir Intervenciones (Área Mantenimiento)

## Objective
Convertir el código del controlador Areas/Mantenimiento/Controllers/IntervencionesController.cs y sus dependencias para que funcionen en .NET 10.

## Scope
- Proyecto: CasaGaillard (principal)
- Archivos iniciales: Areas/Mantenimiento/Controllers/IntervencionesController.cs, Views relacionadas en Areas/Mantenimiento/Views/Intervenciones, modelos usados por el controlador, acceso a datos (EntityFramework o repositorios).
- Paquetes a revisar: System.Web.* usos, Owin/Katana, System.Web.Mvc, bundling/minification, EntityFramework (EF6), paquetes NuGet que no soporten net10.0.

## Done when
- El controlador y sus vistas compilan en el proyecto migrado y las rutas funcionan según el comportamiento actual (al menos en pruebas locales).
- Todas las referencias y paquetes necesarios para el área Mantenimiento están reemplazadas por equivalentes compatibles o documentadas como pendientes con alternativas.
- El proyecto que contiene el controlador es convertido a SDK-style o adaptado para compilar con net10.0.
- Tests relevantes pasan o se marcan con nota en progress-details.md si requieren reescritura.

## Initial research steps
1. Revisar assessment.md para listar incompatibilidades específicas detectadas para este proyecto.
2. Abrir Areas/Mantenimiento/Controllers/IntervencionesController.cs y detectar APIs no compatibles (System.Web.HttpContext, HttpPostedFileBase, etc.).
3. Listar paquetes NuGet afectados y sus versiones actuales.
4. Identificar dependencias transversales (servicios, repositorios, filtros) que requieren cambios.

## Affected files (initial)
- Areas/Mantenimiento/Controllers/IntervencionesController.cs
- Areas/Mantenimiento/Views/**
- Web.config (si aplica)
- Global.asax.cs (posibles migraciones relacionadas)

## Notes
- Esta subtarea es de tipo "execute": implicará cambios de proyecto, código y paquetes.

## Research findings (initial)
- Archivo analizado: Areas/Mantenimiento/Controllers/IntervencionesController.cs
- Namespaces y tipos detectados:
  - `using System.Web.Mvc` → requiere migrar a `Microsoft.AspNetCore.Mvc` y revisar tipos como `Controller`, `ActionResult`, `HttpNotFound()`, `HttpStatusCodeResult`.
  - Uso de métodos de helper WebAPI/MVC antiguos: `HttpNotFound()` y `new HttpStatusCodeResult(400)` deberán reemplazarse por `NotFound()`, `BadRequest()` o `StatusCode(400)`.
- Acceso a datos:
  - Uso de Entity Framework 6 APIs (`Database.SqlQuery`, `ExecuteSqlCommandAsync`) y contextos instanciados directamente (`new TareasMantenimientoContext()`, `new GaillardEntities()`). Recomendado: migrar a EF Core o evaluar compatibilidad de EF6 en .NET 10; además inyectar DbContext mediante DI en lugar de instanciación directa.
- Dependencias en Views y helpers:
  - Uso de `ViewBag`, `TempData`, `SelectListItem` y vistas Razor. Estas son compatibles, pero hay que adaptar llamadas a `@Scripts.Render` / bundling si existen en vistas asociadas.
- Autenticación/Autorización:
  - Uso de `[Authorize(Roles = ...)]` permanece compatible pero revisar cómo se registran roles/claims en la nueva autenticación.
- No se detectaron usos obvios de `HttpContext.Current` ni módulos HTTP en este archivo (más inspección puede ser necesaria en otras partes del proyecto).

## Initial next actions
1. Convertir el proyecto que contiene el controlador a SDK-style (usar la herramienta de conversión). Guardar evidencia de conversión.
2. Actualizar referencias NuGet: reemplazar `System.Web.Mvc` por `Microsoft.AspNetCore.Mvc` (o añadir paquete Meta correspondiente) y revisar EF (migrar a EF Core o plan para mantener EF6).
3. Reescribir el controlador:
   - Cambiar `using` y tipos base si procede.
   - Reemplazar `HttpNotFound()` / `HttpStatusCodeResult` por helpers `NotFound()` / `BadRequest()`.
   - Inyectar DbContexts y adaptar llamadas SQL a EF Core (`FromSqlRaw`, `ExecuteSqlRawAsync`) o usar repositorio.
4. Ejecutar build focalizado del proyecto y corregir errores; iterar hasta compilar.
5. Adaptar vistas relacionadas según sea necesario (quitar `@Scripts.Render` y revisar helpers).

Recorded at: automatic task start
