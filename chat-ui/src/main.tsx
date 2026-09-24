import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { Chat } from "./chat.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <Chat />
  </StrictMode>,
);
