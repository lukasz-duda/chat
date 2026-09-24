using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using ModelContextProtocol.Client;

namespace Chat.Service;

public static class ChatApi
{
    public static async Task<IServiceCollection> AddChatApiAsync(this IServiceCollection services, IConfiguration configuration)
    {
        var ollamaOptions = configuration.GetSection("Ollama").Get<OllamaOptions>()
            ?? throw new InvalidOperationException("Ollama configuration is missing.");

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOllamaChatCompletion(
            modelId: ollamaOptions.Model,
            endpoint: new Uri(ollamaOptions.BaseUrl)
        );

        Kernel kernel = kernelBuilder.Build();


        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = new Uri("http://localhost:5002"),
                TransportMode = HttpTransportMode.StreamableHttp
            });

        await using var mcpClient = await McpClient.CreateAsync(transport);

        var toolsResult = await mcpClient.ListToolsAsync();

        kernel.Plugins.AddFromFunctions(
            pluginName: "ExchangeRateTools",
            functions: toolsResult.Select(tool => tool.AsKernelFunction()));

        kernel.Plugins.AddFromType<WeatherPlugin>();

        services.AddSingleton(kernel);
        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        services.AddSingleton(chatService);

        return services;
    }

    public record OllamaOptions(string BaseUrl, string Model);

    public static IEndpointRouteBuilder MapChat(this IEndpointRouteBuilder app)
    {
        app.MapPost("/chat", async (ChatRequest request, IChatCompletionService chatService, Kernel kernel, HttpContext httpContext) =>
        {
            httpContext.Response.ContentType = "text/event-stream";

            var history = new ChatHistory();
            history.AddSystemMessage(Prompts.SystemPrompt);

            foreach (var msg in request.Messages)
            {
                if (msg.Role == "user") history.AddUserMessage(msg.Content);
                else if (msg.Role == "assistant") history.AddAssistantMessage(msg.Content);
            }

            var responseChunks = chatService.GetStreamingChatMessageContentsAsync(
                history,
                executionSettings: new OllamaPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                }, kernel: kernel);

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