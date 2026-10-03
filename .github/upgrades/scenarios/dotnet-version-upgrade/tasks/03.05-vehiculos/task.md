# 03.05-vehiculos: Migrar VehiculosController

# 03.05-vehiculos

## Objective
Migrar el controlador VehiculosController del área Mantenimiento desde el proyecto .NET Framework al proyecto CasaGaillard.Core (net10.0).

## Scope
- Migrar acciones GET/POST, vistas asociadas y validaciones.
- Extraer lógica de acceso a datos a un servicio ICrudVehiculosService e implementarlo en CasaGaillard.Core.
- Registrar el servicio en DI y actualizar rutas/areas.

## Steps
1. Revisar Areas/Mantenimiento/Controllers/VehiculosController.cs en el repo legacy y listar vistas y modelos usados.
2. Crear CasaGaillard.Core/Areas/Mantenimiento/Controllers/VehiculosController.cs con acciones equivalentes, adaptando a Controller base de ASP.NET Core.
3. Añadir ICrudVehiculosService en CasaGaillard.Core/Services y una implementación VehiculosService que use AppDbContext (EF Core).
4. Registrar el servicio en Program.cs (builder.Services.AddScoped<ICrudVehiculosService, VehiculosService>); actualizar la inyección en el controlador.
5. Migrar vistas necesarias a CasaGaillard.Core/Areas/Mantenimiento/Views/Vehiculos/, actualizar _ViewImports si necesario.
6. Compilar, ejecutar smoke test (Index/Details/Create/Edit/Delete) y corregir errores.
7. Documentar cambios en tasks/03-mantenimiento/tasks/03.05-vehiculos/progress-details.md y commit.
