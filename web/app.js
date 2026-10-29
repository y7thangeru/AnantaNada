// ==========================================================================
// ANANTANADA x OHPE WORKSTATION COMPANION
// Live Web Remote & Mobile Stage Controller
// ==========================================================================

const activeMissionName = document.getElementById("activeMissionName");
const activePatchName = document.getElementById("activePatchName");
const currentChord = document.getElementById("currentChord");
const nextChord = document.getElementById("nextChord");
const bpmDisplay = document.getElementById("bpmDisplay");
const lcdBpm = document.getElementById("lcdBpm");
const lyricSubtitle = document.getElementById("lyricSubtitle");
const transposeTag = document.getElementById("transposeTag");
const btnTransDown = document.getElementById("btnTransDown");
const btnTransUp = document.getElementById("btnTransUp");
const btnPanic = document.getElementById("btnPanic");
const btnCapture = document.getElementById("btnCapture");
const btnLibrary = document.getElementById("btnLibrary");
const btnPro = document.getElementById("btnPro");
const btnTape = document.getElementById("btnTape");
const libraryModal = document.getElementById("libraryModal");
const btnLibCancel = document.getElementById("btnLibCancel");
const btnLibKeep = document.getElementById("btnLibKeep");
const libTableBody = document.getElementById("libTableBody");
const libSearchInput = document.getElementById("libSearchInput");
const btnPrevSong = document.getElementById("btnPrevSong");
const btnNextSong = document.getElementById("btnNextSong");

let currentTranspose = 0;
let currentSongIndex = 0;
let socket = null;
let reconnectInterval = null;

// Repertoire list
let songList = [
  { id: "SONG_001", title: "Dan", artist: "Sheila on 7", category: "INDO", patch: "Vintage EP 1", bpm: 100, chords: "G - Em - C - D" },
  { id: "SONG_002", title: "Kisah Klasik Untuk Masa Depan", artist: "Sheila on 7", category: "INDO", patch: "Bright Grand", bpm: 114, chords: "A - F#m - D - E" },
  { id: "SONG_003", title: "Melompat Lebih Tinggi", artist: "Sheila on 7", category: "INDO", patch: "Fat Saw Lead", bpm: 136, chords: "D - G - Bm - A" },
  { id: "SONG_004", title: "Sahabat Sejati", artist: "Sheila on 7", category: "INDO", patch: "Stage EP 2", bpm: 120, chords: "C - Am - F - G" },
  { id: "SONG_005", title: "Sephia", artist: "Sheila on 7", category: "INDO", patch: "Warm Pad", bpm: 88, chords: "Em - C - G - D" },
  { id: "SONG_006", title: "Berharap Tak Berpisah", artist: "Reza Artamevia", category: "INDO", patch: "Funk Clav", bpm: 116, chords: "Gm - Cm - F - Bb" },
  { id: "SONG_007", title: "Don't Look Back In Anger", artist: "Oasis", category: "WEST", patch: "Concert Grand", bpm: 84, chords: "C - G - Am - E" },
  { id: "SONG_008", title: "Wonderwall", artist: "Oasis", category: "WEST", patch: "Acoustic Strum", bpm: 88, chords: "Em7 - G - Dsus4 - A7sus4" },
  { id: "SONG_009", title: "Fix You", artist: "Coldplay", category: "WEST", patch: "Church Organ", bpm: 138, chords: "Eb - Gm - Cm - Bb" },
  { id: "SONG_010", title: "Clocks", artist: "Coldplay", category: "WEST", patch: "Grand Arp", bpm: 130, chords: "Eb - Bbm - Fm" }
];

function connectWebSocket() {
  const host = window.location.host || "127.0.0.1:8080";
  const protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
  const wsUrl = `${protocol}//${host}/ws`;

  try {
    socket = new WebSocket(wsUrl);

    socket.onopen = () => {
      console.log("[StageHub] Connected to Cockpit Host.");
      const linkTag = document.getElementById("linkTag");
      if (linkTag) {
        linkTag.textContent = "BLE MIDI [ONLINE]";
        linkTag.className = "telem-val green";
      }
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
          currentTranspose = msg.semitones || 0;
          updateTransposeUI();
        } else if (msg.type === "SECTION_CHANGED") {
          highlightSection(msg.section);
        }
      } catch (err) {
        console.error("Telemetry decode error:", err);
      }
    };

    socket.onclose = () => {
      const linkTag = document.getElementById("linkTag");
      if (linkTag) {
        linkTag.textContent = "DISCONNECTED";
        linkTag.className = "telem-val";
      }
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
  if (data.currentChord && currentChord) currentChord.textContent = data.currentChord;
  if (data.nextChord && nextChord) nextChord.textContent = data.nextChord;
  if (data.bpm !== undefined) {
    if (bpmDisplay) bpmDisplay.textContent = `${data.bpm.toFixed(1)} BPM`;
    if (lcdBpm) lcdBpm.textContent = `${data.bpm.toFixed(1)}`;
  }
  if (data.activePatch && activePatchName) activePatchName.textContent = data.activePatch;
  if (data.activeSong && activeMissionName) activeMissionName.textContent = data.activeSong;
}

function updateTransposeUI() {
  const transText = transposeTag ? transposeTag.querySelector(".trans-text") : null;
  if (transText) {
    transText.textContent = `TRANSPOSE: ${currentTranspose >= 0 ? "+" : ""}${currentTranspose}`;
  }
}

function highlightSection(secName) {
  document.querySelectorAll(".sec-chip").forEach((chip) => {
    if (chip.dataset.sec === secName) {
      chip.classList.add("active");
    } else {
      chip.classList.remove("active");
    }
  });
}

// Transpose buttons
if (btnTransDown) {
  btnTransDown.addEventListener("click", () => {
    currentTranspose = Math.max(-12, currentTranspose - 1);
    updateTransposeUI();
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.send(JSON.stringify({ type: "TRIGGER_TRANSPOSE", semitones: currentTranspose }));
    }
  });
}

if (btnTransUp) {
  btnTransUp.addEventListener("click", () => {
    currentTranspose = Math.min(12, currentTranspose + 1);
    updateTransposeUI();
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.send(JSON.stringify({ type: "TRIGGER_TRANSPOSE", semitones: currentTranspose }));
    }
  });
}

// Emergency Panic
if (btnPanic) {
  btnPanic.addEventListener("click", () => {
    alert("! EMERGENCY MIDI PANIC TRIGGERED ! (All notes & sound controllers killed)");
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.send(JSON.stringify({ type: "TRIGGER_PANIC" }));
    }
  });
}

// Retroactive Buffer capture
if (btnCapture) {
  btnCapture.addEventListener("click", () => {
    alert("Capturing 5-Minute Retroactive Circular RAM Buffer to MIDI File...");
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.send(JSON.stringify({ type: "TRIGGER_CAPTURE_BUFFER" }));
    }
  });
}

// Section navigation chips
document.querySelectorAll(".sec-chip").forEach((chip) => {
  chip.addEventListener("click", () => {
    highlightSection(chip.dataset.sec);
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.send(JSON.stringify({ type: "TRIGGER_SECTION", section: chip.dataset.sec }));
    }
  });
});

// Sound engine tiles selection
document.querySelectorAll(".left-rail .ohpe-tile").forEach((tile) => {
  tile.addEventListener("click", () => {
    document.querySelectorAll(".left-rail .ohpe-tile").forEach(t => t.classList.remove("active"));
    tile.classList.add("active");
    if (activePatchName) {
      activePatchName.textContent = `ROLAND ZEN-CORE / ${tile.dataset.cat}`;
    }
  });
});

// Mode switcher tiles selection
document.querySelectorAll(".right-rail .ohpe-tile").forEach((tile) => {
  tile.addEventListener("click", () => {
    document.querySelectorAll(".right-rail .ohpe-tile").forEach(t => t.classList.remove("active"));
    tile.classList.add("active");
    if (tile.dataset.mode === "LYRICS") {
      openLibrary();
    }
  });
});

// Repertoire navigation arrows
function loadSongByIndex(idx) {
  if (idx < 0) idx = songList.length - 1;
  if (idx >= songList.length) idx = 0;
  currentSongIndex = idx;
  const s = songList[currentSongIndex];
  if (activeMissionName) activeMissionName.textContent = `${String(currentSongIndex + 1).padStart(3, '0')}. ${s.title} - ${s.artist}`;
  if (activePatchName) activePatchName.textContent = `ROLAND ZEN-CORE / ${s.patch}`;
  if (bpmDisplay) bpmDisplay.textContent = `${s.bpm.toFixed(1)} BPM`;
  if (lcdBpm) lcdBpm.textContent = `${s.bpm.toFixed(1)}`;
  if (lyricSubtitle) lyricSubtitle.textContent = `"${s.title} - ${s.artist} // Intro: ${s.chords}"`;
  if (currentChord) currentChord.textContent = s.chords.split(" - ")[0] || "C";
  if (nextChord) nextChord.textContent = s.chords.split(" - ")[1] || "G";

  if (socket && socket.readyState === WebSocket.OPEN) {
    socket.send(JSON.stringify({ type: "TRIGGER_SETLIST_ITEM", songIndex: currentSongIndex }));
  }
}

if (btnPrevSong) btnPrevSong.addEventListener("click", () => loadSongByIndex(currentSongIndex - 1));
if (btnNextSong) btnNextSong.addEventListener("click", () => loadSongByIndex(currentSongIndex + 1));

// Library Modal
function openLibrary() {
  if (libraryModal) libraryModal.style.display = "flex";
  renderLibraryRows();
}

function closeLibrary() {
  if (libraryModal) libraryModal.style.display = "none";
}

if (btnLibrary) btnLibrary.addEventListener("click", openLibrary);
if (btnLibCancel) btnLibCancel.addEventListener("click", closeLibrary);
if (btnLibKeep) btnLibKeep.addEventListener("click", closeLibrary);

function renderLibraryRows(filterText = "") {
  if (!libTableBody) return;
  libTableBody.innerHTML = "";
  songList.forEach((s, idx) => {
    if (filterText && !s.title.toLowerCase().includes(filterText) && !s.artist.toLowerCase().includes(filterText)) {
      return;
    }
    const row = document.createElement("div");
    row.className = `lib-song-row ${idx === currentSongIndex ? "active" : ""}`;
    row.innerHTML = `
      <span>${idx === currentSongIndex ? "★" : "☆"}</span>
      <span><b>${s.title}</b> - ${s.artist}</span>
      <span style="color: #f59e0b;">${s.patch}</span>
      <span>${s.bpm}</span>
      <span>${s.category}</span>
    `;
    row.addEventListener("click", () => {
      document.querySelectorAll(".lib-song-row").forEach(r => r.classList.remove("active"));
      row.classList.add("active");
      loadSongByIndex(idx);
    });
    libTableBody.appendChild(row);
  });
}

if (libSearchInput) {
  libSearchInput.addEventListener("input", (e) => {
    renderLibraryRows(e.target.value.toLowerCase());
  });
}

// Physical keybed click feedback
document.querySelectorAll(".physical-keybed .key").forEach((k) => {
  k.addEventListener("mousedown", () => {
    k.style.filter = "brightness(0.85)";
  });
  k.addEventListener("mouseup", () => {
    k.style.filter = "none";
  });
});

// Initialize
connectWebSocket();
loadSongByIndex(0);
