using System.Web;
using System.Web.Optimization;

namespace CasaGaillard
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new Bundle("~/bundles/js").Include(
                        "~/Scripts/Site.js",
                        "~/Scripts/bootstrap.bundle.js"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

           
            bundles.Add(new StyleBundle("~/bundles/styles")
                      .Include("~/Content/bootstrap.css")
                      .Include("~/Content/Site.css", new CssRewriteUrlTransform()));
            //bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
            //          "~/Content/CSS/font-awesome.css"));
        }
    }
}
