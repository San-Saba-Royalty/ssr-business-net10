using SSRBusiness.Data;
using SSRBusiness.Entities;
using SSRBusiness.BusinessFramework;
using Microsoft.EntityFrameworkCore;

namespace SSRBusiness.BusinessClasses;

public class LienTypeRepository : Repository<LienType>
{
    public LienTypeRepository(SsrDbContext context) : base(context) { }

    public async Task DeleteWithGuardAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;
        
        var isUsed = await ((SsrDbContext)Context).AcquisitionLiens.AnyAsync(x => x.LienTypeID == id);
        if (isUsed) throw new InvalidOperationException("Cannot delete Lien Type because it is in use by one or more Acquisitions.");
        
        Delete(entity);
        await SaveChangesAsync();
    }
}

public class FolderLocationRepository : Repository<FolderLocation>
{
    public FolderLocationRepository(SsrDbContext context) : base(context) { }

    public async Task DeleteWithGuardAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;
        
        var isUsed = await ((SsrDbContext)Context).Acquisitions.AnyAsync(x => x.FolderLocation == entity.FolderLocationText);
        if (isUsed) throw new InvalidOperationException("Cannot delete Folder Location because it is in use by one or more Acquisitions.");
        
        Delete(entity);
        await SaveChangesAsync();
    }
}

public class DealStatusRepository : Repository<DealStatus>
{
    public DealStatusRepository(SsrDbContext context) : base(context) { }

    public async Task DeleteWithGuardAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;
        
        var isUsed = await ((SsrDbContext)Context).AcquisitionStatuses.AnyAsync(x => x.DealStatusID == id);
        if (isUsed) throw new InvalidOperationException("Cannot delete Deal Status because it is in use by one or more Acquisitions.");
        
        Delete(entity);
        await SaveChangesAsync();
    }
}

public class CurativeTypeRepository : Repository<CurativeType>
{
    public CurativeTypeRepository(SsrDbContext context) : base(context) { }

    public async Task DeleteWithGuardAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;
        
        var isUsed = await ((SsrDbContext)Context).AcqCurativeRequirements.AnyAsync(x => x.CurativeTypeID == id);
        if (isUsed) throw new InvalidOperationException("Cannot delete Curative Type because it is in use by one or more Acquisitions.");
        
        Delete(entity);
        await SaveChangesAsync();
    }
}

public class LetterAgreementDealStatusRepository : Repository<LetterAgreementDealStatus>
{
    public LetterAgreementDealStatusRepository(SsrDbContext context) : base(context) { }

    public async Task DeleteWithGuardAsync(string id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;
        
        // Note: LetterAgreementStatus uses DealStatusCode as well
        var isUsed = await ((SsrDbContext)Context).LetterAgreementStatuses.AnyAsync(x => x.DealStatusCode == id);
        if (isUsed) throw new InvalidOperationException("Cannot delete Deal Status because it is in use by one or more Letter Agreements.");
        
        Delete(entity);
        await SaveChangesAsync();
    }
}

public class PermissionRepository : Repository<Permission>
{
    public PermissionRepository(SsrDbContext context) : base(context) { }

    public async Task DeleteWithGuardAsync(string id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return;
        
        var isUsed = await ((SsrDbContext)Context).RolePermissions.AnyAsync(x => x.PermissionCode == id);
        if (isUsed) throw new InvalidOperationException("Cannot delete Permission because it is assigned to one or more Roles.");
        
        Delete(entity);
        await SaveChangesAsync();
    }
}
