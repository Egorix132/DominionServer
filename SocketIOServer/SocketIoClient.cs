using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Packet;
using SocketIOSharp.Server.Client;

namespace Dominion.SocketIoServer;

public class SocketIoClient
{
    private readonly SocketIOSocket _socket;
    private readonly List<(string, Action<JToken[]>)> _listeners;
    private readonly List<(string, Action<SocketIOAckEvent>)> _askListeners;

    public Guid Id { get; }

    public SocketIoClient(SocketIOSocket socket)
    {
        Id = Guid.NewGuid();
        _socket = socket;
        _listeners = new List<(string, Action<JToken[]>)>();
        _askListeners = new List<(string, Action<SocketIOAckEvent>)>();
    }

    public void Dispose()
    {
        foreach (var tuple in _listeners)
        {
            _socket.Off(tuple.Item1, tuple.Item2);
        }
        foreach (var tuple in _askListeners)
        {
            _socket.Off(tuple.Item1, tuple.Item2);
        }

        _listeners.Clear();
        _socket.Close();
        _socket.Dispose();
    }

    public void SendMessage(string endpoint)
    {
        _socket.Emit(endpoint);
    }

    public void SendMessage(string endpoint, params object?[] data)
    {
        _socket.Emit(endpoint, data);
    }
    public async Task<JToken[]> AskAsync(string endpoint, params object?[] data)
    {
        return await _socket.AskAsync(endpoint, data);
    }

    public void ListenToMessage(string endpoint, Action<JToken[]> listener)
    {
        _socket.On(endpoint, listener);
        _listeners.Add((endpoint, listener));
    }

    public void OffMessage(string endpoint, Action<JToken[]> listener)
    {
        _socket.Off(endpoint, listener);
        _listeners.Remove((endpoint, listener));
    }

    public void ListenToAsk(string endpoint, Action<SocketIOAckEvent> listener)
    {
        _socket.On(endpoint, listener);
        _askListeners.Add((endpoint, listener));
    }

    public void OffAsk(string endpoint, Action<SocketIOAckEvent> listener)
    {
        _socket.Off(endpoint, listener);
        _askListeners.Remove((endpoint, listener));
    }

    public override string ToString()
    {
        return $"{{ Id: {Id} }}";
    }

    protected bool Equals(SocketIoClient other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SocketIoClient) obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}