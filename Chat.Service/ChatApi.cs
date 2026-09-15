namespace Chat.Service;

public static class ChatApi
{
    public static IEndpointRouteBuilder MapChat(this IEndpointRouteBuilder app)
    {
        app.MapPost("/chat", async (
            ChatRequest request,
            OllamaChatClient ollamaClient,
            CancellationToken cancellationToken) =>
        {
            if (request.Message is null or { Length: 0 })
            {
                return Results.Problem(
                    detail: "The 'message' field is required.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            try
            {
                var response = await ollamaClient.ChatAsync(request.Message, cancellationToken);
                return Results.Ok(new { response });
            }
            catch (OllamaChatException exception)
            {
                return Results.Problem(
                    detail: $"Ollama returned {(int)exception.StatusCode}: {exception.Message}",
                    statusCode: StatusCodes.Status502BadGateway);
            }
        });

        return app;
    }
}

public sealed record ChatRequest(string Message);