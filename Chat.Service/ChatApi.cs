using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
namespace Chat.Service;

public static class ChatApi
{
    public static IServiceCollection AddChatApi(this IServiceCollection services, IConfiguration configuration)
    {
        var ollamaOptions = configuration.GetSection("Ollama").Get<OllamaOptions>()
            ?? throw new InvalidOperationException("Ollama configuration is missing.");

        services.AddOllamaChatCompletion(
            modelId: ollamaOptions.Model,
            endpoint: new Uri(ollamaOptions.BaseUrl)
        );

        return services;
    }

    public record OllamaOptions(string BaseUrl, string Model);

    public static IEndpointRouteBuilder MapChat(this IEndpointRouteBuilder app)
    {
        app.MapPost("/chat", async (ChatRequest request, IChatCompletionService chatService, HttpContext httpContext) =>
        {
            httpContext.Response.ContentType = "text/event-stream";

            var history = new ChatHistory();
            history.AddSystemMessage(Prompts.SystemPrompt);
            
            foreach (var msg in request.Messages)
            {
                if (msg.Role == "user") history.AddUserMessage(msg.Content);
                else if (msg.Role == "assistant") history.AddAssistantMessage(msg.Content);
            }

            var responseChunks = chatService.GetStreamingChatMessageContentsAsync(history);

            await foreach (var chunk in responseChunks)
            {
                if (chunk.Content != null)
                {
                    await httpContext.Response.WriteAsync($"data: {chunk.Content}\n\n");
                    await httpContext.Response.Body.FlushAsync();
                }
            }
        });

        return app;
    }
}

public record ChatRequest(List<ChatMessage> Messages);

public record ChatMessage(string Role, string Content);