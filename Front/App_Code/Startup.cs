using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Front.Startup))]
namespace Front
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
