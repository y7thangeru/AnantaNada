extends SceneTree

func _init():
	print("--- GODOT UI SCREENSHOT CAPTURE INITIALIZED ---")
	var main_scene = load("res://scenes/MainStageView.tscn")
	if not main_scene:
		print("ERROR: Could not load MainStageView.tscn")
		quit(1)
		return

	var node = main_scene.instantiate()
	root.add_child(node)

	# Render multiple frames so layout, fonts, and shaders compile and initialize
	for i in range(20):
		await process_frame

	var img = root.get_texture().get_image()
	if img:
		var export_path = "captures/screenshot_godot_stage.png"
		var err = img.save_png(export_path)
		print("Screenshot successfully saved to: ", export_path, " Code: ", err)
	else:
		print("ERROR: Failed to capture viewport texture.")

	quit(0)
