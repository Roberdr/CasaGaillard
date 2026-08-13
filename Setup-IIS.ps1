param(
    [string]$SiteName = "CasaGaillard",
    [string]$AppPoolName = "CasaGaillardPool",
    [string]$PhysicalPath = "C:\Users\rober\source\repos\Roberdr\CasaGaillard",
    [int]$Port = 8085
)

$ErrorActionPreference = "Stop"

$appCmd = Join-Path $env:windir "System32\inetsrv\appcmd.exe"
if (-not (Test-Path $appCmd)) {
    throw "No se ha encontrado appcmd.exe. Asegurate de que IIS esta instalado."
}

Write-Host "Preparando IIS para $SiteName..." -ForegroundColor Cyan

& $appCmd list apppool $AppPoolName | Out-Null
if ($LASTEXITCODE -eq 0) {
    & $appCmd delete apppool "/apppool.name:$AppPoolName" | Out-Null
}

& $appCmd add apppool "/name:$AppPoolName" "/managedRuntimeVersion:v4.0" "/managedPipelineMode:Integrated" "/startMode:AlwaysRunning" | Out-Null
& $appCmd set apppool "/apppool.name:$AppPoolName" "/processModel.identityType:ApplicationPoolIdentity" "/processModel.loadUserProfile:true" | Out-Null

& $appCmd list site $SiteName | Out-Null
if ($LASTEXITCODE -eq 0) {
    & $appCmd delete site "/site.name:$SiteName" | Out-Null
}

& $appCmd add site "/name:$SiteName" "/bindings:http/*:${Port}:" "/physicalPath:$PhysicalPath" | Out-Null
& $appCmd set app "/app.name:$SiteName/" "/applicationPool:$AppPoolName" | Out-Null

$firewallRule = "IIS $SiteName HTTP $Port"
if (-not (Get-NetFirewallRule -DisplayName $firewallRule -ErrorAction SilentlyContinue)) {
    New-NetFirewallRule -DisplayName $firewallRule -Direction Inbound -Action Allow -Protocol TCP -LocalPort $Port | Out-Null
}

Write-Host ""
Write-Host "Listo." -ForegroundColor Green
Write-Host "Sitio: http://localhost:$Port" -ForegroundColor Green
Write-Host "Pool: $AppPoolName" -ForegroundColor Green
Write-Host "Identidad del pool: ApplicationPoolIdentity" -ForegroundColor Green
