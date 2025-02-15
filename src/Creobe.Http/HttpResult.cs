using System.Net;

namespace Creobe.Http;

public record HttpResult
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; } = [];
    public string? ErrorMessage { get; set; }
}

public record HttpResult<TResult> : HttpResult
{
    public TResult? Value { get; set; }
}

