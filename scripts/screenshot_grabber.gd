extends Node

func _ready() -> void:
	print("--- SCREENSHOT GRABBER INITIALIZING ---")
	DisplayServer.window_set_size(Vector2i(1920, 1080))
	get_viewport().size = Vector2i(1920, 1080)
	# Allow shaders and nodes to render
	for i in range(35):
		await get_tree().process_frame

	var vp = get_viewport()
	if vp:
		var tex = vp.get_texture()
		if tex:
			var img = tex.get_image()
			if img:
				var export_path = "captures/screenshot_godot_stage.png"
				var err = img.save_png(export_path)
				print("DESKTOP SCREENSHOT SAVED: ", export_path, " Code: ", err)
			else:
				print("ERROR: Image was null")
		else:
			print("ERROR: Viewport texture was null")
	else:
		print("ERROR: Viewport was null")

	get_tree().quit(0)
