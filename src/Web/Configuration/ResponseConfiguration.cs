using System.Net.Http.Headers;
using System.Web.Http;

namespace Linker.Web.Configuration
{
    internal static class ResponseConfiguration
    {
        internal static void EnableJsonResponsesInTheBrowser(
            this HttpConfiguration config)
        {
            config
                .Formatters
                .JsonFormatter
                .SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
