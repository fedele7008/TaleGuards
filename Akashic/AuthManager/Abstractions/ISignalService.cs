namespace AuthManager.Abstractions;

public interface ISignalService
{
    IEnumerable<ServiceClient> GetClients();
    void RegisterService(Guid connectionId, ServiceClient client);
    void UnregisterService(Guid connectionId);
    Task BroadcastAsync(byte[] message);
    Task SendAsync(int serviceId, byte[] message);
}