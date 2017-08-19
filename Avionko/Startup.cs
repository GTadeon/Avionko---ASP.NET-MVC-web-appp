using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Avionko.Startup))]
namespace Avionko
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
