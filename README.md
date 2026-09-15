# Chat

Requirements:

- [Docker](https://docs.docker.com/engine/install/ubuntu/)
- [.NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

Start Ollama:

```bash
docker compose up -d
```

Pull model:

```bash
docker exec -it ollama ollama pull llama3.2:3b
```

Start service:

```bash
cd Chat.Service
dotnet run
```
