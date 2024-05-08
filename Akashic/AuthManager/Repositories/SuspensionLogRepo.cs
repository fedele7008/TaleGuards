using AuthManager.Abstractions;
using AuthManager.Data;
using AuthManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthManager.Repositories;

public class SuspensionLogRepo(AkashicDbContext context) : RepoBase<SuspensionLog>(context), ISuspensionLogRepo
{
    public async Task<IEnumerable<SuspensionLog>> GetAllSuspensionLogsAsync(CancellationToken ct = default)
    {
        return await FindAll().ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByAccountIdAsync(int accountId, CancellationToken ct = default)
    {
        return await FindBy(log => log.AssigneeAccount != null && log.AssigneeAccount.Uid == accountId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByUsernameAsync(string username, CancellationToken ct = default)
    {
        return await FindBy(log => log.AssigneeAccount != null && log.AssigneeAccount.Username == username)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await FindBy(log => log.AssigneeAccount != null && log.AssigneeAccount.Email == email)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByServiceIdAsync(int serviceId, CancellationToken ct = default)
    {
        return await FindBy(log => log.Service != null && log.Service.Sid == serviceId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetSuspensionLogsByTypeAsync(string type, CancellationToken ct = default)
    {
        return await FindBy(log => log.Type == type)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetActionLogsByAccountIdAsync(int accountId, CancellationToken ct = default)
    {
        return await FindBy(log => log.AssignerAccount != null && log.AssignerAccount.Uid == accountId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetActionLogsByUsernameAsync(string username, CancellationToken ct = default)
    {
        return await FindBy(log => log.AssignerAccount != null && log.AssignerAccount.Username == username)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetActionLogsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await FindBy(log => log.AssignerAccount != null && log.AssignerAccount.Email == email)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SuspensionLog>> GetActionLogsBySystemAsync(CancellationToken ct = default)
    {
        return await FindBy(log => log.AssignerAccount == null)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}