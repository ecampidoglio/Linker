using Microsoft.AspNetCore.Mvc;

namespace Linker.Tests
{
    internal static class LinkControllerActionResultExtensions
    {
        internal static string CreatedLinkId(this IActionResult result)
        {
            return result
                .As<CreatedAtRouteResult>()
                .RouteValues["id"] as string;
        }
    }
}
