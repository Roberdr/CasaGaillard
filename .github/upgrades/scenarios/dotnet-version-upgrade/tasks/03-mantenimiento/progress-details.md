# Progress details: 03-mantenimiento

Resumen de trabajo realizado para la tarea 03-mantenimiento (subtarea 03.01-intervenciones completada):

- Migración side-by-side del área Mantenimiento → Intervenciones al proyecto CasaGaillard.Core (net10.0).
- Vistas migradas/añadidas: Index, Details, Create, Edit en CasaGaillard.Core/Areas/Mantenimiento/Views/Intervenciones.
- Añadidos archivos _ViewImports.cshtml en Views y en el área para habilitar Tag Helpers y using de modelos.
- Program.cs actualizado: habilitados ControllersWithViews, rutas de áreas, static files y preservado MapRazorPages.
- AppDbContext y servicio IntervencionesService implementados y registrados en DI.
- EF Core migration scaffold (InitialCreate) y script idempotente generados; dotnet ef database update aplicado en entorno de desarrollo; tabla Intervenciones creada.
- Pruebas locales realizadas:
  - Inicio de la app CasaGaillard.Core en http://localhost:5200
  - Creación de intervención verificada mediante script de prueba y comprobación de Index/Details
  - Edición de intervención verificada mediante script de prueba (test_edit_intervencion.ps1)
- Scripts de ayuda añadidos y preservados: scripts/test_edit_intervencion.ps1, scripts/stop_local_server.ps1

Archivos modificados/añadidos (resumen):
- CasaGaillard.Core/Areas/Mantenimiento/Views/Intervenciones/Index.cshtml (modificado)
- CasaGaillard.Core/Areas/Mantenimiento/Views/Intervenciones/Create.cshtml (añadido)
- CasaGaillard.Core/Areas/Mantenimiento/Views/Intervenciones/Details.cshtml (añadido)
- CasaGaillard.Core/Areas/Mantenimiento/Views/Intervenciones/Edit.cshtml (añadido)
- CasaGaillard.Core/Views/_ViewImports.cshtml (añadido)
- CasaGaillard.Core/Areas/Mantenimiento/Views/_ViewImports.cshtml (añadido)
- CasaGaillard.Core/Program.cs (modificado)
- CasaGaillard.Core/appsettings.Development.json (modificado)
- CasaGaillard.Core/EFCoreMigrations/* (InitialCreate migration scaffolded)
- EFCoreMigrations/InitialCreate.sql (idempotent script)
- scripts/test_edit_intervencion.ps1 (añadido)
- scripts/stop_local_server.ps1 (añadido)
- PULL_REQUEST_BODY.txt (añadido)

Resultados de build:
- dotnet build CasaGaillard.Core: OK (sin errores)

Observaciones / próximos pasos:
- Implementar AddRepuesto/RemoveRepuesto y migrar entidades relacionadas si se necesita la funcionalidad completa.
- Revisar y remover scripts de prueba antes del merge si se desea mantener el repo limpio.
- Preparar PR y solicitar revisiones del equipo backend.

Fecha: 2026-10-02
