using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;
using SSRBusiness.BusinessFramework;

namespace SSRBusiness.BusinessClasses;

public class RoleRepository : Repository<Role>
{
    private readonly SsrDbContext _ssrContext;

    public RoleRepository(SsrDbContext context) : base(context)
    {
        _ssrContext = context;
    }

    public override async Task<Role?> GetByIdAsync(params object[] keyValues)
    {
        if (keyValues.Length > 0 && keyValues[0] is int roleId)
        {
            return await _ssrContext.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }
        return await base.GetByIdAsync(keyValues);
    }

    public async Task UpdatePermissionsAsync(int roleId, IEnumerable<string> selectedPermissionCodes)
    {
        var role = await _ssrContext.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);
            
        if (role == null) throw new KeyNotFoundException($"Role with ID {roleId} not found");

        var currentCodes = role.RolePermissions.Select(rp => rp.PermissionCode).ToHashSet();
        var selectedCodes = new HashSet<string>(selectedPermissionCodes);

        var toRemove = role.RolePermissions.Where(rp => !selectedCodes.Contains(rp.PermissionCode)).ToList();
        
        foreach (var item in toRemove)
        {
            role.RolePermissions.Remove(item);
        }

        var toAdd = selectedCodes.Where(code => !currentCodes.Contains(code));
        
        foreach (var code in toAdd)
        {
             role.RolePermissions.Add(new RolePermission { RoleID = roleId, PermissionCode = code });
        }
        
        await _ssrContext.SaveChangesAsync();
    }
    public async Task DeleteWithGuardAsync(int roleId)
    {
        var role = await GetByIdAsync(roleId);
        if (role == null) return;
        
        var isUsed = await _ssrContext.UserRoles.AnyAsync(ur => ur.RoleId == roleId);
        if (isUsed)
            throw new InvalidOperationException("Cannot delete Role because it is assigned to one or more users.");
            
        Delete(role);
        await SaveChangesAsync();
    }
}
