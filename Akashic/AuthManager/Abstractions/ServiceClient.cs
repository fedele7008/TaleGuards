using System.Net.Sockets;

namespace AuthManager.Abstractions;

public record ServiceClient(TcpClient Connection)
{
    public int? ServiceId { get; set; } = null;
    public TcpClient Connection { get; set; } = Connection;
}