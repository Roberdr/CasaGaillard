# Plan de migración a .NET 10 — CasaGaillard

## Understanding
El objetivo es migrar la solución CasaGaillard (actualmente .NET Framework 4.8) a .NET moderno (target: net10.0). El usuario solicitó comenzar por el área "Mantenimiento" y, en concreto, por el controlador Areas/Mantenimiento/Controllers/IntervencionesController.cs. La solución está en la rama de trabajo `upgrade-dotnet-10` basada en `Tienda`.

## Assumptions
- La evaluación inicial (assessment.md) ya detectó incompatibilidades comunes (System.Web, bundling, Global.asax, binding redirects, paquetes NuGet no compatibles, proyectos no SDK-style).
- Flow Mode: Automatic — proceder sin pausas salvo bloqueos.
- Commit strategy: After Each Task.
- El plan generará una tarea padre para el grupo "Mantenimiento" y al menos una subtarea específica para "Intervenciones".

## Approach
1. Generar artefacto plan.md
2. Registrar tareas y subtareas relevantes (Breakdown)
3. Iniciar la subtarea de Intervenciones
4. Investigación y enriquecimiento de task.md
5. Aplicar cambios de código y proyecto en el área Mantenimiento
6. Validación: compilar, corregir warnings, ejecutar tests
7. Documentar progress-details.md y completar la tarea
8. Commit (After Each Task) y sincronización de rama

## Key Files
- .github/upgrades/scenarios/dotnet-version-upgrade/assessment.md
- .github/upgrades/scenarios/dotnet-version-upgrade/plan.md
- Areas/Mantenimiento/Controllers/IntervencionesController.cs
- CasaGaillard.sln

## Steps
1. step-1: Crear plan.md — escribir plan.md en la carpeta del escenario con el contenido del plan.
2. step-2: Registrar tareas — crear tarea padre `03-mantenimiento` y subtarea `03.01-intervenciones` mediante break_down_task.
3. step-3: Iniciar tarea `03.01-intervenciones` — llamar start_task para crear la carpeta y task.md.
4. step-4: Investigación y enriquecimiento — listar proyectos/archivos afectados y documentar en tasks/03.01-intervenciones/task.md.
5. step-5: Ejecutar cambios en área Mantenimiento — actualizar .csproj, paquetes, y adaptar código.
6. step-6: Validación — compilar solución y ejecutar tests.
7. step-7: Documentar progreso y complete_task.
8. step-8: Commit y branch-sync.
