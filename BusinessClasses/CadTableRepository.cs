using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class CadTableRepository
{
    private readonly IDbContextFactory<SsrDbContext> _contextFactory;

    public CadTableRepository(IDbContextFactory<SsrDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<CadTable>> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.CadTables
            .Where(t => t.IncludeInSearch)
            .OrderBy(t => t.DisplayName)
            .ToListAsync();
    }
    
    public async Task<CadTable?> GetByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.CadTables.FindAsync(id);
    }
}
