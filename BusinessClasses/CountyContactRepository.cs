using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRBusiness.BusinessClasses;

public class CountyContactRepository
{
    private readonly IDbContextFactory<SsrDbContext> _contextFactory;

    public CountyContactRepository(IDbContextFactory<SsrDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<CountyContact>> GetByCountyIdAsync(int countyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.CountyContacts
            .Include(c => c.County)
            .Where(c => c.CountyID == countyId)
            .ToListAsync();
    }

    public async Task<CountyContact?> GetByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.CountyContacts
            .Include(c => c.County)
            .FirstOrDefaultAsync(c => c.CountyContactID == id);
    }

    public async Task AddAsync(CountyContact contact)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.CountyContacts.Add(contact);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CountyContact contact)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Entry(contact).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var contact = await context.CountyContacts.FindAsync(id);
        if (contact != null)
        {
            context.CountyContacts.Remove(contact);
            await context.SaveChangesAsync();
        }
    }
}
