# Test Edit flow: GET form, extract antiforgery token, POST update, verify Details
$uri = 'http://localhost:5200/Mantenimiento/Intervenciones/Edit/1'
$sess = New-Object Microsoft.PowerShell.Commands.WebRequestSession

Write-Output "GET $uri"
$get = Invoke-WebRequest -Uri $uri -WebSession $sess -UseBasicParsing -ErrorAction Stop
if (-not $get) { Write-Error 'No response from GET'; exit 1 }

# Try to extract token from Forms collection first
$token = $null
if ($get.Forms -and $get.Forms.Count -gt 0) {
	try {
		$form = $get.Forms[0]
		if ($form.Fields.ContainsKey('__RequestVerificationToken')) { $token = $form.Fields['__RequestVerificationToken'] }
	} catch { }
}

# Fallback: regex on HTML
if (-not $token) {
	if ($get.Content -match 'name="__RequestVerificationToken"\s*type="hidden"\s*value="([^"]+)"') {
		$token = $matches[1]
	} elseif ($get.Content -match 'name="__RequestVerificationToken"\s*value="([^"]+)"') {
		$token = $matches[1]
	}
}

if (-not $token) { Write-Error 'Antiforgery token not found'; exit 2 }
Write-Output "Token extracted (len=$(($token).Length))"

$body = @{
	'Id' = '1'
	'Titulo' = 'Editada por script'
	'Descripcion' = 'Actualizada por script'
	'__RequestVerificationToken' = $token
}

Write-Output 'POSTing form...'
$post = Invoke-WebRequest -Uri $uri -Method Post -Body $body -WebSession $sess -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
if ($post -and $post.StatusCode) {
	Write-Output "POST STATUS: $($post.StatusCode)"
	if ($post.Headers.Location) { Write-Output "Location: $($post.Headers.Location)" }
} else {
	Write-Output 'POST returned no status object; attempting to follow with GET to Details'
}

# Verify by fetching Details/1
$detailsUrl = 'http://localhost:5200/Mantenimiento/Intervenciones/Details/1'
Write-Output "GET $detailsUrl"
$details = Invoke-WebRequest -Uri $detailsUrl -UseBasicParsing -ErrorAction Stop
if ($details.Content -match 'Editada por script') {
	Write-Output 'VERIFIED: Details contains updated title.'
	exit 0
} else {
	Write-Output 'NOT VERIFIED: Details did not contain updated title.'
	exit 3
}
