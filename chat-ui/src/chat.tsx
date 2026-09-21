import { useState, type SubmitEvent } from "react";

export function Chat() {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: SubmitEvent) => {
    e.preventDefault();
    if (!input.trim() || loading) return;

    const userMessage: Message = { role: "user", content: input };
    const updatedMessages = [...messages, userMessage];

    setMessages(updatedMessages);
    setInput("");
    setLoading(true);

    setMessages((prev) => [...prev, { role: "assistant", content: "" }]);

    try {
      const response = await fetch("http://localhost:8080/chat", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ messages: updatedMessages }),
      });

      if (!response.body) return;

      const reader = response.body.getReader();
      const decoder = new TextDecoder();

      while (true) {
        const { done, value } = await reader.read();
        if (done) break;

        const chunk = decoder.decode(value, { stream: true });

        const lines = chunk.split("\n");
        for (const line of lines) {
          if (line.startsWith("data: ")) {
            const textToken = line.replace("data: ", "");

            setMessages((prev) => {
              const last = prev[prev.length - 1];
              return [
                ...prev.slice(0, -1),
                { ...last, content: last.content + textToken },
              ];
            });
          }
        }
      }
    } catch (error: unknown) {
      setError(error instanceof Error ? error.message : "Nieznany błąd");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="chat-container">
      <div className="message-container">
        {messages.map((m, idx) => (
          <div key={idx} className={`${m.role}-message`}>
            <strong>{m.role === "user" ? "Ty: " : "AI: "}</strong>
            <span>{m.content}</span>
          </div>
        ))}
        {loading && (
          <div className="loading-message">
            <em>Przetwarzanie...</em>
          </div>
        )}
        {error && (
          <div className="error-message">
            <strong>Błąd: </strong> {error}
          </div>
        )}
      </div>

      <form onSubmit={handleSubmit} className="new-message-form">
        <input
          className="new-message-input"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Napisz wiadomość..."
        />
        <button type="submit" disabled={loading} className="send-button">
          Wyślij
        </button>
      </form>
    </div>
  );
}

interface Message {
  role: "user" | "assistant";
  content: string;
}
