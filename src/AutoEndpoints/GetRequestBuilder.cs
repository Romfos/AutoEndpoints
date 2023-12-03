using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AutoEndpoints;

public sealed class GetRequestBuilder(WebApplication webApplication, string pattern)
{
    internal readonly WebApplication webApplication = webApplication;
    internal readonly string pattern = pattern;

    internal readonly List<KeyValuePair<int, Func<HttpContext, bool>>> verifications = new();

    public GetRequestBuilder Verify(int statusCode, Func<HttpContext, bool> condition)
    {
        verifications.Add(new(statusCode, condition));
        return this;
    }
}