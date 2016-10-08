using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProjetoPadrao.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Content/{*path}");
            routes.IgnoreRoute("Scripts/{*path}");

            /*
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ProjetoPadrao.Web.Controllers" }
            );
			*/
            routes.RouteExistingFiles = true;

            routes.MapRoute(
                name: "Imagem",
                url: "Images/Upload/{*nome}",
                defaults: new { controller = "Imagem", action = "Index" }
            );

            routes.MapRoute(
				name: "Conteudo",
				url: "{*path}",
				defaults: new { controller = "Conteudo", action = "Index" }
			);
        }
    }
}
