using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class LetterAgreementRepository : BaseRepository<LetterAgreement>
{
    public LetterAgreementRepository(SsrDbContext context) : base(context)
    {
    }

    public async Task<IQueryable<LetterAgreement>> GetLetterAgreementsAsync()
    {
        return await Task.FromResult<IQueryable<LetterAgreement>>(DbSet.AsQueryable());
    }
}
