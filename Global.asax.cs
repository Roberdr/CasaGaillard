using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Text;
using System.Globalization;
using System.Threading;
// No usar 'using CasaGaillard.ModelBinders' para evitar ambigüedad con System.Web.Mvc.ModelBinders

namespace CasaGaillard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;

            // Registrar model binders para decimales que acepten coma como separador decimal
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(decimal), new CasaGaillard.ModelBinders.DecimalModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(decimal?), new CasaGaillard.ModelBinders.DecimalModelBinder());
        }

        protected void Application_BeginRequest()
        {
            var culture = new CultureInfo("es-ES");

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
        }
    }
}
