using CasaGaillard.Core.Data;
using CasaGaillard.Core.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace CasaGaillard.Core.Services;

public sealed class PaginaPublicaService : IPaginaPublicaService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _environment;
    private static readonly FileExtensionContentTypeProvider ContentTypes = new();
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    public PaginaPublicaService(AppDbContext db, IWebHostEnvironment environment) { _db=db; _environment=environment; }

    public async Task<PaginaInicioViewModel> GetHomeAsync()
    {
        var model = new PaginaInicioViewModel { Servicios=GetServices(), RecomendacionesSeguridad=GetSafetyRecommendations() };
        try
        {
            model.TotalProductos = await CountAsync("SELECT COUNT(*) AS Value FROM gaillard.Producto");
            model.TotalCompartimentos = await CountAsync("SELECT COUNT(*) AS Value FROM gaillard.Compartimento");
            model.TotalServicios = await CountAsync("SELECT COUNT(*) AS Value FROM gaillard.TipoMantenimiento");
            model.ProductosDestacados = await GetProductsAsync(4);
        }
        catch { model.CatalogoNoDisponible=true; }
        return model;
    }

    public async Task<PaginaStockViewModel> GetStockAsync()
    {
        var model = new PaginaStockViewModel();
        try { model.Productos=await GetProductsAsync(null); }
        catch { model.CatalogoNoDisponible=true; }
        return model;
    }

    public async Task<(string Path, string ContentType)?> GetImageAsync(string archivo)
    {
        if (string.IsNullOrWhiteSpace(archivo) || Path.IsPathRooted(archivo)) return null;
        var root=Path.GetFullPath(GetImageRoot());
        var path=Path.GetFullPath(Path.Combine(root,archivo.Replace('/',Path.DirectorySeparatorChar).Replace('\\',Path.DirectorySeparatorChar)));
        if (!path.StartsWith(root+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase) || !ImageExtensions.Contains(Path.GetExtension(path)) || !File.Exists(path)) return null;
        ContentTypes.TryGetContentType(path,out var contentType);
        return (path,contentType??"application/octet-stream");
    }

    private async Task<IReadOnlyList<ProductoStockPublicoViewModel>> GetProductsAsync(int? take)
    {
        var sql=@"SELECT p.Producto Nombre,p.Iupac,p.Formula,p.NumUN NumeroUN,p.Grupo,COUNT(c.ID) TotalCompartimentos,
            SUM(COALESCE(c.Capacidad,0)) CapacidadTotal,
            (SELECT TOP(1) m.Material FROM gaillard.Compartimento ci LEFT JOIN gaillard.Material m ON m.ID=ci.MaterialInteriorID WHERE ci.ProductoID=p.ID ORDER BY ci.ID) MaterialPredominante
            FROM gaillard.Producto p LEFT JOIN gaillard.Compartimento c ON c.ProductoID=p.ID
            GROUP BY p.ID,p.Producto,p.Iupac,p.Formula,p.NumUN,p.Grupo ORDER BY p.Producto";
        if (take.HasValue) sql=sql.Replace("SELECT p.Producto",$"SELECT TOP({take.Value}) p.Producto");
        return await _db.Database.SqlQueryRaw<ProductoStockPublicoViewModel>(sql).ToListAsync();
    }

    private async Task<int> CountAsync(string sql) => await _db.Database.SqlQueryRaw<int>(sql).SingleAsync();
    private string GetImageRoot()
    {
        var sibling=Path.GetFullPath(Path.Combine(_environment.ContentRootPath,"..","Content","images"));
        var local=Path.Combine(_environment.ContentRootPath,"Content","images");
        return Directory.Exists(sibling)?sibling:local;
    }
    public IReadOnlyList<ServicioPublicoViewModel> GetServices() =>
    [
        new(){Titulo="Mantenimiento de cubas",Descripcion="Control técnico, revisiones periódicas y conservación del parque de cubas.",Detalle="Seguimiento de estado, operaciones de mantenimiento y soporte documental."},
        new(){Titulo="Mantenimiento de vehículos",Descripcion="Revisiones, control de inspecciones y seguimiento de incidencias de la flota.",Detalle="Gestión de revisiones, caducidades y próximos vencimientos de seguridad."},
        new(){Titulo="Gestión de materiales",Descripcion="Consulta organizada de productos, materiales y accesorios asociados.",Detalle="Inventario técnico para localizar referencias, formatos y familias de producto."}
    ];
    public IReadOnlyList<SeguridadPublicaViewModel> GetSafetyRecommendations() =>
    [
        new(){Titulo="Inspección previa",Descripcion="Verificar estado visual, etiquetas y documentación antes de cualquier operación.",Nivel="Prevención"},
        new(){Titulo="Control de equipos",Descripcion="Revisar válvulas, cierres, conexiones y sistemas de retención con frecuencia.",Nivel="Operativa"},
        new(){Titulo="Uso de EPI",Descripcion="Mantener el uso correcto de guantes, calzado y protección adecuada en cada tarea.",Nivel="Protección"},
        new(){Titulo="Actuación ante incidencia",Descripcion="Detener la operación, señalizar la zona y comunicar cualquier fuga, daño o anomalía.",Nivel="Respuesta"}
    ];
}
