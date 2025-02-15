namespace Creobe.Http;

public record HttpRequestError
{
    public string Title { get; set; } = default!;
    public Dictionary<string, List<string>>? Errors { get; set; }
    public string? Detail { get; set; }
}
