# 03.04-register-di-stubs: Registrar DI y crear stubs de acceso a datos en CasaGaillard.Core

## Objective
Registrar servicios en Program.cs e implementar stubs de acceso a datos para Intervenciones (IIntervencionesService + IntervencionesService) con modelos mínimos para compilar.

## Scope
- Anadir interfaz IIntervencionesService y su implementacion stub en CasaGaillard.Core/Services
- Anadir modelo IntervencionViewModel en CasaGaillard.Core/Models
- Registrar servicio en Program.cs con AddScoped<IIntervencionesService, IntervencionesService>()
- Actualizar IntervencionesController para inyectar el servicio y usar metodos stub

## Done when
- Proyecto CasaGaillard.Core compila sin errores
- task.md y progress-details.md creados y commitados

