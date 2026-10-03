$pathCandidates = @(
    'CasaGaillard.Core\bin\Debug\net10.0\Aspire.Hosting.dll',
    'CasaGaillard.Core\bin\Debug\net10.0\publish\Aspire.Hosting.dll'
)
$found = $null
foreach($p in $pathCandidates){ if(Test-Path $p){ $found = (Resolve-Path $p).Path; break } }
if(-not $found){ Write-Output 'NOT_FOUND'; exit 0 }
# Register resolver to load assemblies from same folder
$folder = Split-Path $found
[System.AppDomain]::CurrentDomain.add_AssemblyResolve({ param($sender,$args) 
    $name = $args.Name.Split(',')[0] + '.dll'
    $candidate = Join-Path $folder $name
    if(Test-Path $candidate){ return [Reflection.Assembly]::LoadFrom($candidate) }
    return $null
})
try{ $asm = [Reflection.Assembly]::LoadFrom($found) }catch{ Write-Output 'LOAD_FAILED'; Write-Output $_.Exception.Message; exit 0 }
$asm.GetExportedTypes() | Where-Object { $_.IsPublic } | ForEach-Object {
    $t = $_
    $m = $t.GetMethods([Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::Static -bor [Reflection.BindingFlags]::Instance) | Where-Object { $_.Name -match 'Use|Map|Dashboard|Aspire|Start|Host|MapAspire|MapDashboard' }
    if($m.Count -gt 0){
        Write-Output "TYPE: $($t.FullName)"
        foreach($mm in $m){
            $pars = ($mm.GetParameters() | ForEach-Object { $_.ParameterType.FullName }) -join ', '
            Write-Output "  METHOD: $($mm.Name)($pars) -> $($mm.ReturnType.FullName)"
        }
    }
}
