using Microsoft.EntityFrameworkCore;
using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class ReferrerFormRepository : BaseRepository<ReferrerForm>
{
    public ReferrerFormRepository(SsrDbContext context) : base(context)
    {
    }

    public async Task<List<ReferrerForm>> GetFormsByReferrerIdAsync(int referrerId)
    {
        return await DbSet
            .Where(f => f.ReferrerID == referrerId)
            .OrderByDescending(f => f.FormYear)
            .ToListAsync();
    }

    public async Task<bool> FormYearExistsAsync(int referrerId, string year, string formTypeCode)
    {
        return await DbSet.AnyAsync(f => f.ReferrerID == referrerId && f.FormYear == year && f.FormTypeCode == formTypeCode);
    }
}
