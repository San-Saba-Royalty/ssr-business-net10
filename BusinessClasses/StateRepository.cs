using System.ComponentModel.DataAnnotations;
using SSRBusiness.Data;
using SSRBusiness.Entities;
using SSRBusiness.BusinessFramework;
using SSRBusiness.Support;
using Microsoft.EntityFrameworkCore;

namespace SSRBusiness.BusinessClasses;

/// <summary>
/// Repository for State entity operations
/// </summary>
public class StateRepository : Repository<State>
{
    private readonly SsrDbContext _context;
    public StateRepository(SsrDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns a specific state by state code
    /// </summary>
    /// <param name="stateCode">State code to lookup</param>
    /// <returns>State or null</returns>
    public async Task<State?> GetStateAsync(string stateCode)
    {
        return await _context.States
            .SingleOrDefaultAsync(s => s.StateCode == stateCode);
    }

    /// <summary>
    /// Returns a full list of states ordered by state code and state name
    /// </summary>
    /// <returns>List of states</returns>
    public async Task<List<State>> GetStateListAsync()
    {
        return await _context.States
            .OrderBy(s => s.StateCode.ToUpper())
            .ThenBy(s => s.StateName.ToUpper())
            .ToListAsync();
    }

    /// <summary>
    /// Returns states as lookup list items
    /// </summary>
    /// <returns>List of lookup items</returns>
    public async Task<List<LookupListItem>> GetListAsync()
    {
        return await _context.States
            .OrderBy(s => s.StateCode)
            .Select(s => new LookupListItem
            {
                Value = s.StateCode,
                Description = s.StateName
            })
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new state entity with default values
    /// </summary>
    /// <returns>New State entity</returns>
    public State CreateNew()
    {
        return new State
        {
            StateCode = string.Empty,
            StateName = string.Empty
        };
    }

    /// <summary>
    /// Validates state entity before saving
    /// </summary>
    /// <param name="state">State to validate</param>
    /// <returns>List of validation errors</returns>
    public List<string> Validate(State state)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(state.StateCode))
        {
            errors.Add("State code cannot be blank.");
        }

        if (string.IsNullOrEmpty(state.StateName))
        {
            errors.Add("State name cannot be blank.");
        }

        return errors;
    }

    /// <summary>
    /// Saves a state entity with validation
    /// </summary>
    /// <param name="state">State to save</param>
    /// <returns>True if successful</returns>
    public async Task<bool> SaveStateAsync(State state)
    {
        var errors = Validate(state);
        if (errors.Any())
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        if (state.StateCode == null)
        {
            await AddAsync(state);
        }
        else
        {
            Update(state);
        }

        return await SaveChangesAsync() > 0;
    }
}
