using Microsoft.Extensions.DependencyInjection;

namespace Creobe.Http;

public static class DependencyInjection
{
    public static IHttpClientBuilder AddHttpRequestClient(this IServiceCollection services, string baseUrl)
    {
        return services.AddHttpClient<IHttpRequestClient, HttpRequestClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });
    }
}