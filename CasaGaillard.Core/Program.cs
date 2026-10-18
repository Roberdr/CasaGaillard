using CasaGaillard.Core.Data;
using CasaGaillard.Core.Services;
using CasaGaillard.Core.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var keyDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtection-Keys"));
    var dataProtection = builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(keyDirectory)
        .SetApplicationName("CasaGaillard.Core");
    if (OperatingSystem.IsWindows())
        dataProtection.ProtectKeysWithDpapi();
}

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddUserStore<LegacyIdentityUserStore>()
    .AddRoleStore<LegacyIdentityRoleStore>()
    .AddDefaultTokenProviders();
// The legacy MVC application uses Microsoft.AspNet.Identity 2.x. Keep hashes
// created by Core readable by that application during the migration.
builder.Services.Configure<PasswordHasherOptions>(options =>
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddScoped<IIntervencionesService, IntervencionesService>();
builder.Services.AddScoped<ITareasMantenimientoService, TareasMantenimientoService>();
builder.Services.AddScoped<IInstalacionesService, InstalacionesService>();
builder.Services.AddScoped<IEquiposService, EquiposService>();
builder.Services.AddScoped<IPlanesMantenimientoService, PlanesMantenimientoService>();
builder.Services.AddScoped<ICostesMantenimientoService, CostesMantenimientoService>();
builder.Services.AddScoped<IRevisionesService, RevisionesService>();
builder.Services.AddScoped<ICrudVehiculosService, VehiculosService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
