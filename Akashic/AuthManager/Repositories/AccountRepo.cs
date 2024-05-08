using AuthManager.Abstractions;
using AuthManager.Data;
using AuthManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthManager.Repositories;

public class AccountRepo(AkashicDbContext context) : RepoBase<Account>(context), IAccountRepo
{
    public async Task<IEnumerable<Account>> GetAllAccountsAsync(CancellationToken ct = default) 
        => await FindAll().ToListAsync(ct).ConfigureAwait(false);

    public async Task<bool> CheckEmailExistsAsync(string email, CancellationToken ct = default)
        => await FindBy(a => a.Email == email)
            .AnyAsync(ct)
            .ConfigureAwait(false);

    public async Task<bool> CheckUsernameExistsAsync(string username, CancellationToken ct = default) 
        => await FindBy(a => a.Username == username)
            .AnyAsync(ct)
            .ConfigureAwait(false);

    public async Task<Account?> GetAccountByEmailAsync(string email, CancellationToken ct = default)
        => await FindBy(a => a.Email == email)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

    public async Task<Account?> GetAccountByUsernameAsync(string username, CancellationToken ct = default) 
        => await FindBy(a => a.Username == username)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

    public async Task<IEnumerable<Account>> GetAllBannedAccountsByServiceAsync(int sid, CancellationToken ct = default) 
        => await Context.Accesses
            .Where(access => access.Sid == sid && access.Banned)
            .Select(access => access.Account!)
            .ToListAsync(ct)
            .ConfigureAwait(false);

    public async Task<IEnumerable<Account>> GetAllSuspendedAccountsByServiceAsync(int sid, CancellationToken ct = default) 
        => await Context.Accesses
            .Where(access => access.Sid == sid && access.SuspensionEndAt != null)
            .Select(access => access.Account!)
            .ToListAsync(ct)
            .ConfigureAwait(false);

    public async Task<Access?> GetAccountAccessByIdAsync(int sid, int uid, CancellationToken ct = default) 
        => await Context.Accesses
            .FirstOrDefaultAsync(access => access.Sid == sid && access.Uid == uid, ct)
            .ConfigureAwait(false);

    public async Task<Access?> GetAccountAccessByUsernameAsync(int sid, string username, CancellationToken ct = default) 
        => await Context.Accesses
            .FirstOrDefaultAsync(access => access.Sid == sid && access.Account!.Username == username, ct)
            .ConfigureAwait(false);

    public async Task<Access?> GetAccountAccessByEmailAsync(int sid, string email, CancellationToken ct = default)
        => await Context.Accesses
            .FirstOrDefaultAsync(access => access.Sid == sid && access.Account!.Email == email, ct)
            .ConfigureAwait(false);

    public async Task BanAccountByIdAsync(int sid, int id, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Uid == id, ct)
            .ContinueWith(t => t.Result.Banned = true, ct)
            .ConfigureAwait(false);

    public async Task BanAccountByUsernameAsync(int sid, string username, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Account!.Username == username, ct)
            .ContinueWith(t => t.Result.Banned = true, ct)
            .ConfigureAwait(false);

    public async Task BanAccountByEmailAsync(int sid, string email, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Account!.Email == email, ct)
            .ContinueWith(t => t.Result.Banned = true, ct)
            .ConfigureAwait(false);

    public async Task UnbanAccountByIdAsync(int sid, int id, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Uid == id, ct)
            .ContinueWith(t => t.Result.Banned = false, ct)
            .ConfigureAwait(false);

    public async Task UnbanAccountByUsernameAsync(int sid, string username, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Account!.Username == username, ct)
            .ContinueWith(t => t.Result.Banned = false, ct)
            .ConfigureAwait(false);

    public async Task UnbanAccountByEmailAsync(int sid, string email, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Account!.Email == email, ct)
            .ContinueWith(t => t.Result.Banned = false, ct)
            .ConfigureAwait(false);

    public async Task SuspendAccountByIdAsync(int sid, int id, DateTime suspensionEndDateTime, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Uid == id, ct)
            .ContinueWith(t => t.Result.SuspensionEndAt = suspensionEndDateTime, ct)
            .ConfigureAwait(false);

    public async Task SuspendAccountByUsernameAsync(int sid, string username, DateTime suspensionEndDateTime,
        CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Account!.Username == username, ct)
            .ContinueWith(t => t.Result.SuspensionEndAt = suspensionEndDateTime, ct)
            .ConfigureAwait(false);

    public async Task SuspendAccountByEmailAsync(int sid, string email, DateTime suspensionEndDateTime, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Account!.Email == email, ct)
            .ContinueWith(t => t.Result.SuspensionEndAt = suspensionEndDateTime, ct)
            .ConfigureAwait(false);

    public async Task UnsuspendAccountByIdAsync(int sid, int id, CancellationToken ct = default) 
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Uid == id, ct)
            .ContinueWith(t => t.Result.SuspensionEndAt = null, ct)
            .ConfigureAwait(false);

    public async Task UnsuspendAccountByUsernameAsync(int sid, string username, CancellationToken ct = default)
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Account!.Username == username, ct)
            .ContinueWith(t => t.Result.SuspensionEndAt = null, ct)
            .ConfigureAwait(false);

    public async Task UnsuspendAccountByEmailAsync(int sid, string email, CancellationToken ct = default)
        => await Context.Accesses
            .SingleAsync(access => access.Sid == sid && access.Account!.Email == email, ct)
            .ContinueWith(t => t.Result.SuspensionEndAt = null, ct)
            .ConfigureAwait(false);
}