using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AerLingus.Startup))]
namespace AerLingus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
