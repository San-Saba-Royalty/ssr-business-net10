using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;

namespace SSRBusiness.BusinessClasses;

public abstract class BaseRepository<T> : Repository<T> where T : class
{
    private readonly SsrDbContext _context;

    public BaseRepository(SsrDbContext context) : base(context)
    {
        _context = context;
    }
}