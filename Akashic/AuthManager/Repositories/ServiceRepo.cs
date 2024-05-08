using AuthManager.Abstractions;
using AuthManager.Data;
using AuthManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthManager.Repositories;

public class ServiceRepo(AkashicDbContext context) : RepoBase<Service>(context), IServiceRepo
{
    public async Task<IEnumerable<Service>> GetAllServicesAsync(CancellationToken ct = default)
        => await FindAll().ToListAsync(ct).ConfigureAwait(false);

    public async Task<IEnumerable<int>> GetAllServiceIdsAsync(CancellationToken ct = default) 
        => await FindAll()
            .Select(service => service.Sid)
            .ToListAsync(ct)
            .ConfigureAwait(false);

    public async Task<bool> CheckServiceExistsAsync(string name, CancellationToken ct = default)
        => await FindBy(s => s.Name == name)
            .AnyAsync(ct)
            .ConfigureAwait(false);
    
    public async Task<Service?> GetServiceByNameAsync(string name, CancellationToken ct = default)
        => await FindBy(s => s.Name == name)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);
}