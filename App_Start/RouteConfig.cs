﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CasaGaillard
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Enabling attribute routing 
            routes.MapMvcAttributeRoutes();

            //routes.MapRoute(
                //name: "Default",
                //url: "{controller}/{action}/{id}",
                //defaults: new {action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}

