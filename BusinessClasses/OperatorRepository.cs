using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Repository for Operator data access
/// </summary>
public class OperatorRepository
{
    private readonly SsrDbContext _context;

    public OperatorRepository(SsrDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all operators as IQueryable for filtering/sorting
    /// </summary>
    public IQueryable<Operator> GetOperatorsAsync()
    {
        return _context.Operators.AsNoTracking();
    }

    /// <summary>
    /// Get operator by ID
    /// </summary>
    public async Task<Operator?> GetByIdAsync(int operatorId)
    {
        return await _context.Operators
            .FirstOrDefaultAsync(o => o.OperatorID == operatorId);
    }

    /// <summary>
    /// Add new operator
    /// </summary>
    public async Task<Operator> AddAsync(Operator operatorEntity)
    {
        _context.Operators.Add(operatorEntity);
        await _context.SaveChangesAsync();
        return operatorEntity;
    }

    /// <summary>
    /// Update existing operator
    /// </summary>
    public async Task<Operator> UpdateAsync(Operator operatorEntity)
    {
        _context.Operators.Update(operatorEntity);
        await _context.SaveChangesAsync();
        return operatorEntity;
    }

    /// <summary>
    /// Delete operator
    /// </summary>
    public async Task DeleteAsync(Operator operatorEntity)
    {
        _context.Operators.Remove(operatorEntity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Check if operator name already exists
    /// </summary>
    public async Task<bool> OperatorNameExistsAsync(string operatorName, int? excludeOperatorId = null)
    {
        var query = _context.Operators
            .Where(o => o.OperatorName == operatorName);

        if (excludeOperatorId.HasValue)
        {
            query = query.Where(o => o.OperatorID != excludeOperatorId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Get operator by ID with contacts
    /// </summary>
    public async Task<Operator?> GetWithContactsAsync(int operatorId)
    {
        return await _context.Operators
            .Include(o => o.OperatorContacts)
            .FirstOrDefaultAsync(o => o.OperatorID == operatorId);
    }

    /// <summary>
    /// Check if operator has associated acquisitions
    /// </summary>
    public async Task<bool> HasAssociatedAcquisitionsAsync(int operatorId)
    {
        return await _context.AcquisitionOperators
            .AnyAsync(ao => ao.OperatorID == operatorId);
    }

    #region Operator Contact Operations

    /// <summary>
    /// Get contacts for an operator
    /// </summary>
    public async Task<List<OperatorContact>> GetContactsByOperatorIdAsync(int operatorId)
    {
        return await _context.OperatorContacts
            .Where(oc => oc.OperatorID == operatorId)
            .OrderBy(oc => oc.ContactName)
            .ToListAsync();
    }

    /// <summary>
    /// Get contact by ID
    /// </summary>
    public async Task<OperatorContact?> GetContactByIdAsync(int contactId)
    {
        return await _context.OperatorContacts
            .FirstOrDefaultAsync(oc => oc.OperatorContactID == contactId);
    }

    /// <summary>
    /// Add new contact
    /// </summary>
    public async Task<OperatorContact> AddContactAsync(OperatorContact contact)
    {
        _context.OperatorContacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    /// <summary>
    /// Update existing contact
    /// </summary>
    public async Task<OperatorContact> UpdateContactAsync(OperatorContact contact)
    {
        _context.OperatorContacts.Update(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    /// <summary>
    /// Delete contact
    /// </summary>
    public async Task DeleteContactAsync(OperatorContact contact)
    {
        _context.OperatorContacts.Remove(contact);
        await _context.SaveChangesAsync();
    }

    #endregion
}