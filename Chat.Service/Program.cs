using Chat.Service;

var builder = WebApplication.CreateBuilder(args);

var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? throw new InvalidOperationException("Cors:AllowedOrigins configuration section is missing or empty.");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

await builder.Services.AddChatApiAsync(builder.Configuration);

var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Chat.Service");

app.MapChat();

app.Run();
