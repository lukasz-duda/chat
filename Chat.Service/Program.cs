using Chat.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddChatApi(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Chat.Service");

app.MapChat();

app.Run();
