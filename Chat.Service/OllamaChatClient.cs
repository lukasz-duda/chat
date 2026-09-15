using System.Net;
using Microsoft.Extensions.Options;

namespace Chat.Service;

public sealed class OllamaChatClient
{
    private readonly HttpClient httpClient;
    private readonly OllamaOptions options;

    public OllamaChatClient(HttpClient httpClient, IOptions<OllamaOptions> options)
    {
        this.httpClient = httpClient;
        this.options = options.Value;
    }

    public async Task<string> ChatAsync(string message, CancellationToken cancellationToken)
    {
        var request = new OllamaChatRequest(
            options.Model,
            [new OllamaMessage("user", message)],
            false);

        using var response = await httpClient.PostAsJsonAsync("api/chat", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new OllamaChatException(response.StatusCode, error);
        }

        var chatResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken);
        return chatResponse?.Message.Content
            ?? throw new OllamaChatException(HttpStatusCode.BadGateway, "Ollama returned an empty response.");
    }
}

public sealed class OllamaChatException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public sealed class OllamaOptions
{
    public string BaseUrl { get; set; } = default!;
    public string Model { get; set; } = default!;
}

public sealed record OllamaChatRequest(
    string Model,
    IReadOnlyList<OllamaMessage> Messages,
    bool Stream);

public sealed record OllamaMessage(string Role, string Content);

public sealed record OllamaChatResponse(OllamaMessage Message);
