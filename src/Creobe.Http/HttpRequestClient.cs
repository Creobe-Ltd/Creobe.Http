namespace Creobe.Http;

public class HttpRequestClient(HttpClient httpClient) : IHttpRequestClient
{
    public HttpRequestClientBuilder<MemoryStream> GetFile()
    {
        return new HttpRequestClientBuilder<MemoryStream>(HttpMethod.Get, httpClient);
    }

    public HttpRequestClientBuilder<TResult> Get<TResult>()
    {
        return new HttpRequestClientBuilder<TResult>(HttpMethod.Get, httpClient);
    }

    public HttpRequestClientBuilder<TResult> Post<TResult>()
    {
        return new HttpRequestClientBuilder<TResult>(HttpMethod.Post, httpClient);
    }

    public HttpRequestClientBuilder Post()
    {
        return new HttpRequestClientBuilder(HttpMethod.Post, httpClient);
    }

    public HttpRequestClientBuilder<TResult> Put<TResult>()
    {
        return new HttpRequestClientBuilder<TResult>(HttpMethod.Put, httpClient);
    }

    public HttpRequestClientBuilder Put()
    {
        return new HttpRequestClientBuilder(HttpMethod.Put, httpClient);
    }

    public HttpRequestClientBuilder<TResult> Patch<TResult>()
    {
        return new HttpRequestClientBuilder<TResult>(HttpMethod.Patch, httpClient);
    }
    
    public HttpRequestClientBuilder Patch()
    {
        return new HttpRequestClientBuilder(HttpMethod.Patch, httpClient);
    }

    public HttpRequestClientBuilder<TResult> Delete<TResult>()
    {
        return new HttpRequestClientBuilder<TResult>(HttpMethod.Delete, httpClient);
    }

    public HttpRequestClientBuilder Delete()
    {
        return new HttpRequestClientBuilder(HttpMethod.Delete, httpClient);
    }
}