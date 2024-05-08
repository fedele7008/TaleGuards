using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthManager.Abstractions;

public interface IRepoBase<T>
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
    IQueryable<T> FindAll();
    IQueryable<T> FindBy(Expression<Func<T, bool>> expression);
    Task<T?> GetByIdAsync(int id);
    Task CreateAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
}
