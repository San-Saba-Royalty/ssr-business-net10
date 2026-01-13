using Microsoft.EntityFrameworkCore;
using SSRBusiness.BusinessFramework;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

public class DocumentTemplateRepository : BaseRepository<DocumentTemplate>
{
    public DocumentTemplateRepository(SsrDbContext context) : base(context)
    {
    }

    public async Task<IQueryable<DocumentTemplate>> GetDocumentTemplatesAsync()
    {
        return await Task.FromResult<IQueryable<DocumentTemplate>>(DbSet.Include(x => x.CustomFields));
    }

    public async Task<DocumentTemplate?> GetByIdAsync(int id)
    {
        return await DbSet.Include(x => x.CustomFields)
                          .FirstOrDefaultAsync(x => x.DocumentTemplateID == id);
    }
}