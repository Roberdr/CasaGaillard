using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CasaGaillard.Models;
using CasaGaillard.Models.ViewModels;

namespace CasaGaillard.Areas.Pagina.Controllers
{
    [RouteArea("Pagina")]
    public class PaginaController : Controller
    {
        private readonly GaillardEntities db = new GaillardEntities();

        public async Task<ActionResult> Index()
        {
            var model = new PaginaInicioViewModel
            {
                Servicios = ObtenerServiciosPublicos(),
                RecomendacionesSeguridad = ObtenerRecomendacionesSeguridad(),
                ProductosDestacados = new List<ProductoStockPublicoViewModel>()
            };

            try
            {
                model.TotalProductos = await db.Productos.CountAsync();
                model.TotalCompartimentos = await db.Compartimentos.CountAsync();
                model.TotalServicios = await db.TiposMantenimiento.CountAsync();
                model.ProductosDestacados = await ObtenerCatalogoProductos()
                    .Take(4)
                    .ToListAsync();
            }
            catch (Exception)
            {
                ViewBag.CatalogoNoDisponible = true;
            }

            return View(model);
        }

        public async Task<ActionResult> Tienda()
        {
            var model = new PaginaStockViewModel
            {
                Productos = new List<ProductoStockPublicoViewModel>()
            };

            try
            {
                model.Productos = await ObtenerCatalogoProductos().ToListAsync();
            }
            catch (Exception)
            {
                ViewBag.CatalogoNoDisponible = true;
            }

            return View(model);
        }

        public ActionResult Servicios()
        {
            return View(ObtenerServiciosPublicos());
        }

        public ActionResult Seguridad()
        {
            return View(ObtenerRecomendacionesSeguridad());
        }

        public ActionResult Contacto()
        {
            return View();
        }

        public ActionResult Imagen(string archivo)
        {
            if (string.IsNullOrWhiteSpace(archivo))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var raiz = Path.GetFullPath(Server.MapPath("~/Content/images"));
            var ruta = Path.GetFullPath(Path.Combine(raiz, archivo.Replace('/', Path.DirectorySeparatorChar)));
            var raizConSeparador = raiz.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (!ruta.StartsWith(raizConSeparador, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(ruta))
            {
                return HttpNotFound();
            }

            return File(ruta, ObtenerContentType(ruta));
        }

        private IQueryable<ProductoStockPublicoViewModel> ObtenerCatalogoProductos()
        {
            return db.Productos
                .AsNoTracking()
                .OrderBy(p => p.Producto1)
                .Select(p => new ProductoStockPublicoViewModel
                {
                    Nombre = p.Producto1,
                    Iupac = p.Iupac,
                    Formula = p.Formula,
                    NumeroUN = p.NumUN,
                    Grupo = p.Grupo,
                    TotalCompartimentos = p.Compartimentos.Count(),
                    CapacidadTotal = p.Compartimentos.Select(c => c.Capacidad ?? 0).Sum(),
                    MaterialPredominante = p.Compartimentos
                        .Select(c => c.Material != null ? c.Material.Material1 : null)
                        .FirstOrDefault()
                });
        }

        private IList<ServicioPublicoViewModel> ObtenerServiciosPublicos()
        {
            return new List<ServicioPublicoViewModel>
            {
                new ServicioPublicoViewModel
                {
                    Titulo = "Mantenimiento de cubas",
                    Descripcion = "Control técnico, revisiones periódicas y conservación del parque de cubas.",
                    Detalle = "Seguimiento de estado, operaciones de mantenimiento y soporte documental."
                },
                new ServicioPublicoViewModel
                {
                    Titulo = "Mantenimiento de vehículos",
                    Descripcion = "Revisiones, control de inspecciones y seguimiento de incidencias de la flota.",
                    Detalle = "Gestión de revisiones, caducidades y próximos vencimientos de seguridad."
                },
                new ServicioPublicoViewModel
                {
                    Titulo = "Gestión de materiales",
                    Descripcion = "Consulta organizada de productos, materiales y accesorios asociados.",
                    Detalle = "Inventario técnico para localizar referencias, formatos y familias de producto."
                }
            };
        }

        private static string ObtenerContentType(string ruta)
        {
            var extension = Path.GetExtension(ruta);

            if (string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase))
            {
                return "image/png";
            }

            if (string.Equals(extension, ".gif", StringComparison.OrdinalIgnoreCase))
            {
                return "image/gif";
            }

            if (string.Equals(extension, ".webp", StringComparison.OrdinalIgnoreCase))
            {
                return "image/webp";
            }

            return "image/jpeg";
        }

        private IList<SeguridadPublicaViewModel> ObtenerRecomendacionesSeguridad()
        {
            return new List<SeguridadPublicaViewModel>
            {
                new SeguridadPublicaViewModel
                {
                    Titulo = "Inspección previa",
                    Descripcion = "Verificar estado visual, etiquetas y documentación antes de cualquier operación.",
                    Nivel = "Prevención"
                },
                new SeguridadPublicaViewModel
                {
                    Titulo = "Control de equipos",
                    Descripcion = "Revisar válvulas, cierres, conexiones y sistemas de retención con frecuencia.",
                    Nivel = "Operativa"
                },
                new SeguridadPublicaViewModel
                {
                    Titulo = "Uso de EPI",
                    Descripcion = "Mantener el uso correcto de guantes, calzado y protección adecuada en cada tarea.",
                    Nivel = "Protección"
                },
                new SeguridadPublicaViewModel
                {
                    Titulo = "Actuación ante incidencia",
                    Descripcion = "Detener la operación, señalizar la zona y comunicar cualquier fuga, daño o anomalía.",
                    Nivel = "Respuesta"
                }
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
