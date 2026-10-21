$conexion = Get-NetTCPConnection -LocalPort 5043 -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1
if ($conexion) {Stop-Process -Id $conexion.OwningProcess -Force}
dotnet build CasaGaillard.Core.csproj -c Release --no-restore
.\Start-LanServer.ps1