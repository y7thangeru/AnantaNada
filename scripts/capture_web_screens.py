import subprocess
import time
import os
import http.server
import socketserver
import threading

PORT = 8089
WEB_DIR = os.path.abspath("web")
CHROME_PATH = r"C:\Program Files\Google\Chrome\Application\chrome.exe"
CAPTURES_DIR = os.path.abspath("captures")

os.makedirs(CAPTURES_DIR, exist_ok=True)

class Handler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=WEB_DIR, **kwargs)

def start_server():
    with socketserver.TCPServer(("", PORT), Handler) as httpd:
        print(f"HTTP Server serving at port {PORT}")
        httpd.serve_forever()

def main():
    t = threading.Thread(target=start_server, daemon=True)
    t.start()
    time.sleep(1.5)

    web_shot = os.path.join(CAPTURES_DIR, "screenshot_web_remote.png")
    android_shot = os.path.join(CAPTURES_DIR, "screenshot_android_companion.png")

    print("1. Capturing Web Remote (1280x820)...")
    cmd_web = [
        CHROME_PATH,
        "--headless=new",
        "--disable-gpu",
        "--window-size=1280,820",
        "--run-all-compositor-stages-before-draw",
        "--virtual-time-budget=3000",
        f"--screenshot={web_shot}",
        f"http://127.0.0.1:{PORT}/index.html"
    ]
    subprocess.run(cmd_web, check=True)
    print(f"Saved: {web_shot}")

    print("2. Capturing Android Companion (430x932)...")
    cmd_android = [
        CHROME_PATH,
        "--headless=new",
        "--disable-gpu",
        "--window-size=430,932",
        "--run-all-compositor-stages-before-draw",
        "--virtual-time-budget=3000",
        f"--screenshot={android_shot}",
        f"http://127.0.0.1:{PORT}/index.html"
    ]
    subprocess.run(cmd_android, check=True)
    print(f"Saved: {android_shot}")
    print("ALL WEB / ANDROID SCREENSHOTS CAPTURED!")

if __name__ == "__main__":
    main()
