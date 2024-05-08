using AuthManager.Models;

namespace AuthManager.Abstractions;

public interface IAccountRepo : IRepoBase<Account>
{
    Task<IEnumerable<Account>> GetAllAccountsAsync(CancellationToken ct = default);
    Task<bool> CheckEmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> CheckUsernameExistsAsync(string username, CancellationToken ct = default);
    Task<Account?> GetAccountByEmailAsync(string email, CancellationToken ct = default);
    Task<Account?> GetAccountByUsernameAsync(string username, CancellationToken ct = default);
    Task<IEnumerable<Account>> GetAllBannedAccountsByServiceAsync(int sid, CancellationToken ct = default);
    Task<IEnumerable<Account>> GetAllSuspendedAccountsByServiceAsync(int sid, CancellationToken ct = default);
    Task<Access?> GetAccountAccessByIdAsync(int sid, int uid, CancellationToken ct = default);
    Task<Access?> GetAccountAccessByUsernameAsync(int sid, string username, CancellationToken ct = default);
    Task<Access?> GetAccountAccessByEmailAsync(int sid, string email, CancellationToken ct = default);
    Task BanAccountByIdAsync(int sid, int id, CancellationToken ct = default);
    Task BanAccountByUsernameAsync(int sid, string username, CancellationToken ct = default);
    Task BanAccountByEmailAsync(int sid, string email, CancellationToken ct = default);
    Task UnbanAccountByIdAsync(int sid, int id, CancellationToken ct = default);
    Task UnbanAccountByUsernameAsync(int sid, string username, CancellationToken ct = default);
    Task UnbanAccountByEmailAsync(int sid, string email, CancellationToken ct = default);
    Task SuspendAccountByIdAsync(int sid, int id, DateTime suspensionEndDateTime, CancellationToken ct = default);
    Task SuspendAccountByUsernameAsync(int sid, string username, DateTime suspensionEndDateTime, CancellationToken ct = default);
    Task SuspendAccountByEmailAsync(int sid, string email, DateTime suspensionEndDateTime, CancellationToken ct = default);
    Task UnsuspendAccountByIdAsync(int sid, int id, CancellationToken ct = default);
    Task UnsuspendAccountByUsernameAsync(int sid, string username, CancellationToken ct = default);
    Task UnsuspendAccountByEmailAsync(int sid, string email, CancellationToken ct = default);
}
