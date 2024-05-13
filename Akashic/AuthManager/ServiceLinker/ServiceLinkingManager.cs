using System.Net;
using System.Net.Sockets;
using System.Text;
using AuthManager.Abstractions;

namespace AuthManager.ServiceLinker;

public class ServiceLinkingManager(
    int port,
    ISignalService signalService,
    ILogger<ServiceLinkingManager> logger,
    IRsaCryptoService rsaManager)
{
    private int Port { get; init; } = port;
    private ISignalService SignalService { get; init; } = signalService;

    public async Task Start(CancellationToken ct = default)
    {
        var listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        logger.LogInformation("Service linking manager started, listening on port {Port} (TCP)", Port);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var service = await listener.AcceptTcpClientAsync(ct);
                var connectionId = Guid.NewGuid();
                logger.LogDebug("New service connection attempted (connectionId: {ConnectionId})", connectionId);
                _ = HandleServiceConnection(connectionId, service, ct);
            }
        }
        finally
        {
            listener.Stop();
            foreach (var service in SignalService.GetClients())
            {
                service.Connection.Close();
            }
            logger.LogInformation("Service linking manager stopped");
        }
    }

    private static async Task ReadExactAsync(Stream stream, Memory<byte> buffer,
        CancellationToken ct = default)
    {
        var memoryRead = 0;
        while (memoryRead < buffer.Length)
        {
            var byteRead = await stream.ReadAsync(buffer[memoryRead..], ct);
            if (byteRead == 0)
                throw new IOException("Disconnected");
            memoryRead += byteRead;
        }
    }

    private static async Task<string> ReadStringMessageAsync(Stream stream, CancellationToken ct = default)
    {
        Memory<byte> lengthBuffer = new byte[4];
        await ReadExactAsync(stream, lengthBuffer, ct);
        var lengthBufferSpan = lengthBuffer.Span.ToArray();
        if (BitConverter.IsLittleEndian)
            Array.Reverse(lengthBufferSpan);
        var msgLength = BitConverter.ToInt32(lengthBufferSpan);
        
        Memory<byte> msgBuffer = new byte[msgLength];
        await ReadExactAsync(stream, msgBuffer, ct);
        
        var msgBufferSpan = msgBuffer.Span.ToArray();
        return Encoding.UTF8.GetString(msgBufferSpan);
    }

    private async Task HandleServiceConnection(Guid connectionId, TcpClient client, CancellationToken ct = default)
    {
        try
        {
            using (client)
            await using (var stream = client.GetStream())
            {
                var timeout = Task.Delay(TimeSpan.FromSeconds(10), ct);
                var validationTask = ReadStringMessageAsync(stream, ct);
                var doneTask = await Task.WhenAny(timeout, validationTask).ConfigureAwait(false);
                if (doneTask == timeout)
                {
                    logger.LogWarning("Connection timeout (connectionId: {connectionId})", connectionId);
                    await stream.WriteAsync("Connection timeout"u8.ToArray(), ct);
                    client.Close();
                    return;
                }
                
                var messageEncrypted = validationTask.Result;
                if (messageEncrypted != "Test")
                {
                    logger.LogWarning("Connection refused (connectionId: {connectionId})", connectionId);
                    await stream.WriteAsync("Connection refused"u8.ToArray(), ct);
                    client.Close();
                    return;
                }
                await stream.WriteAsync("Connection accepted"u8.ToArray(), ct);
                
                SignalService.RegisterService(connectionId, new ServiceClient(client));
                logger.LogInformation("Service connected (connectionId: {connectionId})", connectionId);
                while (true)
                {
                    var request = await ReadStringMessageAsync(stream, ct);
                    var upper = request.ToUpper();
                    var response = Encoding.UTF8.GetBytes(upper);
                    await stream.WriteAsync(response, ct);
                }
            }
        }
        catch (IOException)
        {
            logger.LogInformation("Service disconnected (connectionId: {connectionId})", connectionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handling service connection (connectionId: {connectionId})", connectionId);
        }
        finally
        {
            SignalService.UnregisterService(connectionId);
            client.Close();
        }
    }
}
