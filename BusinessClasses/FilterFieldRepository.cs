using Microsoft.EntityFrameworkCore;
using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Repository for FilterField entity operations
/// </summary>
public class FilterFieldRepository : BaseRepository<FilterField>
{
    public FilterFieldRepository(SsrDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get all filter fields for a specific filter
    /// </summary>
    public async Task<List<FilterField>> GetByFilterIdAsync(int filterId)
    {
        return await DbSet
            .Where(ff => ff.FilterID == filterId)
            .ToListAsync();
    }

    /// <summary>
    /// Delete all filter fields for a specific filter
    /// </summary>
    public async Task DeleteByFilterIdAsync(int filterId)
    {
        var fields = await DbSet
            .Where(ff => ff.FilterID == filterId)
            .ToListAsync();

        DbSet.RemoveRange(fields);
    }

    /// <summary>
    /// Bulk insert filter fields
    /// </summary>
    public async Task AddRangeAsync(IEnumerable<FilterField> filterFields)
    {
        await DbSet.AddRangeAsync(filterFields);
    }
}
