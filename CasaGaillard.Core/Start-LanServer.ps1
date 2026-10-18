$ErrorActionPreference = 'Stop'

$projectRoot = $PSScriptRoot
$appAssembly = Join-Path $projectRoot 'bin\Release\net10.0\CasaGaillard.Core.dll'
$logDirectory = Join-Path $projectRoot 'App_Data'
$outputLog = Join-Path $logDirectory 'kestrel-lan.log'
$errorLog = Join-Path $logDirectory 'kestrel-lan-error.log'

if (-not (Test-Path -LiteralPath $appAssembly)) {
    throw "No se encuentra la publicación de CasaGaillard.Core: $appAssembly"
}
if (-not (Test-Path -LiteralPath (Join-Path $projectRoot 'appsettings.Lan.json'))) {
    throw 'Falta appsettings.Lan.json. Configura la conexión SQL, el certificado y Kestrel antes de iniciar el servidor.'
}

New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null
$client = [System.Net.Sockets.TcpClient]::new()
try {
    $connect = $client.ConnectAsync('127.0.0.1', 5043)
    if ($connect.Wait(300)) {
        return
    }
} catch {
    # No listener is present yet; launch the application below.
} finally {
    $client.Dispose()
}

$oldEnvironment = $env:ASPNETCORE_ENVIRONMENT
$env:ASPNETCORE_ENVIRONMENT = 'Lan'
try {
    $process = Start-Process -FilePath 'dotnet.exe' `
        -ArgumentList 'bin\Release\net10.0\CasaGaillard.Core.dll' `
        -WorkingDirectory $projectRoot `
        -WindowStyle Hidden `
        -RedirectStandardOutput $outputLog `
        -RedirectStandardError $errorLog `
        -PassThru
} finally {
    if ($null -eq $oldEnvironment) {
        Remove-Item Env:ASPNETCORE_ENVIRONMENT -ErrorAction SilentlyContinue
    } else {
        $env:ASPNETCORE_ENVIRONMENT = $oldEnvironment
    }
}

Start-Sleep -Seconds 3
$process.Refresh()
if ($process.HasExited) {
    throw "Kestrel no pudo iniciarse. Consulta $errorLog"
}
