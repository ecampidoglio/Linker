using System.Web.Http;
using System.Web.Http.Results;

namespace Linker.Tests.Extensions
{
    internal static class LinkControllerActionResultExtensions
    {
        internal static string CreatedLinkId(this IHttpActionResult source)
        {
            return source
                .As<CreatedAtRouteNegotiatedContentResult<string>>()
                .RouteValues["id"] as string;
        }
    }
}
