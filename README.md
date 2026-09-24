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

Test prompts:

| Test          | Prompt                                    | Expected         |
| ------------- | ----------------------------------------- | ---------------- |
| System Prompt | Whats your name?                          | Adam             |
| Function Call | Is it good weather for running in Warsaw? | 15°C, Light Rain |

Read more:

- [IChatClient interface](https://learn.microsoft.com/en-us/dotnet/ai/ichatclient)
- [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/get-started/quick-start-guide?toc=%2Fsemantic-kernel%2Ftoc.json&pivots=programming-language-csharp)
- [The Agent–User Interaction (AG-UI) Protocol](https://docs.ag-ui.com/introduction)
- [Model Context Protocol (MCP)](https://modelcontextprotocol.io/docs/2026-07-28/getting-started/intro)
