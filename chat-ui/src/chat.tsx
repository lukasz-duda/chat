import { useState, type SubmitEvent } from "react";

import { streamChat, type ChatMessage } from "./chat-api";
import "./chat.css";

export function Chat() {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: SubmitEvent) => {
    e.preventDefault();
    if (!input.trim() || loading) return;

    const userMessage: ChatMessage = { role: "user", content: input };
    const updatedMessages = [...messages, userMessage];

    setMessages(updatedMessages);
    setInput("");
    setLoading(true);

    setMessages((prev) => [...prev, { role: "assistant", content: "" }]);

    try {
      for await (const textToken of streamChat(updatedMessages)) {
        setMessages((prev) => {
          const last = prev[prev.length - 1];
          return [
            ...prev.slice(0, -1),
            { ...last, content: last.content + textToken },
          ];
        });
      }
    } catch (error: unknown) {
      setError(error instanceof Error ? error.message : "Unknown error");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="chat-container">
      <div className="messages-container">
        {messages.map((m, idx) => (
          <div key={idx} className={`${m.role}-message`}>
            <span>{m.content}</span>
          </div>
        ))}
        {loading ? (
          <div className="loading-message">Processing...</div>
        ) : (
          error && <div className="error-message">{error}</div>
        )}
      </div>

      <form onSubmit={handleSubmit} className="new-message-form">
        <input
          className="new-message-input"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Write a message..."
          autoFocus={true}
        />
        <button type="submit" disabled={loading} className="send-button">
          Send
        </button>
      </form>
    </div>
  );
}
