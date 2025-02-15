using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace Creobe.Http;

public class HttpRequestClientBuilder<TResult>
{
    private readonly HttpMethod _method;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, string?> _queryParameters = [];

    private string _endpoint = string.Empty;
    private HttpContent? _content;

    private bool _isEndpointSet;
    private bool _isContentSet;
    private bool _isPaginationSet;
    private bool _isSkipSet;
    private bool _isLimitSet;

    public HttpRequestClientBuilder(HttpMethod method, HttpClient client)
    {
        _method = method;
        _client = client;

        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public HttpRequestClientBuilder<TResult> ToEndpoint(string endpoint)
    {
        if (_isEndpointSet)
            throw new InvalidOperationException("Endpoint can only be set once.");

        _endpoint = endpoint;
        _isEndpointSet = true;
        return this;
    }

    public HttpRequestClientBuilder<TResult> ToEndpoint(string endpoint, object? key)
    {
        if (_isEndpointSet)
            throw new InvalidOperationException("Endpoint can only be set once.");

        _endpoint = $"{endpoint}/{key}";
        _isEndpointSet = true;
        return this;
    }

    public HttpRequestClientBuilder<TResult> WithContent<TContent>(TContent content)
    {
        if (_isContentSet)
            throw new InvalidOperationException("Content can only be set once.");

        var json = JsonSerializer.Serialize(content, _jsonOptions);
        _content = new StringContent(json, Encoding.UTF8, "application/json");

        _isContentSet = true;
        return this;
    }

    public HttpRequestClientBuilder<TResult> WithFile(FileStream stream, string fileName, string contentType)
    {
        if (_isContentSet)
            throw new InvalidOperationException("Content can only be set once.");

        var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        content.Add(fileContent, "\"file\"", fileName);

        _content = content;
        _isContentSet = true;
        return this;
    }

    public HttpRequestClientBuilder<TResult> WithParameter(string name, object? value)
    {
        _queryParameters.TryAdd(name, value?.ToString());
        return this;
    }

    public HttpRequestClientBuilder<TResult> WithParameters(Dictionary<string, object?> parameters)
    {
        foreach (var (name, value) in parameters)
        {
            _queryParameters.TryAdd(name, value?.ToString());
        }

        return this;
    }

    public HttpRequestClientBuilder<TResult> WithHeader(string name, string value)
    {
        _ = _client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        return this;
    }

    public HttpRequestClientBuilder<TResult> WithHeaders(Dictionary<string, string> headers)
    {
        foreach (var (name, value) in headers)
        {
            _ = _client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        return this;
    }

    public HttpRequestClientBuilder<TResult> WithPagination(int pageNumber, int pageSize)
    {
        if (_isPaginationSet)
            throw new InvalidOperationException("Pagination can only be set once.");

        _queryParameters.TryAdd(nameof(pageNumber), pageNumber.ToString());
        _queryParameters.TryAdd(nameof(pageSize), pageSize.ToString());

        _isPaginationSet = true;

        return this;
    }

    public HttpRequestClientBuilder<TResult> Skip(int skip)
    {
        if (_isSkipSet)
            throw new InvalidOperationException("Skip can only be set once.");

        _queryParameters.TryAdd(nameof(skip), skip.ToString());

        _isSkipSet = true;

        return this;
    }

    public HttpRequestClientBuilder<TResult> Limit(int limit)
    {
        if (_isLimitSet)
            throw new InvalidOperationException("Limit can only be set once.");

        _queryParameters.TryAdd(nameof(limit), limit.ToString());

        _isLimitSet = true;

        return this;
    }

    public async Task<HttpResult<TResult>> SendAsync()
    {
        var endpointWithQuery = QueryHelpers.AddQueryString(_endpoint, _queryParameters);

        var request = new HttpRequestMessage(_method, endpointWithQuery);

        if (_content != null)
            request.Content = _content;

        var response = await _client.SendAsync(request);

        var httpRepositoryResult = new HttpResult<TResult>
        {
            IsSuccess = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode
        };

        if (response.IsSuccessStatusCode)
        {
            if (typeof(TResult) == typeof(MemoryStream))
            {
                var memoryStream = new MemoryStream();
                await response.Content.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                httpRepositoryResult.Value = (TResult)(object)memoryStream;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                    return httpRepositoryResult;

                httpRepositoryResult.Value = await response.Content.ReadFromJsonAsync<TResult>(_jsonOptions);
            }
        }
        else if(response.StatusCode == HttpStatusCode.BadRequest)
        {
            try
            {
                var validationError = await response.Content.ReadFromJsonAsync<HttpRequestError>(_jsonOptions);
                httpRepositoryResult.Errors = validationError?.Errors ?? [];
                httpRepositoryResult.ErrorMessage = validationError?.Detail;
            }
            catch (JsonException)
            {
                var error = await response.Content.ReadAsStringAsync();
                httpRepositoryResult.ErrorMessage = error;
            }
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            httpRepositoryResult.ErrorMessage = error;
        }

        return httpRepositoryResult;
    }
}

public class HttpRequestClientBuilder(HttpMethod method, HttpClient httpClient) 
    : HttpRequestClientBuilder<object>(method, httpClient);