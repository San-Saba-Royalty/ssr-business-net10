using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class BuyerRepository : BaseRepository<Buyer>
{
    public BuyerRepository(SsrDbContext context) : base(context)
    {
    }
    
    public async Task<IQueryable<Buyer>> GetBuyersAsync()
    {
        return await Task.FromResult<IQueryable<Buyer>>(DbSet.AsQueryable());
    }
}