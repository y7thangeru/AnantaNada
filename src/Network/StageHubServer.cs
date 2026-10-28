using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace AnantaNada.Network;

/// <summary>
/// Embedded lightweight HTTP & WebSocket server inside the Godot .NET host.
/// Serves static web remote assets (Canvas2D teleprompter & setlist HUD)
/// and handles real-time bidirectional telemetry over local Wi-Fi / Hotspot.
/// </summary>
public sealed class StageHubServer : IDisposable
{
    private readonly HttpListener _listener = new();
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
    /// Starts the embedded HTTP and WebSocket listener.
    /// </summary>
    public bool Start()
    {
        if (_isRunning) return true;

        try
        {
            // Listen on all network interfaces
            _listener.Prefixes.Clear();
            _listener.Prefixes.Add($"http://*:{_port}/");
            _listener.Start();

            _isRunning = true;
            _serverThread = new Thread(ListenLoop)
            {
                Name = "AnantaNada-StageHub-HttpListener",
                IsBackground = true
            };
            _serverThread.Start();

            OnLog?.Invoke($"[StageHub Server] Online on port {_port}. WebRoot: {_webRootDirectory}");
            return true;
        }
        catch (HttpListenerException hlex)
        {
            // On Windows, listening on '*' without admin URLACL might require localhost prefix fallback
            try
            {
                _listener.Prefixes.Clear();
                _listener.Prefixes.Add($"http://localhost:{_port}/");
                _listener.Start();

                _isRunning = true;
                _serverThread = new Thread(ListenLoop)
                {
                    Name = "AnantaNada-StageHub-HttpListener",
                    IsBackground = true
                };
                _serverThread.Start();

                OnLog?.Invoke($"[StageHub Server] Online on localhost:{_port} (Fallback mode). WebRoot: {_webRootDirectory}");
                return true;
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"[StageHub Server] Start Failed: {ex.Message} (Initial error: {hlex.Message})");
                return false;
            }
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"[StageHub Server] Failed to start: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Returns all active IPv4 addresses to show as connection QR or pairing URLs on the host HUD.
    /// </summary>
    public static List<string> GetLocalIpAddresses()
    {
        var list = new List<string>();
        try
        {
            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (iface.OperationalStatus != OperationalStatus.Up ||
                    iface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                foreach (var addr in iface.GetIPProperties().UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        list.Add(addr.Address.ToString());
                    }
                }
            }
        }
        catch
        {
            list.Add("127.0.0.1");
        }
        return list;
    }

    private async void ListenLoop()
    {
        while (_isRunning && !_cts.Token.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    _ = ProcessWebSocketUpgradeAsync(context);
                }
                else
                {
                    _ = ProcessHttpRequestAsync(context);
                }
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"[StageHub Listener Error] {ex.Message}");
            }
        }
    }

    private async Task ProcessWebSocketUpgradeAsync(HttpListenerContext context)
    {
        HttpListenerWebSocketContext wsContext;
        try
        {
            wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Close();
            OnLog?.Invoke($"[StageHub WS Handshake Error] {ex.Message}");
            return;
        }

        var clientId = Guid.NewGuid();
        var socket = wsContext.WebSocket;
        _clients.TryAdd(clientId, socket);
        OnClientCountChanged?.Invoke(_clients.Count);
        OnLog?.Invoke($"[StageHub WS] Client connected [{clientId}]. Total: {_clients.Count}");

        var buffer = new byte[4096];

        try
        {
            while (socket.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
            {
                var receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    break;
                }

                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
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
            socket.Dispose();
            OnLog?.Invoke($"[StageHub WS] Client disconnected [{clientId}]. Remaining: {_clients.Count}");
        }
    }

    private void HandleClientJsonCommand(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("command", out var cmdProp))
            {
                string? command = cmdProp.GetString();

                switch (command)
                {
                    case "TRIGGER_PANIC":
                        OnLog?.Invoke("[StageHub Remote] Received TRIGGER_PANIC from remote mobile.");
                        OnPanicTriggered?.Invoke();
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
                deadSocket.Dispose();
            }
        }

        if (deadClients.Count > 0)
        {
            OnClientCountChanged?.Invoke(_clients.Count);
        }
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
                deadSocket.Dispose();
            }
        }
    }

    private async Task ProcessHttpRequestAsync(HttpListenerContext context)
    {
        try
        {
            string rawUrl = context.Request.Url?.AbsolutePath ?? "/";
            if (rawUrl == "/" || string.IsNullOrWhiteSpace(rawUrl))
            {
                rawUrl = "/index.html";
            }

            string relativePath = rawUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string fullPath = Path.Combine(_webRootDirectory, relativePath);

            if (!File.Exists(fullPath))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                byte[] notFoundBytes = Encoding.UTF8.GetBytes("404 Not Found - AnantaNada StageHub Remote");
                await context.Response.OutputStream.WriteAsync(notFoundBytes);
                context.Response.Close();
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

            context.Response.ContentType = mimeType;
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            using var fs = File.OpenRead(fullPath);
            await fs.CopyToAsync(context.Response.OutputStream);
            context.Response.Close();
        }
        catch (Exception ex)
        {
            try
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Close();
            }
            catch { }
            OnLog?.Invoke($"[StageHub HTTP Error] {ex.Message}");
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
            _listener.Stop();
            _listener.Close();
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
