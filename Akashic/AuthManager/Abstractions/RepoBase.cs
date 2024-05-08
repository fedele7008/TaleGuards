using System.Linq.Expressions;
using AuthManager.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthManager.Abstractions;

public abstract class RepoBase<T>(AkashicDbContext context) : IRepoBase<T>
    where T : class
{
    protected AkashicDbContext Context { get; set; } = context;

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => await Context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

    public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
        => await Context.SaveChangesAsync(ct) > 0;

    public IQueryable<T> FindAll()
        => Context.Set<T>().AsQueryable();

    public IQueryable<T> FindBy(Expression<Func<T, bool>> expression) 
        => Context.Set<T>().Where(expression).AsQueryable();

    public async Task<T?> GetByIdAsync(int id)
        => await Context.Set<T>().FindAsync(id);

    public async Task CreateAsync(T entity, CancellationToken ct = default)
        => await Context.Set<T>().AddAsync(entity, ct).ConfigureAwait(false);

    public void Update(T entity)
        => Context.Set<T>().Update(entity);

    public void Delete(T entity)
        => Context.Set<T>().Remove(entity);
}