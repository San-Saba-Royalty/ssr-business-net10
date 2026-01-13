using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SSRBusiness.BusinessFramework;

/// <summary>
/// Base repository class for entity operations using Entity Framework Core.
/// Replaces the old LINQ to SQL SsrBusinessObject pattern.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public class Repository<TEntity> where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Get entity by ID
    /// </summary>
    public virtual async Task<TEntity?> GetByIdAsync(params object[] keyValues)
    {
        return await DbSet.FindAsync(keyValues);
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    /// <summary>
    /// Find entities matching a predicate
    /// </summary>
    public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Get a single entity matching a predicate
    /// </summary>
    public virtual async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.SingleOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Add a new entity
    /// </summary>
    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Update an existing entity
    /// </summary>
    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    /// <summary>
    /// Delete an entity
    /// </summary>
    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    /// <summary>
    /// Delete entities matching a predicate
    /// </summary>
    public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = await DbSet.Where(predicate).ToListAsync();
        DbSet.RemoveRange(entities);
        return entities.Count;
    }

    /// <summary>
    /// Check if any entity matches the predicate
    /// </summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.AnyAsync(predicate);
    }

    /// <summary>
    /// Get count of entities matching predicate
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return predicate == null 
            ? await DbSet.CountAsync() 
            : await DbSet.CountAsync(predicate);
    }

    /// <summary>
    /// Save changes to the database
    /// </summary>
    public virtual async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Get queryable for advanced queries
    /// </summary>
    public virtual IQueryable<TEntity> Query()
    {
        return DbSet.AsQueryable();
    }
}
