using CreditFinancialOrganization.Domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CreditFinancialOrganization.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    TEntity Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);

    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = default,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
        CancellationToken cancellationToken = default);

    Task<PagedList<TEntity>> GetAllAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
