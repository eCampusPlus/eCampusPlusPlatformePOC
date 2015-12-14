using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eCampusPlusPlatformePOC.Startup))]
namespace eCampusPlusPlatformePOC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
