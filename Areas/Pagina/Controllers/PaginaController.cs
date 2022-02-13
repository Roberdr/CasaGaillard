using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CasaGaillard.Areas.Pagina.Controllers
{
    [RouteArea("Pagina")]

    public class PaginaController : Controller
    {
        // GET: Pagina/Pagina
        public ActionResult Index()
        {
            return View();
        }

        // GET: Pagina/Pagina/Tienda

        public ActionResult Tienda()
        {
            return View();
        }
    }
}