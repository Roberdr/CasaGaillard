Integrate Aspire (minimal)

1. Add package (recommended stable version):

   dotnet add CasaGaillard.Core package Aspire.Hosting --version 9.*

2. Add wiring to Program.cs (before app.Run()):

   builder.Services.AddAspire();
   app.UseAspire();

   See tools/aspire/README.md for exact API names if package uses different method names.

3. Update appsettings.Development.json with Aspire configuration (port, api key) if needed.

4. Start the Aspire dashboard (if provided) or run the app and open the dashboard URL.

Notes:
- This repo already includes a runtime-safe reflection-based attempt to call Aspire if the assembly is present; adding the package and the service registrations gives compile-time wiring and explicit startup behavior.

Safety:
- Add package on the Core project only. Build and test locally before PR.

