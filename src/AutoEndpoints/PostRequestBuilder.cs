using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AutoEndpoints;

public sealed class PostRequestBuilder(WebApplication webApplication, string pattern)
{
    internal readonly WebApplication webApplication = webApplication;
    internal readonly string pattern = pattern;

    internal readonly List<KeyValuePair<int, Func<HttpContext, bool>>> verifications = new();

    public PostRequestBuilder Verify(int statusCode, Func<HttpContext, bool> condition)
    {
        verifications.Add(new(statusCode, condition));
        return this;
    }
}

public sealed class PostRequestBuilder<T>(WebApplication webApplication, string pattern)
{
    internal readonly WebApplication webApplication = webApplication;
    internal readonly string pattern = pattern;

    internal readonly List<KeyValuePair<int, Func<HttpContext, bool>>> verifications = new();
    internal readonly List<KeyValuePair<int, Func<HttpContext, T, bool>>> validations = new();

    public PostRequestBuilder<T> Verify(int statusCode, Func<HttpContext, bool> condition)
    {
        verifications.Add(new(statusCode, condition));
        return this;
    }

    public PostRequestBuilder<T> Validate(int statusCode, Func<HttpContext, T, bool> condition)
    {
        validations.Add(new(statusCode, condition));
        return this;
    }
}
