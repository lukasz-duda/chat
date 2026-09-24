export interface ChatMessage {
  role: "user" | "assistant";
  content: string;
}

export async function* streamChat(
  messages: ChatMessage[],
): AsyncGenerator<string> {
  const response = await fetch(`${import.meta.env.VITE_CHAT_API_URL}/chat`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ messages }),
  });

  if (!response.body) return;

  const reader = response.body.getReader();
  const decoder = new TextDecoder();
  let trailingPartialLine = "";

  try {
    while (true) {
      const { done, value } = await reader.read();
      const decodedChunk = decoder.decode(value, { stream: !done });
      const combinedText = trailingPartialLine + decodedChunk;

      const lines = combinedText.split("\n");
      trailingPartialLine = lines.pop() ?? "";

      for (const line of lines) {
        const content = getContent(line);
        if (content !== null) yield content;
      }

      if (done) {
        const content = getContent(trailingPartialLine);
        if (content !== null) yield content;
        break;
      }
    }
  } finally {
    reader.releaseLock();
  }
}

function getContent(line: string): string | null {
  const normalizedLine = line.endsWith("\r") ? line.slice(0, -1) : line;
  return normalizedLine.startsWith("data: ")
    ? normalizedLine.slice("data: ".length)
    : null;
}
