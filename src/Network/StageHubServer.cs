using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AnantaNada.Network;

/// <summary>
/// Embedded lightweight HTTP & WebSocket server inside the Godot .NET host.
/// Uses standard cross-platform TcpListener on IPAddress.Any (0.0.0.0) to serve
/// the mobile web remote and bidirectional telemetry without requiring Windows Administrator elevation.
/// </summary>
public sealed class StageHubServer : IDisposable
{
    private TcpListener? _tcpListener;
    private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new();
    private readonly string _webRootDirectory;
    private readonly int _port;
    private readonly CancellationTokenSource _cts = new();

    private Thread? _serverThread;
    private bool _isRunning;
    private bool _isDisposed;

    // Events triggered by incoming remote commands
    public event Action<TriggerSetlistItemPayload>? OnSetlistItemTriggered;
    public event Action? OnPanicTriggered;
    public event Action? OnExportMidiTriggered;
    public event Action<int>? OnTransposeTriggered;
    public event Action<string, int>? OnSectionTriggered;
    public event Action<string, bool, byte>? OnPerformancePadTriggered;
    public event Action<int, int, int, int>? OnEuclideanConfigTriggered;
    public event Action<string, byte, string>? OnKeyGravityTriggered;
    public event Action<string, double>? OnMotionRecorderTriggered;
    public event Action<string>? OnLog;
    public event Action<int>? OnClientCountChanged;

    public int Port => _port;
    public bool IsRunning => _isRunning;
    public int ConnectedClientsCount => _clients.Count;

    public StageHubServer(int port = 8080, string? webRootDirectory = null)
    {
        _port = port;
        _webRootDirectory = webRootDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web");

        // Fallback search if running from Godot editor or custom directory
        if (!Directory.Exists(_webRootDirectory))
        {
            string alternative = Path.Combine(Directory.GetCurrentDirectory(), "web");
            if (Directory.Exists(alternative))
            {
                _webRootDirectory = alternative;
            }
        }
    }

    /// <summary>
    /// Starts the embedded HTTP and WebSocket listener on 0.0.0.0 (all LAN interfaces).
    /// Standard TcpListener does not require Windows HTTP.sys URL reservations or elevation.
    /// </summary>
    public bool Start()
    {
        if (_isRunning) return true;

        try
        {
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _tcpListener.Start();

            _isRunning = true;
            _serverThread = new Thread(ListenLoop)
            {
                Name = "AnantaNada-StageHub-TcpListener",
                IsBackground = true
            };
            _serverThread.Start();

            string primaryIp = GetLocalIpAddresses().FirstOrDefault(ip => ip != "127.0.0.1") ?? "127.0.0.1";
            OnLog?.Invoke($"[StageHub Server] Online on port {_port} (All interfaces 0.0.0.0 / Primary LAN: {primaryIp}). WebRoot: {_webRootDirectory}");
            return true;
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"[StageHub Server] Failed to start on port {_port}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Returns all active IPv4 addresses ordered with physical Wi-Fi and Ethernet first,
    /// so mobile phone scanners immediately get the correct local LAN address.
    /// </summary>
    public static List<string> GetLocalIpAddresses()
    {
        var list = new List<string>();
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up &&
                            i.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .OrderByDescending(i =>
                {
                    if (i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) return 3;
                    string desc = i.Description.ToLowerInvariant();
                    if (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        !desc.Contains("virtual") && !desc.Contains("tailscale") && !desc.Contains("vethernet"))
                    {
                        return 2;
                    }
                    return 1;
                });

            foreach (var iface in interfaces)
            {
                foreach (var addr in iface.GetIPProperties().UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ipStr = addr.Address.ToString();
                        if (!list.Contains(ipStr))
                        {
                            list.Add(ipStr);
                        }
                    }
                }
            }
        }
        catch
        {
            list.Add("127.0.0.1");
        }

        if (list.Count == 0) list.Add("127.0.0.1");
        return list;
    }

    private async void ListenLoop()
    {
        while (_isRunning && !_cts.Token.IsCancellationRequested)
        {
            try
            {
                if (_tcpListener == null || !_isRunning) break;
                var tcpClient = await _tcpListener.AcceptTcpClientAsync(_cts.Token);
                _ = ProcessTcpConnectionAsync(tcpClient);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                if (_isRunning)
                {
                    OnLog?.Invoke($"[StageHub Accept Error] {ex.Message}");
                }
            }
        }
    }

    private async Task ProcessTcpConnectionAsync(TcpClient client)
    {
        try
        {
            using (client)
            await using (var stream = client.GetStream())
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                var buffer = new byte[8192];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                if (bytesRead <= 0) return;

                string headerText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                using var reader = new StringReader(headerText);
                string? requestLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(requestLine)) return;

                var reqTokens = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (reqTokens.Length < 2) return;
                string method = reqTokens[0].ToUpperInvariant();
                string rawUrl = reqTokens[1];

                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string? line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    int colon = line.IndexOf(':');
                    if (colon > 0)
                    {
                        string hKey = line.Substring(0, colon).Trim();
                        string hVal = line.Substring(colon + 1).Trim();
                        headers[hKey] = hVal;
                    }
                }

                // Check WebSocket Upgrade
                if (headers.TryGetValue("Upgrade", out var up) &&
                    up.Equals("websocket", StringComparison.OrdinalIgnoreCase) &&
                    headers.TryGetValue("Sec-WebSocket-Key", out var secKey))
                {
                    // RFC 6455 Handshake
                    string acceptKey = Convert.ToBase64String(
                        SHA1.HashData(Encoding.UTF8.GetBytes(secKey.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))
                    );

                    string responseHeader =
                        "HTTP/1.1 101 Switching Protocols\r\n" +
                        "Upgrade: websocket\r\n" +
                        "Connection: Upgrade\r\n" +
                        $"Sec-WebSocket-Accept: {acceptKey}\r\n\r\n";

                    byte[] respBytes = Encoding.UTF8.GetBytes(responseHeader);
                    await stream.WriteAsync(respBytes, 0, respBytes.Length);
                    await stream.FlushAsync();

                    using var ws = WebSocket.CreateFromStream(stream, isServer: true, subProtocol: null, TimeSpan.FromSeconds(30));
                    var clientId = Guid.NewGuid();
                    _clients.TryAdd(clientId, ws);
                    OnClientCountChanged?.Invoke(_clients.Count);
                    OnLog?.Invoke($"[StageHub WS] Client connected [{clientId}]. Total: {_clients.Count}");

                    var wsBuffer = new byte[4096];
                    try
                    {
                        while (ws.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
                        {
                            var receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(wsBuffer), _cts.Token);
                            if (receiveResult.MessageType == WebSocketMessageType.Close)
                            {
                                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                                break;
                            }
                            if (receiveResult.MessageType == WebSocketMessageType.Text)
                            {
                                string json = Encoding.UTF8.GetString(wsBuffer, 0, receiveResult.Count);
                                HandleClientJsonCommand(json);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OnLog?.Invoke($"[StageHub WS Client Ex] {ex.Message}");
                    }
                    finally
                    {
                        _clients.TryRemove(clientId, out _);
                        OnClientCountChanged?.Invoke(_clients.Count);
                        OnLog?.Invoke($"[StageHub WS] Client disconnected [{clientId}]. Remaining: {_clients.Count}");
                    }
                    return;
                }

                // Standard HTTP handling (GET / HEAD)
                await HandleHttpRequestAsync(stream, method, rawUrl);
            }
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"[StageHub Conn Error] {ex.Message}");
        }
    }

    private async Task HandleHttpRequestAsync(NetworkStream stream, string method, string rawUrl)
    {
        string path = rawUrl.Split('?')[0];

        // 1. API: LAN Info & QR
        if (path.Equals("/api/lan-info", StringComparison.OrdinalIgnoreCase))
        {
            var ips = GetLocalIpAddresses();
            string primaryIp = ips.FirstOrDefault(ip => ip != "127.0.0.1") ?? "127.0.0.1";
            string targetUrl = $"http://{primaryIp}:{_port}";

            using var qrGen = new QRCoder.QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(targetUrl, QRCoder.QRCodeGenerator.ECCLevel.M);
            using var pngQr = new QRCoder.PngByteQRCode(qrData);
            byte[] qrBytes = pngQr.GetGraphic(10);
            string base64 = Convert.ToBase64String(qrBytes);

            var info = new
            {
                primaryIp,
                allIps = ips,
                port = _port,
                url = targetUrl,
                qrDataUrl = $"data:image/png;base64,{base64}"
            };

            byte[] jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(info));
            await SendHttpResponseAsync(stream, method, 200, "OK", "application/json; charset=utf-8", jsonBytes);
            return;
        }

        // 2. API: QR Code Image
        if (path.Equals("/api/qrcode", StringComparison.OrdinalIgnoreCase))
        {
            var ips = GetLocalIpAddresses();
            string primaryIp = ips.FirstOrDefault(ip => ip != "127.0.0.1") ?? "127.0.0.1";
            string targetUrl = $"http://{primaryIp}:{_port}";

            using var qrGen = new QRCoder.QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(targetUrl, QRCoder.QRCodeGenerator.ECCLevel.M);
            using var pngQr = new QRCoder.PngByteQRCode(qrData);
            byte[] qrBytes = pngQr.GetGraphic(12);

            await SendHttpResponseAsync(stream, method, 200, "OK", "image/png", qrBytes);
            return;
        }

        // 3. Static Web Files
        if (path == "/" || string.IsNullOrWhiteSpace(path))
        {
            path = "/index.html";
        }

        string relativePath = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        string fullPath = Path.Combine(_webRootDirectory, relativePath);

        if (!File.Exists(fullPath))
        {
            byte[] notFoundBytes = Encoding.UTF8.GetBytes("404 Not Found - AnantaNada StageHub Remote");
            await SendHttpResponseAsync(stream, method, 404, "Not Found", "text/plain; charset=utf-8", notFoundBytes);
            return;
        }

        string ext = Path.GetExtension(fullPath).ToLowerInvariant();
        string mimeType = ext switch
        {
            ".html" or ".htm" => "text/html; charset=utf-8",
            ".js" => "application/javascript; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".json" => "application/json; charset=utf-8",
            ".svg" => "image/svg+xml",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".ico" => "image/x-icon",
            _ => "application/octet-stream"
        };

        byte[] fileBytes = await File.ReadAllBytesAsync(fullPath);
        await SendHttpResponseAsync(stream, method, 200, "OK", mimeType, fileBytes);
    }

    private static async Task SendHttpResponseAsync(NetworkStream stream, string method, int statusCode, string statusMsg, string contentType, byte[] bodyBytes)
    {
        var sb = new StringBuilder();
        sb.Append($"HTTP/1.1 {statusCode} {statusMsg}\r\n");
        sb.Append($"Content-Type: {contentType}\r\n");
        sb.Append($"Content-Length: {bodyBytes.Length}\r\n");
        sb.Append("Access-Control-Allow-Origin: *\r\n");
        sb.Append("Connection: close\r\n\r\n");

        byte[] headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
        await stream.WriteAsync(headerBytes, 0, headerBytes.Length);

        if (!method.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
        {
            await stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
        }
        await stream.FlushAsync();
    }

    private void HandleClientJsonCommand(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("command", out var cmdProp))
            {
                string command = cmdProp.GetString() ?? "";

                switch (command.ToUpperInvariant())
                {
                    case "PANIC":
                        OnLog?.Invoke("[StageHub Remote] Received PANIC command.");
                        OnPanicTriggered?.Invoke();
                        break;

                    case "EXPORT_MIDI":
                        OnLog?.Invoke("[StageHub Remote] Received EXPORT_MIDI command.");
                        OnExportMidiTriggered?.Invoke();
                        break;

                    case "SET_TRANSPOSE":
                        if (root.TryGetProperty("semitones", out var semiProp))
                        {
                            int semitones = semiProp.GetInt32();
                            OnLog?.Invoke($"[StageHub Remote] Received SET_TRANSPOSE ({semitones})");
                            OnTransposeTriggered?.Invoke(semitones);
                        }
                        break;

                    case "JUMP_SECTION":
                        string sectionName = root.TryGetProperty("section", out var sProp) ? sProp.GetString() ?? "INTRO" : "INTRO";
                        int barIndex = root.TryGetProperty("bar", out var bProp) ? bProp.GetInt32() : 1;
                        OnLog?.Invoke($"[StageHub Remote] JUMP_SECTION -> {sectionName} (Bar {barIndex})");
                        OnSectionTriggered?.Invoke(sectionName, barIndex);
                        break;

                    case "TRIGGER_SETLIST_ITEM":
                        if (root.TryGetProperty("payload", out var payloadProp))
                        {
                            var payload = JsonSerializer.Deserialize<TriggerSetlistItemPayload>(payloadProp.GetRawText());
                            if (payload != null)
                            {
                                OnLog?.Invoke($"[StageHub Remote] Switched setlist item #{payload.SongIndex} (PC:{payload.ProgramChange})");
                                OnSetlistItemTriggered?.Invoke(payload);
                            }
                        }
                        break;

                    case "TRIGGER_PERFORMANCE_PAD":
                        string padType = root.TryGetProperty("padType", out var ptProp) ? ptProp.GetString() ?? "Stutter" : "Stutter";
                        bool active = !root.TryGetProperty("active", out var actProp) || actProp.GetBoolean();
                        byte channel = root.TryGetProperty("channel", out var chProp) ? chProp.GetByte() : (byte)0;
                        OnLog?.Invoke($"[StageHub Remote] Pad Triggered: {padType} (Active: {active})");
                        OnPerformancePadTriggered?.Invoke(padType, active, channel);
                        break;

                    case "SET_EUCLIDEAN_CONFIG":
                        int rIdx = root.TryGetProperty("ringIndex", out var riProp) ? riProp.GetInt32() : 0;
                        int steps = root.TryGetProperty("steps", out var stProp) ? stProp.GetInt32() : 16;
                        int pulses = root.TryGetProperty("pulses", out var puProp) ? puProp.GetInt32() : 4;
                        int rot = root.TryGetProperty("rotation", out var roProp) ? roProp.GetInt32() : 0;
                        OnLog?.Invoke($"[StageHub Remote] Euclidean Ring #{rIdx} -> E({pulses},{steps}) rot:{rot}");
                        OnEuclideanConfigTriggered?.Invoke(rIdx, steps, pulses, rot);
                        break;

                    case "SET_KEY_GRAVITY":
                        string mode = root.TryGetProperty("mode", out var mdProp) ? mdProp.GetString() ?? "SnapNearest" : "SnapNearest";
                        byte rootNote = root.TryGetProperty("rootNote", out var rnProp) ? rnProp.GetByte() : (byte)0;
                        string scale = root.TryGetProperty("scale", out var scProp) ? scProp.GetString() ?? "Major" : "Major";
                        OnLog?.Invoke($"[StageHub Remote] Key Gravity Mode: {mode} (Scale: {rootNote} {scale})");
                        OnKeyGravityTriggered?.Invoke(mode, rootNote, scale);
                        break;

                    case "TRIGGER_MOTION_REC":
                        string mAction = root.TryGetProperty("action", out var maProp) ? maProp.GetString() ?? "START" : "START";
                        double dur = root.TryGetProperty("duration", out var duProp) ? duProp.GetDouble() : 2.0;
                        OnLog?.Invoke($"[StageHub Remote] Motion Action: {mAction} ({dur}s)");
                        OnMotionRecorderTriggered?.Invoke(mAction, dur);
                        break;

                    default:
                        OnLog?.Invoke($"[StageHub Remote] Unrecognized command: {command}");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"[StageHub Command Parse Error] {ex.Message}");
        }
    }

    /// <summary>
    /// Broadcasts playback status, bar, beat, BPM, active chord, and patch to all connected mobile clients.
    /// </summary>
    public async Task BroadcastPlaybackTickAsync(PlaybackTickData tickData)
    {
        if (_clients.IsEmpty) return;

        var message = new PlaybackTickMessage
        {
            Type = "PLAYBACK_TICK",
            Data = tickData
        };

        string json = JsonSerializer.Serialize(message);
        await BroadcastRawJsonAsync(json);
    }

    public async Task BroadcastNoteOnAsync(byte channel, byte note, byte velocity)
    {
        string json = $"{{\"type\":\"NOTE_ON\",\"channel\":{channel},\"note\":{note},\"velocity\":{velocity}}}";
        await BroadcastRawJsonAsync(json);
    }

    public async Task BroadcastNoteOffAsync(byte channel, byte note)
    {
        string json = $"{{\"type\":\"NOTE_OFF\",\"channel\":{channel},\"note\":{note}}}";
        await BroadcastRawJsonAsync(json);
    }

    public async Task BroadcastPanicAsync()
    {
        string json = "{\"type\":\"PANIC\"}";
        await BroadcastRawJsonAsync(json);
    }

    public async Task BroadcastDeviceStatusAsync(string deviceName, bool isConnected)
    {
        string json = $"{{\"type\":\"DEVICE_STATUS\",\"deviceName\":\"{deviceName}\",\"connected\":{(isConnected ? "true" : "false")}}}";
        await BroadcastRawJsonAsync(json);
    }

    public async Task BroadcastCaptureToastAsync(string filename, int eventsCount, int bars)
    {
        string json = $"{{\"type\":\"CAPTURE_SAVED\",\"filename\":\"{filename}\",\"events\":{eventsCount},\"bars\":{bars}}}";
        await BroadcastRawJsonAsync(json);
    }

    public async Task BroadcastHarmonyUpdateAsync(string chordName, string root, string quality, string bass, string gravityMode)
    {
        string json = $"{{\"type\":\"HARMONY_UPDATE\",\"chordName\":\"{chordName}\",\"root\":\"{root}\",\"quality\":\"{quality}\",\"bass\":\"{bass}\",\"gravityMode\":\"{gravityMode}\"}}";
        await BroadcastRawJsonAsync(json);
    }

    public async Task BroadcastEuclideanTickAsync(string ringName, int currentStep, int totalSteps)
    {
        string json = $"{{\"type\":\"EUCLIDEAN_TICK\",\"ringName\":\"{ringName}\",\"currentStep\":{currentStep},\"totalSteps\":{totalSteps}}}";
        await BroadcastRawJsonAsync(json);
    }

    public async Task BroadcastRawJsonAsync(string json)
    {
        if (_clients.IsEmpty) return;

        byte[] bytes = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(bytes);
        var deadClients = new List<Guid>();

        foreach (var pair in _clients)
        {
            if (pair.Value.State == WebSocketState.Open)
            {
                try
                {
                    await pair.Value.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch
                {
                    deadClients.Add(pair.Key);
                }
            }
            else
            {
                deadClients.Add(pair.Key);
            }
        }

        foreach (var id in deadClients)
        {
            if (_clients.TryRemove(id, out var deadSocket))
            {
                try { deadSocket.Dispose(); } catch { }
            }
        }

        if (deadClients.Count > 0)
        {
            OnClientCountChanged?.Invoke(_clients.Count);
        }
    }

    public void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;

        _cts.Cancel();

        foreach (var pair in _clients)
        {
            try
            {
                pair.Value.Abort();
                pair.Value.Dispose();
            }
            catch { }
        }
        _clients.Clear();

        try
        {
            _tcpListener?.Stop();
        }
        catch { }

        OnLog?.Invoke("[StageHub Server] Stopped.");
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        Stop();
        _cts.Dispose();
    }
}
