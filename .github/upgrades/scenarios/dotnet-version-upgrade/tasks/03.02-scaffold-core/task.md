# 03.02-scaffold-core: Scaffold ASP.NET Core project CasaGaillard.Core

## Objective
Crear un nuevo proyecto ASP.NET Core (CasaGaillard.Core) en la solución para soportar la migración side-by-side.

## Scope
- Crear el proyecto CasaGaillard.Core (Razor Pages template) targeting net10.0
- Añadir el proyecto a la solución CasaGaillard.sln
- Confirmar que el proyecto build y run localmente

## Done when
- Proyecto CasaGaillard.Core añadido a la solución y restaurado
- `dotnet build` para CasaGaillard.Core pasa sin errores
- Commit con el proyecto scaffolded

## Initial actions performed
- Se ejecutó `dotnet new webapp -n CasaGaillard.Core -o CasaGaillard.Core --framework net10.0`
- Se ejecutó `dotnet sln add CasaGaillard.Core/CasaGaillard.Core.csproj`
- Commit realizado: "scaffold(core): add CasaGaillard.Core ASP.NET Core project for side-by-side migration"

## Next steps
- Validar build focalizado: `dotnet build CasaGaillard.Core/CasaGaillard.Core.csproj`
- Abrir Program.cs en el nuevo proyecto y preparar plantillas para migrar controladores

Recorded at: automatic scaffolding
