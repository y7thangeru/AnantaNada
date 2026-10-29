// --- ANANTANADA TACTICAL COCKPIT REMOTE ---
// WebSocket telemetry & kinetic feedback

const panicBtn = document.getElementById("panicBtn");
const retroBtn = document.getElementById("retroBtn");
const setlistGrid = document.getElementById("setlistGrid");
const activeMissionName = document.getElementById("activeMissionName");
const activePatchName = document.getElementById("activePatchName");
const currentChord = document.getElementById("currentChord");
const nextChord = document.getElementById("nextChord");
const bpmTag = document.getElementById("bpmTag");
const lyricBox = document.getElementById("lyricBox");
const timingVerdict = document.getElementById("timingVerdict");
const scoreAcc = document.getElementById("scoreAcc");
const strikeGauge = document.getElementById("strikeGauge");
const rankBadge = document.getElementById("rankBadge");
const cadenceHeart = document.getElementById("cadenceHeart");
const hifiBurst = document.getElementById("hifiBurst");
const djmaxBadge = document.getElementById("djmaxBadge");
const djmaxFever = document.getElementById("djmaxFever");
const taikoLabel = document.getElementById("taikoLabel");
const beatSteps = [
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

  try {
    socket = new WebSocket(wsUrl);

    socket.onopen = () => {
      console.log("[StageHub] Connected to Cockpit Host.");
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
        } else if (msg.type === "TRANSPOSE_CHANGED") {
          currentWebTranspose = msg.semitones || 0;
          if (transposeTag) {
            transposeTag.textContent = `TRANSPOSE: [ ${currentWebTranspose >= 0 ? "+" : ""}${currentWebTranspose} ]`;
            transposeTag.style.color = currentWebTranspose === 0 ? "#ffd166" : "#00f5d4";
          }
        } else if (msg.type === "SECTION_CHANGED") {
          highlightSectionBtn(msg.section);
        }
      } catch (err) {
        console.error("Telemetry decode error:", err);
      }
    };

    socket.onclose = () => {
      if (!reconnectInterval) {
        reconnectInterval = setInterval(connectWebSocket, 2000);
      }
    };
  } catch (err) {
    if (!reconnectInterval) {
      reconnectInterval = setInterval(connectWebSocket, 2000);
    }
  }
}

function handlePlaybackTick(data) {
  if (data.currentChord) currentChord.textContent = `[ ${data.currentChord} ]`;
  if (data.nextChord) nextChord.textContent = `NEXT CHORD -> ${data.nextChord}`;
  if (data.bpm !== undefined && data.bar !== undefined && data.beat !== undefined) {
    bpmTag.textContent = `[BPM: ${data.bpm} /// 4/4] [BAR: ${String(data.bar).padStart(3, '0')} . BEAT: ${data.beat}/4]`;
  }
  if (data.activePatch) activePatchName.textContent = data.activePatch;

  // Beat stepper animation & 12-Game Kinetic Pulse
  if (data.beat !== undefined) {
    if (cadenceHeart) {
      cadenceHeart.classList.add("pulse");
      setTimeout(() => cadenceHeart.classList.remove("pulse"), 130);
    }
    if (hifiBurst && data.beat === 1) {
      hifiBurst.classList.add("hit");
      setTimeout(() => hifiBurst.classList.remove("hit"), 150);
    }

    beatSteps.forEach((step, idx) => {
      if (idx + 1 === data.beat) {
        step.classList.add("active");
      } else {
        step.classList.remove("active");
      }
    });
  }
}

// Emergency Panic Purge
panicBtn.addEventListener("click", () => {
  if (socket && socket.readyState === WebSocket.OPEN) {
    socket.send(JSON.stringify({ command: "TRIGGER_PANIC" }));
  }

  // Red glitch flash feedback
  document.body.style.filter = "invert(1) hue-rotate(180deg)";
  timingVerdict.textContent = "ACCURACY: [ ALL CHANNELS PURGED ]";
  timingVerdict.style.color = "#ef233c";
  setTimeout(() => { document.body.style.filter = ""; }, 150);
});

// Setlist Selection
setlistGrid.addEventListener("click", (e) => {
  const card = e.target.closest(".mission-card");
  if (!card) return;

  const songIndex = parseInt(card.dataset.index, 10);
  const bankMsb = parseInt(card.dataset.msb, 10);
  const bankLsb = parseInt(card.dataset.lsb, 10);
  const programChange = parseInt(card.dataset.pc, 10);

  activeMissionName.textContent = card.querySelector(".mission-title").textContent;
  activePatchName.textContent = card.querySelector(".mission-patch").textContent;

  if (socket && socket.readyState === WebSocket.OPEN) {
    socket.send(JSON.stringify({
      command: "TRIGGER_SETLIST_ITEM",
      payload: {
        songIndex: songIndex,
        programChange: programChange,
        bankMsb: bankMsb,
        bankLsb: bankLsb,
        transpose: 0
      }
    }));
  }
});

// Retroactive Buffer 1-Click
retroBtn.addEventListener("click", () => {
  retroBtn.textContent = "RETRO-BUFFER: [ 5m SMF EXPORTED! ]";
  retroBtn.style.borderColor = "#00f5d4";
  retroBtn.style.color = "#00f5d4";
  setTimeout(() => {
    retroBtn.textContent = "RETRO-BUFFER: [ 5m CAPTURE ARMED ]";
    retroBtn.style.borderColor = "";
    retroBtn.style.color = "";
  }, 2000);
});

let currentWebTranspose = 0;

// Interactive Web Transpose Controls
const btnWebTransposeDown = document.getElementById("btnWebTransposeDown");
const btnWebTransposeUp = document.getElementById("btnWebTransposeUp");

if (btnWebTransposeDown) {
  btnWebTransposeDown.addEventListener("click", () => {
    currentWebTranspose = Math.max(-12, currentWebTranspose - 1);
    sendTranspose(currentWebTranspose);
  });
}

if (btnWebTransposeUp) {
  btnWebTransposeUp.addEventListener("click", () => {
    currentWebTranspose = Math.min(12, currentWebTranspose + 1);
    sendTranspose(currentWebTranspose);
  });
}

function sendTranspose(semitones) {
  if (transposeTag) {
    transposeTag.textContent = `TRANSPOSE: [ ${semitones >= 0 ? "+" : ""}${semitones} ]`;
    transposeTag.style.color = semitones === 0 ? "#ffd166" : "#00f5d4";
  }
  if (socket && socket.readyState === WebSocket.OPEN) {
    socket.send(JSON.stringify({
      command: "TRIGGER_TRANSPOSE",
      payload: { semitones: semitones }
    }));
  }
}

// Interactive Web Section Buttons
const sectionBtns = document.querySelectorAll(".section-btn");
sectionBtns.forEach(btn => {
  btn.addEventListener("click", () => {
    const sec = btn.dataset.sec;
    const bar = parseInt(btn.dataset.bar, 10);
    highlightSectionBtn(sec);
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.send(JSON.stringify({
        command: "TRIGGER_SECTION",
        payload: { section: sec, bar: bar }
      }));
    }
  });
});

function highlightSectionBtn(secName) {
  sectionBtns.forEach(btn => {
    if (btn.dataset.sec === secName) {
      btn.style.background = "#d4af37";
      btn.style.color = "#1a120b";
      btn.style.borderColor = "#fff";
      btn.style.fontWeight = "bold";
    } else {
      btn.style.background = "#2b1f14";
      btn.style.color = "#fff";
      btn.style.borderColor = "#c8a86b";
      btn.style.fontWeight = "normal";
    }
  });
}

window.addEventListener("DOMContentLoaded", connectWebSocket);
