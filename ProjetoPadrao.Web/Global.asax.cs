using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ProjetoPadrao.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            HostingEnvironment.RegisterVirtualPathProvider(new ProjetoPadrao.WebEngine.DbVirtualPathProvider());

            foreach (var viewEngine in ViewEngines.Engines.Where(ve => !(ve is RazorViewEngine)).ToList())
            {
                ViewEngines.Engines.Remove(viewEngine);
            }

            foreach (var viewEngine in ViewEngines.Engines.OfType<RazorViewEngine>())
            {
                viewEngine.AreaViewLocationFormats = viewEngine.AreaViewLocationFormats.Where(f => f.EndsWith(".cshtml")).ToArray();
                viewEngine.AreaMasterLocationFormats = viewEngine.AreaMasterLocationFormats.Where(f => f.EndsWith(".cshtml")).ToArray();
                viewEngine.AreaPartialViewLocationFormats = viewEngine.AreaPartialViewLocationFormats.Where(f => f.EndsWith(".cshtml")).ToArray();
                viewEngine.ViewLocationFormats = viewEngine.ViewLocationFormats.Where(f => f.EndsWith(".cshtml")).ToArray();
                viewEngine.MasterLocationFormats = viewEngine.MasterLocationFormats.Where(f => f.EndsWith(".cshtml")).ToArray();
                viewEngine.PartialViewLocationFormats = viewEngine.PartialViewLocationFormats.Where(f => f.EndsWith(".cshtml")).ToArray();
                viewEngine.FileExtensions = viewEngine.FileExtensions.Where(f => f.EndsWith("cshtml")).ToArray();
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
