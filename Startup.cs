using Microsoft.Owin;
using Owin;
using Microsoft.AspNetCore.Builder;

[assembly: OwinStartupAttribute(typeof(CasaGaillard.Startup))]
namespace CasaGaillard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
