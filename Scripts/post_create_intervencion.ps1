$uri = 'http://localhost:5200/Mantenimiento/Intervenciones/Create'
$sess = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$resp = Invoke-WebRequest -Uri $uri -WebSession $sess -UseBasicParsing
if ($null -eq $resp) { Write-Error 'No response from GET'; exit 1 }
# extract antiforgery token
if ($resp.Content -match 'name="__RequestVerificationToken"\s+value="([^"]+)"') {
	$token = $matches[1]
} elseif ($resp.Content -match 'name="__RequestVerificationToken"\s+type="hidden"\s+value="([^"]+)"') {
	$token = $matches[1]
} else {
	Write-Error 'Antiforgery token not found in response HTML'; exit 2
}
$body = @{
	'Titulo' = 'Prueba Automática'
	'Descripcion' = 'Creada por test'
	'__RequestVerificationToken' = $token
}
try {
	$post = Invoke-WebRequest -Uri $uri -Method Post -Body $body -WebSession $sess -UseBasicParsing -ErrorAction Stop
	if ($post -ne $null) {
		Write-Output "STATUS:$($post.StatusCode)"
		if ($post.Headers.Location) { Write-Output "LOC:$($post.Headers.Location)" }
		if ($post.Content) { Write-Output "LENGTH:$($post.Content.Length)" }
	} else {
		Write-Output 'POST returned no response object'
	}
} catch {
	Write-Error "POST failed: $($_.Exception.Message)"
	exit 3
}
