using System.Web.Mvc;

namespace CasaGaillard.Areas.Pagina
{
    public class PaginaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Pagina";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Pagina_default",
                "Pagina/{controller}/{action}/{id}",
                new { controller = "Pagina", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}