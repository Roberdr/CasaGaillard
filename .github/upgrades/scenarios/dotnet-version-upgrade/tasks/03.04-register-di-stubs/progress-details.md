# Progress details for 03.04-register-di-stubs

- Registered IIntervencionesService and IntervencionesService in CasaGaillard.Core/Services
- Added IntervencionViewModel in CasaGaillard.Core/Models
- Updated IntervencionesController to receive IIntervencionesService via DI and use stub methods
- Registered service in Program.cs
- Verified `dotnet build CasaGaillard.Core` succeeds after changes
- Next: run the app and perform a smoke test of the Index endpoint, then implement real data access in next task
