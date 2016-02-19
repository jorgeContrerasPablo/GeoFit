using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RestServiceGeoFit.Startup))]
namespace RestServiceGeoFit
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
