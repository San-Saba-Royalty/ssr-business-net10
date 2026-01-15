using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class CountyAppraisalGroupRepository
{
    private readonly SsrDbContext _context;

    public CountyAppraisalGroupRepository(SsrDbContext context)
    {
        _context = context;
    }

    public async Task<List<CountyAppraisalGroup>> GetByCountyIdAsync(int countyId)
    {
        return await _context.CountyAppraisalGroups
            .Include(c => c.AppraisalGroup)
            .Where(c => c.CountyID == countyId)
            .OrderByDescending(c => c.EffectiveDate)
            .ToListAsync();
    }

    public async Task<CountyAppraisalGroup?> GetByIdAsync(int id)
    {
        return await _context.CountyAppraisalGroups
            .Include(c => c.AppraisalGroup)
            .FirstOrDefaultAsync(c => c.CountyAppraisalGroupID == id);
    }
    
    public async Task<bool> ExistsForDateAsync(int countyId, DateTime effectiveDate, int? excludeId = null)
    {
        var query = _context.CountyAppraisalGroups
            .Where(c => c.CountyID == countyId && c.EffectiveDate.Date == effectiveDate.Date);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.CountyAppraisalGroupID != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(CountyAppraisalGroup entity)
    {
        _context.CountyAppraisalGroups.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.CountyAppraisalGroups.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
