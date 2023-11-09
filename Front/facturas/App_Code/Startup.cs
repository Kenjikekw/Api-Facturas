using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(facturas.Startup))]
namespace facturas
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
