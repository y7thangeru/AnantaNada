import webview
import time
from PIL import ImageGrab

def capture_screenshot(window):
    time.sleep(1.5)
    # Grab window bounds
    x = max(0, window.x)
    y = max(0, window.y)
    bbox = (x, y, x + window.width, y + window.height)
    img = ImageGrab.grab(bbox=bbox)
    output_path = "d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/captures/screenshot_web_remote.png"
    img.save(output_path)
    print(f"Web remote screenshot saved to: {output_path}")
    window.destroy()

if __name__ == '__main__':
    html_path = "d:/AntiGravity_Project/AnantaNada_RolandGoKeys3/web/index.html"
    window = webview.create_window(
        title="AnantaNada StageHub Remote",
        url=f"file:///{html_path}",
        width=480,
        height=860,
        resizable=False
    )
    webview.start(capture_screenshot, window)
