using AuthManager.Models;

namespace AuthManager.Abstractions;

public interface IServiceRepo : IRepoBase<Service>
{
    Task<IEnumerable<Service>> GetAllServicesAsync(CancellationToken ct = default);
    Task<IEnumerable<int>> GetAllServiceIdsAsync(CancellationToken ct = default);
    Task<bool> CheckServiceExistsAsync(string name, CancellationToken ct = default);
    Task<Service?> GetServiceByNameAsync(string name, CancellationToken ct = default);
}