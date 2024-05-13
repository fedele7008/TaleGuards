using System.Collections.Concurrent;
using AuthManager.Abstractions;

namespace AuthManager.ServiceLinker;

public class SignalService : ISignalService
{
    private ConcurrentDictionary<Guid, ServiceClient> Clients { get; set; } = new();

    public IEnumerable<ServiceClient> GetClients() => Clients.Values;

    public void RegisterService(Guid connectionId, ServiceClient client) 
        => Clients.TryAdd(connectionId, client);

    public void UnregisterService(Guid connectionId) 
        => Clients.TryRemove(connectionId, out _);

    public async Task BroadcastAsync(byte[] message)
    {
        foreach (var client in Clients.Values)
        {
            if (!client.Connection.Connected) continue;
            var stream = client.Connection.GetStream();
            await stream.WriteAsync(message);
        }
    }

    public async Task SendAsync(int serviceId, byte[] message)
    {
        foreach (var client in Clients.Values)
        {
            if (client.ServiceId != serviceId || !client.Connection.Connected) continue;
            var stream = client.Connection.GetStream();
            await stream.WriteAsync(message);
        }
    }
}