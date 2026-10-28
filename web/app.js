// --- ANANTANADA STAGEHUB COMPANION CLIENT ---
// Adheres strictly to Section 5 WebSocket Data Contracts in ANANTANADA_SPEC.md

const connectionDot = document.getElementById("connectionDot");
const connectionText = document.getElementById("connectionText");
const songMeta = document.getElementById("songMeta");
const currentChord = document.getElementById("currentChord");
const nextChord = document.getElementById("nextChord");
const barDisplay = document.getElementById("barDisplay");
const bpmDisplay = document.getElementById("bpmDisplay");
const patchDisplay = document.getElementById("patchDisplay");
const panicBtn = document.getElementById("panicBtn");
const setlistGrid = document.getElementById("setlistGrid");
const beatDots = [
  document.getElementById("beat1"),
  document.getElementById("beat2"),
  document.getElementById("beat3"),
  document.getElementById("beat4")
];

let socket = null;
let reconnectInterval = null;

function connectWebSocket() {
  const host = window.location.host || "127.0.0.1:8080";
  const protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
  const wsUrl = `${protocol}//${host}/ws`;

  connectionText.textContent = "CONNECTING...";
  connectionDot.className = "pulse-dot";

  try {
    socket = new WebSocket(wsUrl);

    socket.onopen = () => {
      connectionDot.className = "pulse-dot online";
      connectionText.textContent = "LINKED // STAGE HUB";
      if (reconnectInterval) {
        clearInterval(reconnectInterval);
        reconnectInterval = null;
      }
    };

    socket.onmessage = (event) => {
      try {
        const msg = JSON.parse(event.data);
        if (msg.type === "PLAYBACK_TICK" && msg.data) {
          handlePlaybackTick(msg.data);
        }
      } catch (err) {
        console.error("Telemetry decode error:", err);
      }
    };

    socket.onerror = () => {
      connectionDot.className = "pulse-dot error";
      connectionText.textContent = "OFFLINE // RETRYING";
    };

    socket.onclose = () => {
      connectionDot.className = "pulse-dot error";
      connectionText.textContent = "DISCONNECTED";
      if (!reconnectInterval) {
        reconnectInterval = setInterval(connectWebSocket, 2000);
      }
    };
  } catch (err) {
    connectionDot.className = "pulse-dot error";
    connectionText.textContent = "CONN ERR";
    if (!reconnectInterval) {
      reconnectInterval = setInterval(connectWebSocket, 2000);
    }
  }
}

// Inbound Telemetry Handler: PLAYBACK_TICK
function handlePlaybackTick(data) {
  if (data.currentChord) currentChord.textContent = data.currentChord;
  if (data.nextChord) nextChord.textContent = data.nextChord;
  if (data.bar !== undefined) barDisplay.textContent = `BAR ${String(data.bar).padStart(3, '0')}`;
  if (data.bpm !== undefined) bpmDisplay.textContent = `${data.bpm} BPM`;
  if (data.activePatch) patchDisplay.textContent = `PATCH: ${data.activePatch}`;
  if (data.songId) songMeta.textContent = `ID: ${data.songId.toUpperCase()}`;

  // Update 4-beat visual metronome dots
  if (data.beat !== undefined) {
    beatDots.forEach((dot, idx) => {
      if (idx + 1 === data.beat) {
        dot.classList.add("active");
      } else {
        dot.classList.remove("active");
      }
    });
  }
}

// Outbound Command: TRIGGER_PANIC
panicBtn.addEventListener("click", () => {
  if (socket && socket.readyState === WebSocket.OPEN) {
    const payload = {
      command: "TRIGGER_PANIC"
    };
    socket.send(JSON.stringify(payload));
  }
  // Visual tactile feedback
  panicBtn.style.transform = "scale(0.92)";
  setTimeout(() => { panicBtn.style.transform = ""; }, 150);
});

// Outbound Command: TRIGGER_SETLIST_ITEM
setlistGrid.addEventListener("click", (e) => {
  const card = e.target.closest(".song-card");
  if (!card) return;

  const songIndex = parseInt(card.dataset.index, 10);
  const bankMsb = parseInt(card.dataset.msb, 10);
  const bankLsb = parseInt(card.dataset.lsb, 10);
  const programChange = parseInt(card.dataset.pc, 10);

  // Update UI active card
  document.querySelectorAll(".song-card").forEach(c => c.classList.remove("active"));
  card.classList.add("active");

  if (socket && socket.readyState === WebSocket.OPEN) {
    const payload = {
      command: "TRIGGER_SETLIST_ITEM",
      payload: {
        songIndex: songIndex,
        programChange: programChange,
        bankMsb: bankMsb,
        bankLsb: bankLsb,
        transpose: 0
      }
    };
    socket.send(JSON.stringify(payload));
  }
});

// Auto-connect on load
window.addEventListener("DOMContentLoaded", connectWebSocket);
