using System.Linq.Expressions;
using CreditFinancialOrganization.Domain.Models;
using Microsoft.EntityFrameworkCore;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Query;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly ApplicationDbContext DbContext;

    protected BaseRepository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public TEntity Add(TEntity entity)
    {
        return DbContext.Set<TEntity>().Add(entity).Entity;
    }

    public void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    public async Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await GetQueryable(predicate, include)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedList<TEntity>> GetAllAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return await PagedList<TEntity>.CreateAsync(
            GetQueryable(predicate, include),
            pageNumber,
            pageSize,
            cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>()
            .AnyAsync(predicate, cancellationToken);
    }

    private IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        var query = DbContext.Set<TEntity>().AsNoTracking();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (include != null)
        {
            query = include(query);
        }

        return query;
    }
}