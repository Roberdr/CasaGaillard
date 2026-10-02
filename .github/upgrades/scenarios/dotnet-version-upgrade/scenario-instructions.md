# dotnet-version-upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: Tienda
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)

## User Preferences
- **Schema ownership**: EF Core será el propietario de los cambios en el esquema durante la ventana side-by-side (confirmado por el usuario).
- **Preserve test scripts**: Mantener scripts de prueba locales para validaciones rápidas (scripts/test_edit_intervencion.ps1, scripts/stop_local_server.ps1). Serán revisados y/o eliminados antes del merge si procede.

## Notes
- Inicio de la migración solicitado por el usuario desde la rama `Tienda`.
- Repositorio remoto: origin -> https://github.com/Roberdr/CasaGaillard
- No detectar cambios pendientes al iniciar (working tree limpio)

> Archivo generado automáticamente por el flujo de inicialización de dotnet-version-upgrade.
