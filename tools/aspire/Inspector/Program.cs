using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

if (args.Length == 0)
{
    Console.WriteLine("Usage: AsmInspector <path-to-assembly.dll> [--scan-strings]");
    return 1;
}

var asmPath = args[0];
var scanStrings = args.Length > 1 && args[1] == "--scan-strings";
if (!File.Exists(asmPath))
{
    Console.WriteLine("NOT_FOUND");
    return 2;
}

using var module = ModuleDefinition.ReadModule(asmPath);

// Existing API/method name scan
foreach (var type in module.Types.OrderBy(t => t.FullName))
{
    try
    {
        if (!type.IsPublic) continue;
        var methods = type.Methods.Where(m => m.Name.IndexOf("Use", StringComparison.OrdinalIgnoreCase) >= 0 || m.Name.IndexOf("Map", StringComparison.OrdinalIgnoreCase) >= 0 || m.Name.IndexOf("Dashboard", StringComparison.OrdinalIgnoreCase) >= 0 || m.Name.IndexOf("Aspire", StringComparison.OrdinalIgnoreCase) >= 0 || m.Name.IndexOf("Start", StringComparison.OrdinalIgnoreCase) >= 0 || m.Name.IndexOf("Host", StringComparison.OrdinalIgnoreCase) >= 0 || m.Name.IndexOf("MapAspire", StringComparison.OrdinalIgnoreCase) >= 0 || m.Name.IndexOf("MapDashboard", StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
        if (methods.Length == 0) continue;
        Console.WriteLine($"TYPE: {type.FullName}");
        foreach (var m in methods)
        {
            var pars = string.Join(", ", m.Parameters.Select(p => p.ParameterType.FullName));
            var ret = m.ReturnType?.FullName ?? "void";
            Console.WriteLine($"  METHOD: {m.Name}({pars}) -> {ret}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"TYPE_LOAD_ERR: {type.FullName} : {ex.Message}");
    }
}

if (scanStrings)
{
    Console.WriteLine();
    Console.WriteLine("Scanning string literals for 'dashboard' (case-insensitive)");
    var hits = 0;
    foreach (var t in module.Types)
    {
        foreach (var m in t.Methods.Where(mm => mm.HasBody))
        {
            try
            {
                foreach (var instr in m.Body.Instructions)
                {
                    if (instr.OpCode == OpCodes.Ldstr && instr.Operand is string s)
                    {
                        if (s.IndexOf("dashboard", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Console.WriteLine($"LITERAL in {t.FullName}.{m.Name}: '{s}'");
                            hits++;
                        }
                    }
                }
            }
            catch { }
        }
    }

    // Also scan embedded resources (text based)
    foreach (var res in module.Resources)
    {
        try
        {
            if (res is EmbeddedResource er)
            {
                using var stream = er.GetResourceStream();
                if (stream == null) continue;
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                var data = ms.ToArray();
                var text = System.Text.Encoding.UTF8.GetString(data);
                if (text.IndexOf("dashboard", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine($"EMBEDDED_RESOURCE {er.Name} contains 'dashboard'");
                    hits++;
                }
            }
        }
        catch { }
    }

    if (hits == 0) Console.WriteLine("No string literals containing 'dashboard' found.");
}

return 0;
