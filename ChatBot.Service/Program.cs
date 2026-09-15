using ChatBot.Service;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));
builder.Services.AddHttpClient<OllamaChatClient>((serviceProvider, client) =>
{
    OllamaOptions options = serviceProvider.GetRequiredService<IOptions<OllamaOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

var app = builder.Build();

app.MapGet("/", () => "ChatBot.Service");

app.MapChatBot();

app.Run();
