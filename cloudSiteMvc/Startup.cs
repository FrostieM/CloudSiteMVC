using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(cloudSiteMvc.Startup))]
namespace cloudSiteMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
