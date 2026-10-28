extends Control

# AnantaNada Master Stage View Controller (Godot 4.x Forward+ / Vulkan)
# Connects to embedded C# StageHub engine via WebSocket (ws://127.0.0.1:8080/ws)

const HTTP_PORT: int = 8080
const WS_URL: String = "ws://127.0.0.1:8080/ws"
const LOWEST_NOTE: int = 36 # C2
const HIGHEST_NOTE: int = 96 # C7

# Nodes
@onready var hud_container: Control = $HUD
@onready var lbl_device_status: Label = $HUD/TopBar/DevicePill/StatusLabel
@onready var lbl_ip_address: Label = $HUD/TopBar/NetworkPill/IpLabel
@onready var lbl_bpm_beat: Label = $HUD/TopBar/BpmPill/BpmLabel
@onready var btn_panic: Button = $HUD/TopBar/PanicButtonContainer/PanicButton

@onready var search_box: LineEdit = $HUD/CenterSplit/SetlistDeck/MissionContainer/SearchBox
@onready var btn_filter_all: Button = $HUD/CenterSplit/SetlistDeck/MissionContainer/FilterRow/BtnFilterAll
@onready var btn_filter_indo: Button = $HUD/CenterSplit/SetlistDeck/MissionContainer/FilterRow/BtnFilterIndo
@onready var btn_filter_western: Button = $HUD/CenterSplit/SetlistDeck/MissionContainer/FilterRow/BtnFilterWestern
@onready var active_card: PanelContainer = $HUD/CenterSplit/SetlistDeck/MissionContainer/ActiveCard
@onready var lbl_active_mission: Label = $HUD/CenterSplit/SetlistDeck/MissionContainer/ActiveCard/MissionName

@onready var next_card_1: PanelContainer = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard1
@onready var next_card_2: PanelContainer = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard2
@onready var next_card_3: PanelContainer = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard3
@onready var next_card_4: PanelContainer = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard4
@onready var next_card_5: PanelContainer = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard5
@onready var next_card_6: PanelContainer = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard6
@onready var lbl_card_1: Label = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard1/Label
@onready var lbl_card_2: Label = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard2/Label
@onready var lbl_card_3: Label = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard3/Label
@onready var lbl_card_4: Label = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard4/Label
@onready var lbl_card_5: Label = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard5/Label
@onready var lbl_card_6: Label = $HUD/CenterSplit/SetlistDeck/MissionContainer/NextCard6/Label

@onready var btn_prev_page: Button = $HUD/CenterSplit/SetlistDeck/MissionContainer/PaginationRow/BtnPrevPage
@onready var btn_next_page: Button = $HUD/CenterSplit/SetlistDeck/MissionContainer/PaginationRow/BtnNextPage
@onready var lbl_page_indicator: Label = $HUD/CenterSplit/SetlistDeck/MissionContainer/PaginationRow/PageIndicator

@onready var waterfall_rect: ColorRect = $HUD/CenterSplit/CenterCascade/WaterfallRect
@onready var lbl_rhythm_accuracy: Label = $HUD/CenterSplit/CenterCascade/CoachDeck/AccuracyValue
@onready var lbl_timing_verdict: Label = $HUD/CenterSplit/CenterCascade/CoachDeck/TimingVerdict
@onready var lbl_strike_gauge: Label = $HUD/CenterSplit/CenterCascade/CoachDeck/StrikeGauge
@onready var lbl_velocity_rank: Label = $HUD/CenterSplit/CenterCascade/CoachDeck/RankBadge
@onready var btn_never_lost: Button = $HUD/CenterSplit/CenterCascade/CoachDeck/RetroBufferBtn

@onready var current_chord_box: PanelContainer = $HUD/CenterSplit/ChordHud/CurrentChordBox
@onready var lbl_current_chord: Label = $HUD/CenterSplit/ChordHud/CurrentChordBox/VBox/ChordLabel
@onready var lbl_transpose: Label = $HUD/CenterSplit/ChordHud/CurrentChordBox/VBox/TransposePill
@onready var next_chord_box: PanelContainer = $HUD/CenterSplit/ChordHud/NextChordBox
@onready var lbl_next_chord: Label = $HUD/CenterSplit/ChordHud/NextChordBox/NextLabel
@onready var lbl_lyric_teleprompter: Label = $HUD/CenterSplit/ChordHud/LyricBox/VBox/LyricText
@onready var lbl_lyric_bar: Label = $HUD/CenterSplit/ChordHud/LyricBox/VBox/LyricBarIndicator

@onready var keybed_rect: ColorRect = $HUD/BottomKeybed/KeybedContainer/KeybedRect
@onready var lbl_zone_a: Label = $HUD/BottomKeybed/ZoneRow/ZoneALabel
@onready var lbl_split_line: Label = $HUD/BottomKeybed/ZoneRow/SplitLine
@onready var lbl_zone_b: Label = $HUD/BottomKeybed/ZoneRow/ZoneBLabel

const CARDS_PER_PAGE: int = 6
var cards: Array[PanelContainer] = []
var card_labels: Array[Label] = []

# State
var ws: WebSocketPeer = WebSocketPeer.new()
var is_ws_connected: bool = false
var ws_reconnect_timer: float = 0.0

var all_songs: Array = []
var displayed_songs: Array = []
var current_category: String = "ALL"
var current_search: String = ""
var current_page: int = 0
var current_song_index: int = 0

var current_bpm: int = 120
var beat_timer: float = 0.0
var seconds_per_beat: float = 0.5
var current_bar: int = 1
var current_beat: int = 1

# Shaders & Textures
var keybed_material: ShaderMaterial
var waterfall_material: ShaderMaterial
var key_state_image: Image
var key_state_texture: ImageTexture
var key_velocities: PackedFloat32Array = PackedFloat32Array()
var key_impacts: PackedFloat32Array = PackedFloat32Array()

# Note Stream Runway for Waterfall Visualizer
var waterfall_image: Image
var waterfall_texture: ImageTexture
var waterfall_scroll_timer: float = 0.0

# Feedback FX
var beat_pulse: float = 0.0
var hit_beam_flash: float = 0.0
var screen_shake: float = 0.0
var glitch_intensity: float = 0.0
var original_hud_pos: Vector2 = Vector2.ZERO

func _ready() -> void:
	original_hud_pos = hud_container.position
	key_velocities.resize(64)
	key_impacts.resize(64)
	for i in range(64):
		key_velocities[i] = 0.0
		key_impacts[i] = 0.0

	# 1. Shaders Setup
	if keybed_rect and keybed_rect.material is ShaderMaterial:
		keybed_material = keybed_rect.material
	if waterfall_rect and waterfall_rect.material is ShaderMaterial:
		waterfall_material = waterfall_rect.material

	# Dynamic 64x1 RGBA8 Texture for Keybed & Waterfall Laser Columns
	key_state_image = Image.create_empty(64, 1, false, Image.FORMAT_RGBA8)
	key_state_texture = ImageTexture.create_from_image(key_state_image)
	if keybed_material:
		keybed_material.set_shader_parameter("key_states", key_state_texture)
	if waterfall_material:
		waterfall_material.set_shader_parameter("key_states", key_state_texture)

	# Dynamic 64x128 RGBA8 Falling Note Stream Runway for Waterfall Visualizer
	waterfall_image = Image.create_empty(64, 128, false, Image.FORMAT_RGBA8)
	waterfall_texture = ImageTexture.create_from_image(waterfall_image)
	if waterfall_material:
		waterfall_material.set_shader_parameter("note_stream_texture", waterfall_texture)

	# 2. Wire UI Buttons
	btn_panic.pressed.connect(_on_panic_pressed)
	btn_never_lost.pressed.connect(_on_never_lost_pressed)
	search_box.text_changed.connect(_on_search_changed)
	btn_filter_all.pressed.connect(func(): _set_category("ALL"))
	btn_filter_indo.pressed.connect(func(): _set_category("Indo_Band"))
	btn_filter_western.pressed.connect(func(): _set_category("Western_Band"))
	btn_prev_page.pressed.connect(_on_prev_page)
	btn_next_page.pressed.connect(_on_next_page)

	active_card.gui_input.connect(func(event):
		if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
			_select_song(current_song_index)
	)
	active_card.mouse_entered.connect(func(): active_card.modulate = Color(1.2, 1.2, 1.1))
	active_card.mouse_exited.connect(func(): active_card.modulate = Color.WHITE)

	cards = [next_card_1, next_card_2, next_card_3, next_card_4, next_card_5, next_card_6]
	card_labels = [lbl_card_1, lbl_card_2, lbl_card_3, lbl_card_4, lbl_card_5, lbl_card_6]

	for i in range(cards.size()):
		var slot = i
		var card = cards[i]
		card.gui_input.connect(func(event): _on_card_click(event, slot))
		card.mouse_entered.connect(func(): card.modulate = Color(0.4, 1.3, 1.2))
		card.mouse_exited.connect(func(): card.modulate = Color.WHITE)

	# 3. Load 100 Repertoire JSON
	_load_repertoire_json()

	# 4. Connect to Backend WebSocket
	_connect_ws()

	# Select first song
	if all_songs.size() > 0:
		_select_song(0)

func _load_repertoire_json() -> void:
	var path = "res://setlists/master_100_repertoire.json"
	if not FileAccess.file_exists(path):
		path = "res://setlists/default_band_setlist.json"

	if FileAccess.file_exists(path):
		var file = FileAccess.open(path, FileAccess.READ)
		var text = file.get_as_text()
		var parsed = JSON.parse_string(text)
		if parsed is Array:
			all_songs = parsed
		elif parsed is Dictionary and parsed.has("songs"):
			all_songs = parsed["songs"]

	_filter_songs()

func _set_category(cat: String) -> void:
	current_category = cat
	current_page = 0
	_filter_songs()

func _on_search_changed(new_text: String) -> void:
	current_search = new_text.strip_edges().to_lower()
	current_page = 0
	_filter_songs()

func _filter_songs() -> void:
	displayed_songs.clear()
	for s in all_songs:
		var cat_match = (current_category == "ALL") or (s.get("category", "") == current_category)
		if not cat_match:
			continue

		if current_search == "":
			displayed_songs.append(s)
		else:
			var title = str(s.get("title", "")).to_lower()
			var artist = str(s.get("artist", "")).to_lower()
			var id_str = str(s.get("id", "")).to_lower()
			var tags = str(s.get("tags", "")).to_lower()
			if title.contains(current_search) or artist.contains(current_search) or id_str.contains(current_search) or tags.contains(current_search):
				displayed_songs.append(s)

	_update_cards_ui()

func _on_prev_page() -> void:
	if current_page > 0:
		current_page -= 1
		_update_cards_ui()

func _on_next_page() -> void:
	var max_page = max(0, int(ceil(displayed_songs.size() / float(CARDS_PER_PAGE))) - 1)
	if current_page < max_page:
		current_page += 1
		_update_cards_ui()

func _update_cards_ui() -> void:
	var total = displayed_songs.size()
	var max_page = max(1, int(ceil(total / float(CARDS_PER_PAGE))))
	current_page = clamp(current_page, 0, max_page - 1)
	lbl_page_indicator.text = "PAGE %d/%d (%d)" % [current_page + 1, max_page, total]

	var start = current_page * CARDS_PER_PAGE
	for i in range(cards.size()):
		_set_card_slot(cards[i], card_labels[i], start + i)

func _set_card_slot(card: PanelContainer, label: Label, idx: int) -> void:
	if idx < displayed_songs.size():
		card.visible = true
		var s = displayed_songs[idx]
		label.text = "  %s. %s [%s // %d BPM]" % [str(s.get("id", idx + 1)), str(s.get("title", "")), str(s.get("keySignature", "C")), int(s.get("bpm", 120))]
	else:
		card.visible = false

func _on_card_click(event: InputEvent, slot_offset: int) -> void:
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		var target_idx = (current_page * CARDS_PER_PAGE) + slot_offset
		if target_idx < displayed_songs.size():
			var song = displayed_songs[target_idx]
			var full_idx = all_songs.find(song)
			if full_idx >= 0:
				_select_song(full_idx)

func _select_song(idx: int) -> void:
	if idx < 0 or idx >= all_songs.size():
		return
	current_song_index = idx
	var song = all_songs[idx]

	current_bpm = int(song.get("bpm", 120))
	seconds_per_beat = 60.0 / float(current_bpm)
	current_bar = 1
	current_beat = 1

	lbl_active_mission.text = "> %s - %s" % [str(song.get("title", "")).to_upper(), str(song.get("artist", "")).to_upper()]
	lbl_bpm_beat.text = "[BPM: %d /// %s] [BEAT: 01/4]" % [current_bpm, str(song.get("timeSignature", "4/4"))]
	var tr = int(song.get("transpose", 0))
	lbl_transpose.text = "TRANSPOSE: [ %s%d SEMITONES ]" % ["+" if tr >= 0 else "", tr]

	# Chords & Lyrics
	var prog = song.get("chordProgression", {})
	var intro_chords = prog.get("intro", "C - G - Am - F")
	var chord_tokens = intro_chords.split("-", false)
	var cur_chord = chord_tokens[0].strip_edges() if chord_tokens.size() > 0 else "C"
	var nxt_chord = chord_tokens[1].strip_edges() if chord_tokens.size() > 1 else "G"
	lbl_current_chord.text = "[ %s ]" % cur_chord
	lbl_next_chord.text = "NEXT CHORD -> %s" % nxt_chord
	lbl_lyric_teleprompter.text = "\"%s - %s // Intro: %s\"" % [str(song.get("title", "")), str(song.get("artist", "")), intro_chords]

	# Split Zone
	var split_pt = int(song.get("splitPoint", 60))
	var split_name = "C%d" % (split_pt / 12 - 1)
	lbl_split_line.text = "[ SPLIT POINT: %s ]" % split_name

	var pt = song.get("primaryTone", {})
	var st = song.get("splitTone", {})
	var pt_name = pt.get("name", "CONCERT PIANO")
	var st_name = st.get("name", "WARM PAD")
	lbl_zone_a.text = "ZONE A: SUB-BASS (AMBER PULSE // %s // C2-%s)" % [st_name, split_name]
	lbl_zone_b.text = "ZONE B: NEO-SOLO LEAD (CYAN OVERDRIVE // %s // %s-C7)" % [pt_name, split_name]

	if keybed_material:
		var s_oct = int((split_pt - LOWEST_NOTE) / 12)
		var s_rem = int((split_pt - LOWEST_NOTE) % 12)
		var white_offsets = [0.0, 0.5, 1.0, 1.5, 2.0, 3.0, 3.5, 4.0, 4.5, 5.0, 5.5, 6.0]
		var split_w = float(s_oct * 7) + white_offsets[s_rem]
		keybed_material.set_shader_parameter("split_point_white_key", split_w)

	# Send WebSocket command to C# hardware MIDI engine
	if is_ws_connected:
		var cmd = {
			"command": "TRIGGER_SETLIST_ITEM",
			"payload": {
				"songIndex": idx,
				"songId": str(song.get("id", "")),
				"programChange": int(pt.get("pc", 1)) - 1,
				"bankMsb": int(pt.get("msb", 71)),
				"bankLsb": int(pt.get("lsb", 96)),
				"transpose": tr
			}
		}
		ws.send_text(JSON.stringify(cmd))

func _on_panic_pressed() -> void:
	glitch_intensity = 1.0
	screen_shake = 18.0
	# Clear key states
	for i in range(64):
		key_velocities[i] = 0.0
		key_impacts[i] = 0.0
	_push_key_texture()

	lbl_timing_verdict.text = "ACCURACY: [ ALL CHANNELS PURGED ]"
	lbl_timing_verdict.modulate = Color(0.93, 0.14, 0.23)

	if is_ws_connected:
		var cmd = {"command": "TRIGGER_PANIC"}
		ws.send_text(JSON.stringify(cmd))

func _on_never_lost_pressed() -> void:
	btn_never_lost.modulate = Color(1.0, 0.85, 0.2)
	lbl_timing_verdict.text = "CAPTURING 5-MIN SESSION TO MIDI..."
	lbl_timing_verdict.modulate = Color(1.0, 0.85, 0.2)
	if is_ws_connected:
		var cmd = {"command": "TRIGGER_EXPORT_MIDI"}
		ws.send_text(JSON.stringify(cmd))

func _connect_ws() -> void:
	ws.close()
	var err = ws.connect_to_url(WS_URL)
	if err == OK:
		lbl_ip_address.text = "> LINK: CONNECTING WS..."
	else:
		lbl_ip_address.text = "> LINK: STANDALONE MODE"

func _process(delta: float) -> void:
	# 1. WebSocket Poll
	ws.poll()
	var state = ws.get_ready_state()
	if state == WebSocketPeer.STATE_OPEN:
		if not is_ws_connected:
			is_ws_connected = true
			lbl_ip_address.text = "> LINK: http://127.0.0.1:%d [WS SYNCED]" % HTTP_PORT
			lbl_device_status.text = "> LINK: ROLAND [CONNECTED VIA C#]"
			lbl_device_status.modulate = Color(0.0, 0.96, 0.83)

		while ws.get_available_packet_count() > 0:
			var pkt = ws.get_packet().get_string_from_utf8()
			_handle_ws_packet(pkt)
	elif state == WebSocketPeer.STATE_CLOSED:
		if is_ws_connected:
			is_ws_connected = false
			lbl_ip_address.text = "> LINK: WS DISCONNECTED (RETRYING)"
			lbl_device_status.text = "> LINK: OFFLINE"
			lbl_device_status.modulate = Color(0.93, 0.14, 0.23)

		ws_reconnect_timer += delta
		if ws_reconnect_timer > 3.0:
			ws_reconnect_timer = 0.0
			_connect_ws()

	# 2. Metronome Tick
	beat_timer += delta
	if beat_timer >= seconds_per_beat:
		beat_timer -= seconds_per_beat
		_advance_beat()

	# 3. Decay FX
	beat_pulse = max(0.0, beat_pulse - delta * 2.5)
	hit_beam_flash = max(0.0, hit_beam_flash - delta * 3.0)
	glitch_intensity = max(0.0, glitch_intensity - delta * 4.0)

	if screen_shake > 0.0:
		screen_shake = max(0.0, screen_shake - delta * 15.0)
		hud_container.position = original_hud_pos + Vector2(randf_range(-screen_shake, screen_shake), randf_range(-screen_shake, screen_shake))
	else:
		hud_container.position = original_hud_pos

	for i in range(64):
		if key_impacts[i] > 0.0:
			key_impacts[i] = max(0.0, key_impacts[i] - delta * 3.5)

	# Update Shader Uniforms
	if keybed_material:
		keybed_material.set_shader_parameter("beat_pulse", beat_pulse)
	if waterfall_material:
		waterfall_material.set_shader_parameter("beat_pulse", beat_pulse)
		waterfall_material.set_shader_parameter("hit_beam_intensity", hit_beam_flash)

	_push_key_texture()
	_update_waterfall_stream(delta)

func _update_waterfall_stream(delta: float) -> void:
	if not waterfall_image or not waterfall_texture:
		return

	waterfall_scroll_timer += delta
	# Continuous scroll ~55 frames/sec
	if waterfall_scroll_timer >= 0.018:
		waterfall_scroll_timer = 0.0
		# Blit image downward by 1 pixel (rows 0..126 -> rows 1..127)
		waterfall_image.blit_rect(waterfall_image, Rect2i(0, 0, 64, 127), Vector2i(0, 1))
		# Clear top row
		for x in range(64):
			waterfall_image.set_pixel(x, 0, Color.BLACK)
		waterfall_texture.update(waterfall_image)

func _advance_beat() -> void:
	current_beat += 1
	if current_beat > 4:
		current_beat = 1
		current_bar += 1

	beat_pulse = 1.0
	lbl_bpm_beat.text = "[BPM: %d /// 4/4] [BAR: %03d . BEAT: %d/4]" % [current_bpm, current_bar, current_beat]
	lbl_lyric_bar.text = ">> CURRENT BAR %03d // BEAT %d OF 4" % [current_bar, current_beat]

	_inject_chord_falling_notes()

func _inject_chord_falling_notes() -> void:
	if not waterfall_image or all_songs.size() <= current_song_index:
		return

	var song = all_songs[current_song_index]
	var prog = song.get("chordProgression", {})
	var intro_chords = str(prog.get("intro", "C - G - Am - F"))
	var tokens = intro_chords.split("-", false)
	if tokens.is_empty():
		return

	var chord_idx = (current_bar - 1) % tokens.size()
	var cur_chord = tokens[chord_idx].strip_edges()
	var next_idx = (chord_idx + 1) % tokens.size()
	var next_chord = tokens[next_idx].strip_edges()

	# Dynamically update HUD chord display on bar change
	lbl_current_chord.text = "[ %s ]" % cur_chord
	lbl_next_chord.text = "NEXT CHORD -> %s" % next_chord

	var notes = _get_chord_notes(cur_chord)
	var split_pt = int(song.get("splitPoint", 60))

	# Spawn chord note pills at top row (rows 0, 1, 2)
	for n in notes:
		var idx = n - LOWEST_NOTE
		if idx >= 0 and idx < 61:
			var is_zone_b = 1.0 if n >= split_pt else 0.0
			var col = Color(0.95, is_zone_b, 0.0, 1.0)
			waterfall_image.set_pixel(idx, 0, col)
			waterfall_image.set_pixel(idx, 1, col)
			waterfall_image.set_pixel(idx, 2, col)

func _get_chord_notes(chord_str: String) -> Array[int]:
	var result: Array[int] = []
	var c = chord_str.strip_edges()
	if c.is_empty():
		return [48, 52, 55]

	var root_map = {
		"C": 0, "C#": 1, "Db": 1, "D": 2, "D#": 3, "Eb": 3,
		"E": 4, "F": 5, "F#": 6, "Gb": 6, "G": 7, "G#": 8,
		"Ab": 8, "A": 9, "A#": 10, "Bb": 10, "B": 11
	}

	var root_name = ""
	var is_minor = false
	if c.length() >= 2 and (c[1] == '#' or c[1] == 'b'):
		root_name = c.substr(0, 2)
		is_minor = c.contains("m") and not c.contains("maj")
	else:
		root_name = c.substr(0, 1)
		is_minor = c.contains("m") and not c.contains("maj")

	var root_offset = root_map.get(root_name, 0)
	var root_midi = 48 + root_offset # C3 octave base

	if is_minor:
		result = [root_midi, root_midi + 3, root_midi + 7, root_midi + 12]
	else:
		result = [root_midi, root_midi + 4, root_midi + 7, root_midi + 12]

	return result

func _handle_ws_packet(json_text: String) -> void:
	var parsed = JSON.parse_string(json_text)
	if parsed is Dictionary:
		var type = parsed.get("type", "")
		if type == "NOTE_ON":
			var note = int(parsed.get("note", 60))
			var vel = int(parsed.get("velocity", 100))
			_handle_note_on(note, vel)
		elif type == "NOTE_OFF":
			var note = int(parsed.get("note", 60))
			_handle_note_off(note)
		elif type == "PLAYBACK_TICK":
			var data = parsed.get("data", {})
			lbl_current_chord.text = "[ %s ]" % str(data.get("currentChord", "C"))
			lbl_next_chord.text = "NEXT CHORD -> %s" % str(data.get("nextChord", "G"))
		elif type == "PANIC":
			_on_panic_pressed()
		elif type == "CAPTURE_SAVED":
			var fn = str(parsed.get("filename", "session.mid"))
			var bars = int(parsed.get("bars", 0))
			var evts = int(parsed.get("events", 0))
			lbl_timing_verdict.text = ">> CAPTURED: %s (%d bars, %d notes)" % [fn, bars, evts]
			lbl_timing_verdict.modulate = Color(0.0, 0.96, 0.83)
			btn_never_lost.modulate = Color(0.0, 0.96, 0.83)
		elif type == "DEVICE_STATUS":
			var d_name = str(parsed.get("deviceName", "ROLAND GO:KEYS 3"))
			var conn = bool(parsed.get("connected", false))
			if conn:
				lbl_device_status.text = "> LINK: %s" % d_name.to_upper()
				lbl_device_status.modulate = Color(0.0, 0.96, 0.83)
			else:
				lbl_device_status.text = "> LINK: OFFLINE"
				lbl_device_status.modulate = Color(0.93, 0.14, 0.23)

func _handle_note_on(note: int, velocity: int) -> void:
	var idx = note - LOWEST_NOTE
	if idx >= 0 and idx < 61:
		var v_norm = float(velocity) / 127.0
		key_velocities[idx] = v_norm
		key_impacts[idx] = 1.0
		hit_beam_flash = max(hit_beam_flash, v_norm)

		# Strike rank evaluation
		if velocity >= 110:
			lbl_velocity_rank.text = "RANK [S]"
			lbl_velocity_rank.modulate = Color(1.0, 0.85, 0.2)
			screen_shake = 6.0
		elif velocity >= 90:
			lbl_velocity_rank.text = "RANK [A]"
			lbl_velocity_rank.modulate = Color(0.0, 0.96, 0.83)
		else:
			lbl_velocity_rank.text = "RANK [B]"
			lbl_velocity_rank.modulate = Color(0.8, 0.8, 0.8)

		var bars = int((float(velocity) / 127.0) * 16.0)
		var bar_str = ""
		for b in range(bars): bar_str += "|"
		lbl_strike_gauge.text = "[ %-16s ] %d" % [bar_str, velocity]

func _handle_note_off(note: int) -> void:
	var idx = note - LOWEST_NOTE
	if idx >= 0 and idx < 61:
		key_velocities[idx] = 0.0

func _push_key_texture() -> void:
	var split_pt = 60
	if all_songs.size() > current_song_index:
		split_pt = int(all_songs[current_song_index].get("splitPoint", 60))

	for i in range(61):
		var vel = int(key_velocities[i] * 255.0)
		var is_zone_b = 255 if (i + LOWEST_NOTE) >= split_pt else 0
		var impact = int(key_impacts[i] * 255.0)
		key_state_image.set_pixel(i, 0, Color8(vel, is_zone_b, impact, 255))

	key_state_texture.update(key_state_image)
