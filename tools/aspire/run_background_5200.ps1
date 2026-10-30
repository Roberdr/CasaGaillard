# Stop any running CasaGaillard.Core host processes (by command line)
$found = $false
try {
	$procs = Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -and $_.CommandLine -match 'CasaGaillard.Core' }
	if ($procs) {
		$found = $true
		foreach ($p in $procs) {
			try { Stop-Process -Id $p.ProcessId -Force -ErrorAction Stop; Write-Output "Stopped PID:$($p.ProcessId)" } catch { Write-Output "Failed to stop PID:$($p.ProcessId) - $($_.Exception.Message)" }
		}
	}
} catch {
	Write-Output "Process enumeration failed: $($_.Exception.Message)"
}
if (-not $found) { Write-Output 'NO_RUNNING_PROCESS_FOUND' }

# Ensure logs folder
$logDir = Join-Path (Get-Location) 'logs'
if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }

# Set environment variables for child process
$env:ASPIRE_SHOW_DASHBOARD_RESOURCES = '1'
$env:ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL = 'http://localhost:4318/v1/traces'
$env:ASPNETCORE_URLS = 'http://localhost:5200'

# Start the app in background redirecting stdout/stderr and force --urls 5200
$stdout = Join-Path $logDir 'aspire.log'
$stderr = Join-Path $logDir 'aspire.err'
try {
	$proc = Start-Process -FilePath dotnet -ArgumentList 'run','--project','CasaGaillard.Core','--configuration','Debug','--urls','http://localhost:5200' -RedirectStandardOutput $stdout -RedirectStandardError $stderr -NoNewWindow -PassThru -WorkingDirectory (Get-Location)
	Write-Output "Started PID:$($proc.Id)"
} catch {
	Write-Output "Failed to start process: $($_.Exception.Message)"
	exit 1
}

# Wait a few seconds for logs to appear then show tail
Start-Sleep -Seconds 3
if (Test-Path $stdout) {
	Write-Output "--- aspire.log (last 200 lines) ---"
	Get-Content $stdout -Tail 200 | ForEach-Object { Write-Output $_ }
} else {
	Write-Output 'NO_LOG_YET'
}

if (Test-Path $stderr) {
	Write-Output "--- aspire.err (last 50 lines) ---"
	Get-Content $stderr -Tail 50 | ForEach-Object { Write-Output $_ }
}

Write-Output 'Background start complete.'
