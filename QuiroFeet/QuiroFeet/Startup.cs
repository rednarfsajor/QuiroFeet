using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QuiroFeet.Startup))]
namespace QuiroFeet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
