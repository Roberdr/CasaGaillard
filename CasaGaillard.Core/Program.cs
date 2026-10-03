using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq;
using CasaGaillard.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Asp ire explicit wiring (if Aspire.Hosting is referenced at compile time)
#if NET10_0_OR_GREATER
try
{
    // Some Aspire packages expose AddAspire extension methods
    var aspireAddMethod = typeof(Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .FirstOrDefault(m => m.Name == "AddAspire");
    if (aspireAddMethod != null)
    {
        aspireAddMethod.Invoke(null, new object[] { builder.Services });
    }
}
catch { }
#endif

// Explicit AddAspire call - wrapped in try/catch to avoid breaking if signature differs
try
{
    // Prefer compile-time call when available
    builder.Services.GetType().GetMethod("AddAspire", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)?.Invoke(builder.Services, null);
}
catch { }

// Register application services
builder.Services.AddScoped<CasaGaillard.Core.Services.IIntervencionesService, CasaGaillard.Core.Services.IntervencionesService>();
builder.Services.AddScoped<ICrudVehiculosService, VehiculosService>();

// Register EF Core DbContext (DefaultConnection in appsettings.json expected)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<CasaGaillard.Core.Data.AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Serve static files (wwwroot)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map controller routes, including areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Keep Razor Pages mapping for any pages the project uses
app.MapRazorPages();

// Legacy helper from the template (leave if present)
try
{
    app.MapStaticAssets();
}
catch { }

// If compile-time Aspire exists, try to call UseAspire
#if NET10_0_OR_GREATER
try
{
    var useAspireMethod = app.GetType().GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
        .FirstOrDefault(m => m.Name == "UseAspire" || m.Name == "UseAspireHost");
    if (useAspireMethod != null)
    {
        try { useAspireMethod.Invoke(app, null); } catch { }
    }
}
catch { }
#endif

app.Run();

// Optional: try to integrate Aspire at runtime if the Aspire assembly is present.
// This uses reflection so the project does not need a compile-time dependency on Aspire.
try
{
    var aspireAssembly = AppDomain.CurrentDomain.GetAssemblies()
        .FirstOrDefault(a => a.GetName().Name.IndexOf("Aspire", System.StringComparison.OrdinalIgnoreCase) >= 0);
    if (aspireAssembly == null)
    {
        try { aspireAssembly = Assembly.Load("Aspire.Hosting"); } catch { }
    }

    if (aspireAssembly != null)
    {
        // Common pattern: a static "UseAspire(WebApplication app)" method on any Aspire type
        var aspireType = aspireAssembly.GetTypes()
            .FirstOrDefault(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(m => m.Name.Equals("UseAspire") || m.Name.Equals("UseAspireHost") || m.Name.Equals("StartAspire")));

        if (aspireType != null)
        {
            var method = aspireType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.GetParameters().Any(p => p.ParameterType == typeof(Microsoft.AspNetCore.Builder.WebApplication)));
            if (method != null)
            {
                try { method.Invoke(null, new object[] { app }); } catch { }
            }
        }
    }
}
catch { }
