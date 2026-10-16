# Progress details for 03.05-vehiculos

- Migrated the `VehiculosController` CRUD actions to ASP.NET Core and registered `ICrudVehiculosService` / `VehiculosService` with EF Core.
- Migrated the Index, AddOrEdit, Details, and Delete views to Razor Tag Helpers.
- Added filtering by matrícula, marca, modelo, and tipo, plus eight-item pagination that preserves the active filters and the bajas toggle.
- AddOrEdit loads vehicle types and workshops from `AppDbContext` for both GET and invalid POST responses.
- Restricted the POST model binding to the fields accepted by the legacy controller and retained anti-forgery validation for write operations.
- Reviewed legacy authorization: read access used Administrador/Mantenimiento/Consulta and writes used Administrador/Mantenimiento. The Core project does not yet configure authentication, Identity, or roles, so these policies cannot be migrated safely in this task. They remain a cross-cutting migration prerequisite.
- Verified compilation successfully with `dotnet build CasaGaillard.Core/CasaGaillard.Core.csproj --no-restore -p:UseAppHost=false` using an isolated output directory.
- Attempted an HTTP smoke test for Index, AddOrEdit, Details, and Delete. The application started on a separate port, but requests fail in this environment because the Event Log provider cannot write and an existing DPAPI Data Protection key cannot be decrypted. This is an environment/bootstrap issue; end-to-end route testing remains pending after logging and Data Protection are configured.
