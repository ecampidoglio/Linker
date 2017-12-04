using System;
using System.Web.Http;
using System.Web.Http.Results;

namespace Linker.Tests.Extensions
{
    internal static class HttpActionResultExtensions
    {
        internal static T ContentAs<T>(this IHttpActionResult result)
        {
            return result.As<OkNegotiatedContentResult<T>>().Content;
        }

        internal static T As<T>(this object source) where T : class
        {
            if (!(source is T value))
            {
                throw new ArgumentException(
                    $"The value should be of type {typeof(T)}, but is {source.GetType()}",
                    nameof(source));
            }

            return value;
        }
    }
}
