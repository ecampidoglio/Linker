using System.Web.Http;

namespace Linker.Web.Configuration
{
    internal static class RouteConfiguration
    {
        internal static void AddDefaultRouteConvention(
            this HttpConfiguration config)
        {
            config
                .Routes
                .MapHttpRoute(
                    "DefaultApi",
                    "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional });
        }
    }
}
