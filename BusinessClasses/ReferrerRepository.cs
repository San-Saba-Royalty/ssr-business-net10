using Microsoft.EntityFrameworkCore;
using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class ReferrerRepository : BaseRepository<Referrer>
{
    public ReferrerRepository(SsrDbContext context) : base(context)
    {
    }
    
    public async Task<bool> DoesReferrerNameExistAsync(string referrerName, int excludeReferrerId = 0)
    {
        return await ((SsrDbContext)Context).Referrers.AnyAsync(r => r.ReferrerName == referrerName && r.ReferrerID != excludeReferrerId);
    }

    public IQueryable<Referrer> GetReferrersQuery()
    {
        return ((SsrDbContext)Context).Referrers.AsQueryable();
    }
    
    public async Task DeleteWithGuardAsync(int referrerId)
    {
        var context = (SsrDbContext)Context;
        
        var hasAcquisitions = await context.AcquisitionReferrers.AnyAsync(a => a.ReferrerID == referrerId);
        var hasLetterAgreements = await context.LetterAgreementReferrers.AnyAsync(la => la.ReferrerID == referrerId);

        if (hasAcquisitions || hasLetterAgreements)
        {
            var msg = new System.Text.StringBuilder("Unable to delete. ");
            if (hasAcquisitions && hasLetterAgreements)
                msg.Append("There are letter agreements and acquisitions tied to this Referrer.");
            else if (hasAcquisitions)
                msg.Append("There are acquisitions tied to this Referrer.");
            else
                msg.Append("There are letter agreements tied to this Referrer.");
                
            throw new InvalidOperationException(msg.ToString());
        }

        // Logic to delete DocuShare collection if needed - will be handled by Service or Repository? 
        // Legacy had separate DeleteReferrerDocuShare.
        // Repository should just delete data. Service handles file cleanup.
        
        await DeleteAsync(r => r.ReferrerID == referrerId);
        await SaveChangesAsync();
    }
}