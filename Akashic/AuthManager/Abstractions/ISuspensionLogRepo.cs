using AuthManager.Models;

namespace AuthManager.Abstractions;

public interface ISuspensionLogRepo : IRepoBase<SuspensionLog>
{
    Task<IEnumerable<SuspensionLog>> GetAllSuspensionLogsAsync(CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByAccountIdAsync(int accountId, CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByUsernameAsync(string username, CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByServiceIdAsync(int serviceId, CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByTypeAsync(string type, CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetActionLogsByAccountIdAsync(int accountId, CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetActionLogsByUsernameAsync(string username, CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetActionLogsByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<SuspensionLog>> GetActionLogsBySystemAsync(CancellationToken ct = default);
}