# Stop CasaGaillard.Core process and any process using port 5200
Try {
	$procs = Get-Process -Name CasaGaillard.Core -ErrorAction SilentlyContinue
	if ($procs) {
		$procs | ForEach-Object { Stop-Process -Id $_.Id -Force; Write-Output "Stopped process by name: $($_.Id)" }
	} else {
		Write-Output "No process named CasaGaillard.Core found"
	}
} Catch {
	Write-Output "Error stopping by name: $($_.Exception.Message)"
}

# Find processes using port 5200
$lines = netstat -ano | Select-String ":5200"
if ($lines) {
	$lines | ForEach-Object {
		$parts = ($_ -replace '^\s+','') -split '\s+'
		$pid = $parts[-1]
		if ($pid -and $pid -match '^\d+$') {
			Try {
				Stop-Process -Id $pid -Force -ErrorAction Stop
				Write-Output "Stopped process using port 5200: $pid"
			} Catch {
				# fallback to taskkill
				cmd /c "taskkill /PID $pid /F" | Out-Null
				Write-Output "Attempted taskkill on pid: $pid"
			}
		}
	}
} else {
	Write-Output "No process found listening on port 5200"
}
