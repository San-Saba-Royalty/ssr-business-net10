using Microsoft.EntityFrameworkCore;
using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class FilterRepository : BaseRepository<Filter>
{
    public FilterRepository(SsrDbContext context) : base(context)
    {
    }

    public async Task<IQueryable<Filter>> GetFiltersAsync()
    {
        return await Task.FromResult<IQueryable<Filter>>(DbSet.AsQueryable());
    }

    public async Task<Filter?> GetFilterByIdAsync(int filterId)
    {
        return await DbSet.FirstOrDefaultAsync(f => f.FilterID == filterId);
    }

    public async Task<List<Filter>> GetAllFiltersAsync()
    {
        return await DbSet.OrderBy(f => f.FilterName).ToListAsync();
    }
}