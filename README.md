# Creobe.Http

A fluent HTTP client library for .NET that simplifies making HTTP requests.

## Installation

Install via NuGet:

```shell
dotnet add package Creobe.Http
```
## Features

- Fluent builder API for HTTP requests
- Support for common HTTP methods (GET, POST, PUT, DELETE)
- File upload support
- Query parameter handling
- Pagination support
- Error handling
- Strongly typed responses

## Usage

### Setup

Register the HTTP client in your DI container:

```csharp
services.AddHttpRequestClient("https://api.example.com");
```

### Making Requests

```csharp
private readonly IHttpRequestClient _client;

public MyService(IHttpRequestClient client)
{
    _client = client;
}
```

### GET Request

```csharp
// Simple GET
var response = await _client.Get<MyResponse>()
    .ToEndpoint("api/items")
    .SendAsync();

// GET with query parameters
var response = await _client.Get<MyResponse>()
    .ToEndpoint("api/items")
    .WithParameter("category", "books")
    .WithParameter("sort", "name")
    .SendAsync();
```

### POST Request

```csharp
var response = await _client.Post<MyResponse>()
    .ToEndpoint("api/items")
    .WithContent(new { Name = "Book", Price = 9.99 })
    .SendAsync();
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.