using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjetoPadrao.Web.Startup))]
namespace ProjetoPadrao.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
