# Chat

Requirements:

- [Docker](https://docs.docker.com/engine/install/ubuntu/)
- [.NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Node.js v20](https://nodejs.org/en)

Start Ollama:

```bash
docker compose up -d
```

Pull model:

```bash
docker exec -it ollama ollama pull llama3.2:3b
```

Start chat service:

```bash
cd Chat.Service
dotnet run
```

Start chat ui:

```bash
cd chat-ui
npm i
npm run dev
```
