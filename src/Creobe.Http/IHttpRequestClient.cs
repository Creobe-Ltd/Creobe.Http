namespace Creobe.Http;

public interface IHttpRequestClient
{
    HttpRequestClientBuilder<MemoryStream> GetFile();
    HttpRequestClientBuilder<TResult> Get<TResult>();
    HttpRequestClientBuilder<TResult> Post<TResult>();
    HttpRequestClientBuilder Post();
    HttpRequestClientBuilder<TResult> Put<TResult>();
    HttpRequestClientBuilder Put();
    HttpRequestClientBuilder<TResult> Patch<TResult>();
    HttpRequestClientBuilder Patch();
    HttpRequestClientBuilder<TResult> Delete<TResult>();
    HttpRequestClientBuilder Delete();
}
