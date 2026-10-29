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
@onready var cadence_heart_rect: TextureRect = $HUD/TopBar/CadenceHeartRect
@onready var btn_zen_mode: Button = $HUD/TopBar/ZenModeBtn
@onready var zen_vignette: TextureRect = $ZenVignetteOverlay

@onready var setlist_deck: PanelContainer = $HUD/CenterSplit/SetlistDeck
@onready var search_box: LineEdit = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/SearchBox
@onready var btn_filter_all: Button = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/FilterRow/BtnFilterAll
@onready var btn_filter_indo: Button = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/FilterRow/BtnFilterIndo
@onready var btn_filter_western: Button = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/FilterRow/BtnFilterWestern
@onready var active_card: PanelContainer = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/ActiveCard
@onready var lbl_active_mission: Label = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/ActiveCard/Pad/Row/Col/MissionName
@onready var lbl_active_meta: Label = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/ActiveCard/Pad/Row/Col/MissionMeta

@onready var next_card_1: PanelContainer = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/NextCard1
@onready var next_card_2: PanelContainer = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/NextCard2
@onready var next_card_3: PanelContainer = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/NextCard3
@onready var next_card_4: PanelContainer = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/NextCard4
@onready var next_card_5: PanelContainer = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/NextCard5
@onready var next_card_6: PanelContainer = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/NextCard6

@onready var btn_prev_page: Button = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/PaginationRow/BtnPrevPage
@onready var btn_next_page: Button = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/PaginationRow/BtnNextPage
@onready var lbl_page_indicator: Label = $HUD/CenterSplit/SetlistDeck/NotebookMargin/MissionContainer/PaginationRow/PageIndicator

@onready var minimetro_map: TextureRect = $HUD/CenterSplit/CenterCascade/MiniMetroTransitMap
@onready var waterfall_rect: ColorRect = $HUD/CenterSplit/CenterCascade/WaterfallRect
@onready var nostalgia_left: TextureRect = $HUD/CenterSplit/CenterCascade/WaterfallRect/NostalgiaCornerLeft
@onready var nostalgia_right: TextureRect = $HUD/CenterSplit/CenterCascade/WaterfallRect/NostalgiaCornerRight

@onready var coach_deck: HBoxContainer = $HUD/CenterSplit/CenterCascade/CoachDeck
@onready var lbl_rhythm_accuracy: Label = $HUD/CenterSplit/CenterCascade/CoachDeck/AccuracyValue
@onready var lbl_timing_verdict: Label = $HUD/CenterSplit/CenterCascade/CoachDeck/TimingVerdict
@onready var djmax_badge: TextureRect = $HUD/CenterSplit/CenterCascade/CoachDeck/DJMaxJudgementBadge
@onready var djmax_fever: TextureRect = $HUD/CenterSplit/CenterCascade/CoachDeck/DJMaxFeverGauge
@onready var hifi_burst: TextureRect = $HUD/CenterSplit/CenterCascade/CoachDeck/HiFiBurstBadge
@onready var lbl_strike_gauge: Label = $HUD/CenterSplit/CenterCascade/CoachDeck/StrikeGauge
@onready var lbl_velocity_rank: Label = $HUD/CenterSplit/CenterCascade/CoachDeck/RankBadge
@onready var btn_never_lost: Button = $HUD/CenterSplit/CenterCascade/CoachDeck/RetroBufferBtn

@onready var chord_hud: VBoxContainer = $HUD/CenterSplit/ChordHud
@onready var current_chord_box: PanelContainer = $HUD/CenterSplit/ChordHud/CurrentChordBox
@onready var lbl_current_chord: Label = $HUD/CenterSplit/ChordHud/CurrentChordBox/ChordMargin/VBox/ChordLabel
@onready var lbl_transpose: Label = $HUD/CenterSplit/ChordHud/CurrentChordBox/ChordMargin/VBox/TransposeRow/TransposePill
@onready var btn_transpose_down: Button = $HUD/CenterSplit/ChordHud/CurrentChordBox/ChordMargin/VBox/TransposeRow/BtnTransposeDown
@onready var btn_transpose_up: Button = $HUD/CenterSplit/ChordHud/CurrentChordBox/ChordMargin/VBox/TransposeRow/BtnTransposeUp
@onready var btn_transpose_reset: Button = $HUD/CenterSplit/ChordHud/CurrentChordBox/ChordMargin/VBox/TransposeRow/BtnTransposeReset

@onready var btn_section_intro: Button = $HUD/CenterSplit/CenterCascade/SectionBar/BtnSectionIntro
@onready var btn_section_verse: Button = $HUD/CenterSplit/CenterCascade/SectionBar/BtnSectionVerse
@onready var btn_section_chorus: Button = $HUD/CenterSplit/CenterCascade/SectionBar/BtnSectionChorus
@onready var btn_section_solo: Button = $HUD/CenterSplit/CenterCascade/SectionBar/BtnSectionSolo
@onready var btn_section_outro: Button = $HUD/CenterSplit/CenterCascade/SectionBar/BtnSectionOutro

@onready var btn_compass: Button = $HUD/TopBar/CompassBtn
@onready var qr_modal: ColorRect = $QrPairingModal
@onready var qr_texture_rect: TextureRect = $QrPairingModal/ModalDialog/ModalMargin/ModalVBox/QrTextureRect
@onready var modal_url_label: Label = $QrPairingModal/ModalDialog/ModalMargin/ModalVBox/ModalUrlLabel
@onready var btn_close_qr: Button = $QrPairingModal/ModalDialog/ModalMargin/ModalVBox/BtnCloseQr

@onready var taiko_don_ka: TextureRect = $HUD/CenterSplit/ChordHud/TaikoRow/TaikoDonKa
@onready var taiko_label: Label = $HUD/CenterSplit/ChordHud/TaikoRow/TaikoLabel
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

# Transpose & Musical Scale State
var base_song_transpose: int = 0
var current_transpose_offset: int = 0
const SEMITONE_NAMES: Array[String] = ["C", "C#", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B"]

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

# 12-Master Game UI States
var is_zen_mode: bool = false
var fever_energy: float = 0.0
var burst_scale: float = 1.0
var don_ka_flash: float = 0.0
var active_station: int = 1

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
	waterfall_image.fill(Color.BLACK)
	waterfall_texture = ImageTexture.create_from_image(waterfall_image)
	if waterfall_material:
		waterfall_material.set_shader_parameter("note_stream_texture", waterfall_texture)

	# 1b. Stage Compass Preloaded High-Res QR Texture
	_load_qr_image_direct()

	# 2. Wire UI Buttons
	btn_panic.pressed.connect(_on_panic_pressed)
	btn_never_lost.pressed.connect(_on_never_lost_pressed)
	if btn_zen_mode:
		btn_zen_mode.pressed.connect(_toggle_zen_mode)
	search_box.text_changed.connect(_on_search_changed)
	btn_filter_all.pressed.connect(func(): _set_category("ALL"))
	btn_filter_indo.pressed.connect(func(): _set_category("Indo_Band"))
	btn_filter_western.pressed.connect(func(): _set_category("Western_Band"))
	btn_prev_page.pressed.connect(_on_prev_page)
	btn_next_page.pressed.connect(_on_next_page)

	# Sprint 1: Interactive Transpose Controls
	if btn_transpose_down:
		btn_transpose_down.pressed.connect(func(): _adjust_transpose(-1))
	if btn_transpose_up:
		btn_transpose_up.pressed.connect(func(): _adjust_transpose(1))
	if btn_transpose_reset:
		btn_transpose_reset.pressed.connect(_reset_transpose)

	# Sprint 1: Interactive Section Navigation
	if btn_section_intro:
		btn_section_intro.pressed.connect(func(): _on_jump_section(1, "INTRO"))
	if btn_section_verse:
		btn_section_verse.pressed.connect(func(): _on_jump_section(2, "VERSE"))
	if btn_section_chorus:
		btn_section_chorus.pressed.connect(func(): _on_jump_section(3, "CHORUS"))
	if btn_section_solo:
		btn_section_solo.pressed.connect(func(): _on_jump_section(4, "SOLO"))
	if btn_section_outro:
		btn_section_outro.pressed.connect(func(): _on_jump_section(5, "OUTRO"))

	# Sprint 1: Stage Compass QR Pairing Modal
	if btn_compass:
		btn_compass.pressed.connect(_show_qr_modal)
	if btn_close_qr:
		btn_close_qr.pressed.connect(_hide_qr_modal)

	active_card.gui_input.connect(func(event):
		if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
			_select_song(current_song_index)
	)
	active_card.mouse_entered.connect(func(): active_card.modulate = Color(1.2, 1.2, 1.1))
	active_card.mouse_exited.connect(func(): active_card.modulate = Color.WHITE)

	cards = [next_card_1, next_card_2, next_card_3, next_card_4, next_card_5, next_card_6]

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
		_set_card_slot(cards[i], start + i)

func _set_card_slot(card: PanelContainer, idx: int) -> void:
	if idx < displayed_songs.size():
		card.visible = true
		var s = displayed_songs[idx]
		var badge_lbl: Label = card.get_node_or_null("Pad/Row/Badge/Num")
		var title_lbl: Label = card.get_node_or_null("Pad/Row/Col/Title")
		var meta_lbl: Label = card.get_node_or_null("Pad/Row/Col/Meta")
		if badge_lbl:
			badge_lbl.text = "%02d" % (idx + 1)
		if title_lbl:
			title_lbl.text = str(s.get("title", "")).to_upper()
		if meta_lbl:
			meta_lbl.text = "%s • %s • %d BPM" % [str(s.get("artist", "")), str(s.get("keySignature", "C")), int(s.get("bpm", 120))]
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

	lbl_active_mission.text = str(song.get("title", "")).to_upper()
	if lbl_active_meta:
		lbl_active_meta.text = "%s • %s • %d BPM" % [str(song.get("artist", "")), str(song.get("keySignature", "C")), current_bpm]
	lbl_bpm_beat.text = "[BPM: %d /// %s] [BEAT: 01/4]" % [current_bpm, str(song.get("timeSignature", "4/4"))]
	base_song_transpose = int(song.get("transpose", 0))
	current_transpose_offset = 0
	_apply_transpose(false)
	_update_section_buttons_visual(1)

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
				"transpose": base_song_transpose
			}
		}
		ws.send_text(JSON.stringify(cmd))

# ==========================================
# SPRINT 1: TRANSPOSE, SECTION & QR COMPASS
# ==========================================

func _adjust_transpose(delta_semi: int) -> void:
	current_transpose_offset += delta_semi
	_apply_transpose(true)

func _reset_transpose() -> void:
	current_transpose_offset = 0
	_apply_transpose(true)

func _apply_transpose(dispatch_midi: bool = true) -> void:
	var total_tr = clampi(base_song_transpose + current_transpose_offset, -12, 12)
	if lbl_transpose:
		lbl_transpose.text = "TRANSPOSE: [ %s%d ]" % ["+" if total_tr >= 0 else "", total_tr]
		lbl_transpose.modulate = Color(1.0, 0.85, 0.25) if total_tr == 0 else Color(0.2, 0.95, 0.85)

	# Update chords and lyric teleprompter
	if all_songs.size() > 0 and current_song_index < all_songs.size():
		var song = all_songs[current_song_index]
		var prog = song.get("chordProgression", {})
		var intro_chords = prog.get("intro", "C - G - Am - F")
		var transposed_intro = transpose_progression(intro_chords, total_tr)
		var tokens = transposed_intro.split("-", false)
		var cur_chord = tokens[0].strip_edges() if tokens.size() > 0 else "C"
		var nxt_chord = tokens[1].strip_edges() if tokens.size() > 1 else "G"
		lbl_current_chord.text = "[ %s ]" % cur_chord
		lbl_next_chord.text = "NEXT CHORD -> %s" % nxt_chord
		lbl_lyric_teleprompter.text = "\"%s - %s // %s\"" % [str(song.get("title", "")), str(song.get("artist", "")), transposed_intro]

	if dispatch_midi and is_ws_connected:
		var cmd = {
			"command": "TRIGGER_TRANSPOSE",
			"payload": {
				"semitones": total_tr
			}
		}
		ws.send_text(JSON.stringify(cmd))

func transpose_chord(chord: String, semitones: int) -> String:
	if semitones == 0 or chord.is_empty():
		return chord
	var root = ""
	var suffix = ""
	if chord.length() >= 2 and (chord[1] == "#" or chord[1] == "b"):
		root = chord.substr(0, 2)
		suffix = chord.substr(2)
	elif chord.length() >= 1:
		root = chord.substr(0, 1)
		suffix = chord.substr(1)
	else:
		return chord

	var note_map = {
		"C": 0, "B#": 0, "C#": 1, "Db": 1, "D": 2, "D#": 3, "Eb": 3,
		"E": 4, "Fb": 4, "F": 5, "E#": 5, "F#": 6, "Gb": 6, "G": 7,
		"G#": 8, "Ab": 8, "A": 9, "A#": 10, "Bb": 10, "B": 11, "Cb": 11
	}
	if not note_map.has(root):
		return chord
	var idx = int(note_map[root])
	var new_idx = posmod(idx + semitones, 12)
	return SEMITONE_NAMES[new_idx] + suffix

func transpose_progression(prog: String, semitones: int) -> String:
	if semitones == 0:
		return prog
	var parts = prog.split("-")
	var new_parts: Array[String] = []
	for p in parts:
		var c = p.strip_edges()
		new_parts.append(transpose_chord(c, semitones))
	return " - ".join(new_parts)

func _on_jump_section(station_idx: int, sec_name: String) -> void:
	active_station = station_idx
	match station_idx:
		1: current_bar = 1
		2: current_bar = 5
		3: current_bar = 17
		4: current_bar = 29
		5: current_bar = 37
	current_beat = 1

	_update_section_buttons_visual(station_idx)

	if all_songs.size() > 0 and current_song_index < all_songs.size():
		var song = all_songs[current_song_index]
		var prog = song.get("chordProgression", {})
		var sec_key = sec_name.to_lower()
		var sec_chords = prog.get(sec_key, prog.get("intro", "C - G - Am - F"))
		var total_tr = clampi(base_song_transpose + current_transpose_offset, -12, 12)
		var transposed_chords = transpose_progression(sec_chords, total_tr)
		var tokens = transposed_chords.split("-", false)
		var cur = tokens[0].strip_edges() if tokens.size() > 0 else "C"
		var nxt = tokens[1].strip_edges() if tokens.size() > 1 else "G"
		lbl_current_chord.text = "[ %s ]" % cur
		lbl_next_chord.text = "NEXT CHORD -> %s" % nxt
		lbl_lyric_teleprompter.text = "[SECTION: %s] %s" % [sec_name, transposed_chords]
		lbl_lyric_bar.text = ">> CURRENT BAR %03d // BEAT 1 OF 4 [%s]" % [current_bar, sec_name]

	if is_ws_connected:
		var cmd = {
			"command": "TRIGGER_SECTION",
			"payload": {
				"section": sec_name,
				"bar": current_bar
			}
		}
		ws.send_text(JSON.stringify(cmd))

func _update_section_buttons_visual(station: int) -> void:
	var btns = [btn_section_intro, btn_section_verse, btn_section_chorus, btn_section_solo, btn_section_outro]
	for i in range(btns.size()):
		var b = btns[i]
		if b:
			if i + 1 == station:
				b.modulate = Color(1.8, 1.4, 0.4) # Glowing gold active section
			else:
				b.modulate = Color(0.85, 0.85, 0.85) # Normal

func _show_qr_modal() -> void:
	if qr_modal:
		qr_modal.visible = true
		_load_qr_image_direct()
		_fetch_qr_code()

func _load_qr_image_direct() -> void:
	if not qr_texture_rect:
		return
	if ResourceLoader.exists("res://assets/qr_stage_hub.png"):
		var res = load("res://assets/qr_stage_hub.png")
		if res:
			qr_texture_rect.texture = res
			print("[Stage Compass] Preloaded QR resource applied.")
			return
	var global_path = ProjectSettings.globalize_path("res://assets/qr_stage_hub.png")
	if FileAccess.file_exists(global_path):
		var img = Image.load_from_file(global_path)
		if img and not img.is_empty():
			var tex = ImageTexture.create_from_image(img)
			qr_texture_rect.texture = tex
			print("[Stage Compass] Direct QR texture loaded successfully.")

func _hide_qr_modal() -> void:
	if qr_modal:
		qr_modal.visible = false

func _fetch_qr_code() -> void:
	var http = HTTPRequest.new()
	add_child(http)
	http.request_completed.connect(func(_result, response_code, _headers, body):
		if response_code == 200:
			var json_str = body.get_string_from_utf8()
			var parsed = JSON.parse_string(json_str)
			if parsed is Dictionary and modal_url_label:
				var url = str(parsed.get("url", "http://127.0.0.1:8080"))
				modal_url_label.text = url
		http.queue_free()
		_fetch_qr_image()
	)
	http.request("http://127.0.0.1:8080/api/lan-info")

func _fetch_qr_image() -> void:
	var img_http = HTTPRequest.new()
	add_child(img_http)
	img_http.request_completed.connect(func(_result, response_code, _headers, body):
		if response_code == 200 and qr_texture_rect:
			var img = Image.new()
			var err = img.load_png_from_buffer(body)
			if err == OK:
				var tex = ImageTexture.create_from_image(img)
				qr_texture_rect.texture = tex
		img_http.queue_free()
	)
	img_http.request("http://127.0.0.1:8080/api/qrcode")

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

	# 4. 12-Master Game Visualizer Updates
	# Cadence of Hyrule: 16-Bit Heart Beat Pulse Bar
	if cadence_heart_rect:
		cadence_heart_rect.scale = Vector2.ONE * (1.0 + beat_pulse * 0.16)
		cadence_heart_rect.modulate = Color(1.0 + beat_pulse * 0.5, 1.0 - beat_pulse * 0.1, 1.0 - beat_pulse * 0.1, 0.85 + beat_pulse * 0.15)

	# Mini Metro: Song Section Transit Schematic Progression
	var target_station = 1
	if current_bar >= 37:
		target_station = 5 # OUTRO
	elif current_bar >= 29:
		target_station = 4 # SOLO
	elif current_bar >= 17:
		target_station = 3 # CHORUS
	elif current_bar >= 5:
		target_station = 2 # VERSE
	else:
		target_station = 1 # INTRO

	if target_station != active_station:
		active_station = target_station
		_update_section_buttons_visual(active_station)
		if minimetro_map:
			minimetro_map.modulate = Color(1.4, 1.3, 0.8)
	elif minimetro_map and minimetro_map.modulate != Color.WHITE:
		minimetro_map.modulate = minimetro_map.modulate.lerp(Color.WHITE, delta * 3.0)

	# DJMAX Respect V: Dynamic FEVER Gauge & Judgement Decay
	fever_energy = max(0.0, fever_energy - delta * 0.04)
	if djmax_fever:
		var fever_tint = Color(0.3 + fever_energy * 0.7, 0.8, 1.0) if fever_energy < 1.0 else Color(1.6 + beat_pulse * 0.4, 0.3, 1.4)
		djmax_fever.modulate = fever_tint

	if djmax_badge and djmax_badge.modulate != Color.WHITE:
		djmax_badge.modulate = djmax_badge.modulate.lerp(Color.WHITE, delta * 3.0)

	# Hi-Fi Rush: Comic Pop-Art Burst Decay
	if hifi_burst:
		if burst_scale > 1.0:
			burst_scale = max(1.0, burst_scale - delta * 3.0)
			hifi_burst.scale = Vector2.ONE * burst_scale
		else:
			hifi_burst.modulate = hifi_burst.modulate.lerp(Color(1.0, 1.0, 1.0, 0.4 + beat_pulse * 0.6), delta * 4.0)

	# NOSTALGIA Op. 2: Baroque Gilded Shimmer
	if nostalgia_left and nostalgia_left.modulate != Color.WHITE:
		nostalgia_left.modulate = nostalgia_left.modulate.lerp(Color.WHITE, delta * 2.5)
	if nostalgia_right and nostalgia_right.modulate != Color.WHITE:
		nostalgia_right.modulate = nostalgia_right.modulate.lerp(Color.WHITE, delta * 2.5)

	# Taiko: Don / Ka Drum Marker Glow Decay
	if taiko_don_ka and don_ka_flash > 0.0:
		don_ka_flash = max(0.0, don_ka_flash - delta * 4.0)
		taiko_don_ka.modulate = Color(1.0 + don_ka_flash * 0.5, 1.0 + don_ka_flash * 0.5, 1.0)
	elif taiko_don_ka and taiko_don_ka.modulate != Color.WHITE:
		taiko_don_ka.modulate = Color.WHITE

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
		_update_chord_hud_on_bar()

	beat_pulse = 1.0
	lbl_bpm_beat.text = "[BPM: %d /// 4/4] [BAR: %03d . BEAT: %d/4]" % [current_bpm, current_bar, current_beat]
	lbl_lyric_bar.text = ">> CURRENT BAR %03d // BEAT %d OF 4" % [current_bar, current_beat]

func _update_chord_hud_on_bar() -> void:
	if all_songs.size() <= current_song_index:
		return

	var song = all_songs[current_song_index]
	var prog = song.get("chordProgression", {})
	var intro_chords = str(prog.get("intro", "C - G - Am - F"))
	var total_tr = clampi(base_song_transpose + current_transpose_offset, -12, 12)
	var transposed_intro = transpose_progression(intro_chords, total_tr)
	var tokens = transposed_intro.split("-", false)
	if tokens.is_empty():
		return

	var chord_idx = (current_bar - 1) % tokens.size()
	var cur_chord = tokens[chord_idx].strip_edges()
	var next_idx = (chord_idx + 1) % tokens.size()
	var next_chord = tokens[next_idx].strip_edges()

	# Dynamically update HUD chord display on bar change
	lbl_current_chord.text = "[ %s ]" % cur_chord
	lbl_next_chord.text = "NEXT CHORD -> %s" % next_chord

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
		elif type == "TRANSPOSE_CHANGED":
			var semi = int(parsed.get("semitones", 0))
			current_transpose_offset = semi - base_song_transpose
			_apply_transpose(false)
		elif type == "SECTION_CHANGED":
			var sec = str(parsed.get("section", "INTRO"))
			var bar = int(parsed.get("bar", 1))
			current_bar = bar
			current_beat = 1
			lbl_lyric_bar.text = ">> CURRENT BAR %03d // BEAT 1 OF 4 [%s]" % [current_bar, sec]
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

		# 12-Master Game Interactive Hooks
		fever_energy = min(1.2, fever_energy + 0.07 * v_norm)
		don_ka_flash = 1.0

		var split_pt = 60
		if all_songs.size() > current_song_index:
			split_pt = int(all_songs[current_song_index].get("splitPoint", 60))

		# Taiko: Don / Ka Drum Feedback
		if taiko_label:
			if note < split_pt:
				taiko_label.text = "[★ DON! BASS HIT ★]"
				taiko_label.modulate = Color(1.0, 0.25, 0.35)
			else:
				taiko_label.text = "[★ KA! LEAD HIT ★]"
				taiko_label.modulate = Color(0.0, 0.96, 0.83)

		# Strike rank evaluation & Hi-Fi / DJMAX triggers
		if velocity >= 110:
			lbl_velocity_rank.text = "RANK [S]"
			lbl_velocity_rank.modulate = Color(1.0, 0.85, 0.2)
			screen_shake = 6.0
			burst_scale = 1.35
			if djmax_badge:
				djmax_badge.modulate = Color(2.0, 1.8, 0.4)
			if nostalgia_left:
				nostalgia_left.modulate = Color(1.6, 1.4, 0.6)
			if nostalgia_right:
				nostalgia_right.modulate = Color(1.6, 1.4, 0.6)
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

func _toggle_zen_mode() -> void:
	is_zen_mode = not is_zen_mode
	if setlist_deck: setlist_deck.visible = not is_zen_mode
	if chord_hud: chord_hud.visible = not is_zen_mode
	if coach_deck: coach_deck.visible = not is_zen_mode
	if zen_vignette: zen_vignette.visible = is_zen_mode
	if is_zen_mode:
		lbl_timing_verdict.text = ">> GRIS ZEN MODE: ACTIVE (MINIMALIST CONCERT FOCUS)"
		lbl_timing_verdict.modulate = Color(0.8, 0.9, 1.0)
	else:
		lbl_timing_verdict.text = "ACCURACY: [ PERFECT! ]"
		lbl_timing_verdict.modulate = Color(0.0, 0.96, 0.83)

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
