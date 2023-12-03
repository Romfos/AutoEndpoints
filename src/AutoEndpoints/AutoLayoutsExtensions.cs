using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AutoEndpoints;

public static class AutoLayoutsExtensions
{
    public static GetRequestBuilder MapGetLayout(this WebApplication webApplication, string pattern)
    {
        return new GetRequestBuilder(webApplication, pattern);
    }

    public static PostRequestBuilder MapPostLayout(this WebApplication webApplication, string pattern)
    {
        return new PostRequestBuilder(webApplication, pattern);
    }

    public static PostRequestBuilder<T> MapPostLayout<T>(this WebApplication webApplication, string pattern)
    {
        return new PostRequestBuilder<T>(webApplication, pattern);
    }

    public static string GetStringRouteValue(this HttpContext httpContext, string key)
    {
        var value = httpContext.GetRouteValue(key);
        if (value is not string stringValue)
        {
            throw new Exception($"Unable to get route {key} value");
        }
        return stringValue;
    }
}