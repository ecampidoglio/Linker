using System.Web.Http;
using Owin;

namespace Linker.Web.Configuration
{
    public static class WebApiConfiguration
    {
        public static void ConfigureWebApi(this IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.AddDefaultRouteConvention();
            config.ConfigureDependencyResolution();
            config.EnableJsonResponsesInTheBrowser();

            app.UseWebApi(config);
        }
    }
}
