using System;
using Microsoft.AspNetCore.Mvc;

namespace Linker.Tests
{
    internal static class ActionResultExtensions
    {
        internal static int? StatusCode(this IActionResult result)
        {
            return result.As<ObjectResult>().StatusCode;
        }

        internal static T ContentAs<T>(this IActionResult result)
        {
            return (T)result.As<ObjectResult>().Value;
        }

        internal static T As<T>(this IActionResult result) where T : class
        {
            if (!(result is T value))
            {
                throw new ArgumentException(
                    $"The result should be of type {typeof(T)}, but is {result.GetType()}",
                    nameof(result));
            }

            return value;
        }
    }
}
