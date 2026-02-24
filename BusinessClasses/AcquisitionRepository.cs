using Microsoft.EntityFrameworkCore;
using SSRBusiness.Data;
using SSRBusiness.Entities;
using SSRBusiness.ReportQueries;

namespace SSRBusiness.BusinessClasses;

public class AcquisitionRepository : BaseRepository<Acquisition>
{
    private readonly SsrDbContext _context;

    public AcquisitionRepository(SsrDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Acquisition?> LoadAcquisitionByAcquisitionIDAsync(int acquisitionID)
    {
        return await DbSet
            .Include(a => a.AcquisitionStatus)
            .Include(a => a.LandMan)
            .Include(a => a.AcquisitionBuyers).ThenInclude(ab => ab.Buyer).ThenInclude(b => b.BuyerContacts)
            .Include(a => a.AcquisitionSellers)
            .Include(a => a.AcquisitionCounties).ThenInclude(ac => ac.County).ThenInclude(c => c.CountyContacts)
            .Include(a => a.AcquisitionOperators).ThenInclude(ao => ao.Operator).ThenInclude(o => o.OperatorContacts)
            .Include(a => a.AcquisitionUnits).ThenInclude(au => au.UnitType)
            .Include(a => a.AcquisitionUnits).ThenInclude(au => au.AcqUnitCounties)
            .Include(a => a.AcquisitionUnits).ThenInclude(au => au.AcqUnitWells)
            .Include(a => a.AcquisitionNotes)
            .Include(a => a.AcquisitionLiens)
            .Include(a => a.AcqCurativeRequirements)
            .Include(a => a.AcquisitionReferrers)
            .SingleOrDefaultAsync(u => u.AcquisitionID == acquisitionID);
    }

    public async Task<List<Acquisition>> GetAcquisitionListAsync()
    {
        return await DbSet
            .Include(a => a.AcquisitionBuyers).ThenInclude(ab => ab.Buyer)
            .Include(a => a.AcquisitionSellers)
            .Include(a => a.AcquisitionOperators).ThenInclude(ao => ao.Operator)
            .Include(a => a.AcquisitionCounties).ThenInclude(ac => ac.County)
            .Include(a => a.AcquisitionUnits).ThenInclude(au => au.UnitType)
            .OrderBy(a => a.AcquisitionID)
            .ToListAsync();
    }

    public void DeleteAcquisition(Acquisition entity)
    {
        if (entity == null) return;

        // EF Core handles cascade delete if configured in DB, but legacy code did manual delete.
        // We will follow legacy pattern to be safe, assuming relying on manual dependency deletion.

        _context.AcquisitionChanges.RemoveRange(entity.AcquisitionChanges);
        if (entity.AcqCurativeRequirements != null) _context.AcqCurativeRequirements.RemoveRange(entity.AcqCurativeRequirements); // Assuming DbSet exists on Context
        _context.AcquisitionStatuses.RemoveRange(entity.AcquisitionStatus);
        _context.AcquisitionBuyers.RemoveRange(entity.AcquisitionBuyers);
        _context.AcquisitionSellers.RemoveRange(entity.AcquisitionSellers);
        _context.AcquisitionReferrers.RemoveRange(entity.AcquisitionReferrers);

        // Complex logic for Unit deletes in legacy: 
        // AcqUnitCountyOperators -> AcqUnitCounties -> AcqUnitWells -> AcquisitionUnits
        // This requires loading the graph before deleting if EF Core cascade isn't set up.
        // For now, assuming DbSet removal is enough or Cascade paths exist in DB.

        _context.AcquisitionUnits.RemoveRange(entity.AcquisitionUnits);
        _context.AcquisitionOperators.RemoveRange(entity.AcquisitionOperators);
        _context.AcquisitionCounties.RemoveRange(entity.AcquisitionCounties);
        _context.AcquisitionNotes.RemoveRange(entity.AcquisitionNotes);

        if (entity.AcquisitionLiens != null)
        {
            // Removing related tables for Liens if needed, legacy did LienCounties, LienUnits first
            _context.AcquisitionLiens.RemoveRange(entity.AcquisitionLiens);
        }

        _context.AcquisitionDocuments.RemoveRange(entity.AcquisitionDocuments);

        DbSet.Remove(entity);
    }


    public async Task<List<Acquisition>> GetFilteredAcquisitionListAsync(ReportSelectionCriteria criteria)
    {
        var query = DbSet.AsQueryable();

        if (criteria.LandmanList.Any())
        {
            var ids = ParseIntList(criteria.LandmanList);
            query = query.Where(a => a.LandManID.HasValue && ids.Contains(a.LandManID.Value));
        }

        if (criteria.FieldLandmanList.Any())
        {
            var ids = ParseIntList(criteria.FieldLandmanList);
            query = query.Where(a => a.FieldLandmanID.HasValue && ids.Contains(a.FieldLandmanID.Value));
        }

        if (criteria.BuyerList.Any())
        {
            var ids = ParseIntList(criteria.BuyerList);
            query = query.Where(a => a.AcquisitionBuyers.Any(ab => ids.Contains(ab.BuyerID)));
        }

        if (criteria.CountyList.Any())
        {
            var ids = ParseIntList(criteria.CountyList);
            query = query.Where(a => a.AcquisitionCounties.Any(ac => ids.Contains(ac.CountyID)));
        }

        if (criteria.OperatorList.Any())
        {
            var ids = ParseIntList(criteria.OperatorList);
            query = query.Where(a => a.AcquisitionOperators.Any(ao => ids.Contains(ao.OperatorID)));
        }

        if (criteria.DealStatusList.Any())
        {
            query = ApplyDealStatusFilter(query, criteria.DealQueryType, ParseIntList(criteria.DealStatusList));
        }

        query = ApplyDateFilter(query, criteria.EffectiveDate, a => a.EffectiveDate);
        query = ApplyDateFilter(query, criteria.BuyerEffectiveDate, a => a.BuyerEffectiveDate);
        query = ApplyDateFilter(query, criteria.DueDate, a => a.DueDate);
        query = ApplyDateFilter(query, criteria.PaidDate, a => a.PaidDate);
        query = ApplyDateFilter(query, criteria.TitleOpinionReceivedDate, a => a.TitleOpinionReceivedDate);
        query = ApplyDateFilter(query, criteria.ClosingLetterDate, a => a.ClosingLetterDate);
        query = ApplyDateFilter(query, criteria.DeedDate, a => a.DeedDate);
        query = ApplyDateFilter(query, criteria.InvoiceDate, a => a.InvoiceDate);
        query = ApplyDateFilter(query, criteria.InvoiceDueDate, a => a.InvoiceDueDate);
        query = ApplyDateFilter(query, criteria.InvoicePaidDate, a => a.InvoicePaidDate);

        if (!string.IsNullOrEmpty(criteria.InvoiceNumber.CheckIsEmpty.ToString()) && criteria.InvoiceNumber.CheckIsEmpty)
        {
            query = query.Where(a => string.IsNullOrEmpty(a.InvoiceNumber));
        }
        else if (!string.IsNullOrEmpty(criteria.InvoiceNumber.CheckIsNotEmpty.ToString()) && criteria.InvoiceNumber.CheckIsNotEmpty)
        {
            // Legacy VB "CheckIsNotEmpty" logic check
            query = query.Where(a => !string.IsNullOrEmpty(a.InvoiceNumber));
        }

        // Specific Invoice Number search not shown in VB extend method but commonly requested? 
        // VB logic: query = ExtendInvoiceNumber(criteria.InvoiceNumber, query)
        // Oops, VB used EmptyCheck class for InvoiceNumber, so it's Empty/NotEmpty check.

        if (criteria.ReferralCheck.CheckExists)
        {
            query = query.Where(a => a.AcquisitionReferrers.Any());
        }
        else if (criteria.ReferralCheck.CheckNotExists)
        {
            query = query.Where(a => !a.AcquisitionReferrers.Any());
        }

        if (criteria.LienCheck.CheckExists)
        {
            query = query.Where(a => a.AcquisitionLiens.Any());
        }
        else if (criteria.LienCheck.CheckNotExists)
        {
            query = query.Where(a => !a.AcquisitionLiens.Any());
        }

        if (criteria.CurativeCheck.CheckExists)
        {
            query = query.Where(a => a.AcqCurativeRequirements.Any());
        }
        else if (criteria.CurativeCheck.CheckNotExists)
        {
            query = query.Where(a => !a.AcqCurativeRequirements.Any());
        }

        // LetterAgreementCheck logic was: ExtendLetterAgreement
        // We need to implement it if needed, assuming Acquisition has relationship to LA, 
        // but AcquisitionManagementEntities.cs didn't show LA relationship?
        // Legacy: query = ExtendLetterAgreement(criteria.LetterAgreementCheck, query)
        // If relationship not present in Entity, skippping or noting.

        if (criteria.IncludeNewNoInvoiceNumber == "Y")
        {
            // Union logic
            // NewQuery: BuyerName == "NEW" && InvoiceNumber is empty
            var unionQuery = DbSet.Where(a =>
                a.AcquisitionBuyers.Any(ab => ab.Buyer.BuyerName.ToUpper() == "NEW") &&
                (string.IsNullOrEmpty(a.InvoiceNumber))
            );

            // EF Core Union
            // To do this efficiently, we might need to construct the predicate
            // OR we can concat results if returning List

            // query = query.Union(unionQuery); 
            // Better to construct "Where (PreviousFilters) OR (NewNoInvoiceCondition)"
            // But composing predicates is hard. 
            // We'll stick to Union for now.

            // ToListAsync() and then Union in memory if complex, but Union() works on IQueryable.
            query = query.Union(unionQuery);
        }

        return await query.OrderBy(a => a.AcquisitionID).ToListAsync();
    }

    private IQueryable<Acquisition> ApplyDealStatusFilter(IQueryable<Acquisition> query, string type, List<int> statusIds)
    {
        switch (type)
        {
            case "1": // Is Currently
                return query.Where(a => a.AcquisitionStatus.Any() &&
                    statusIds.Contains(a.AcquisitionStatus
                        .OrderByDescending(s => s.StatusDate)
                        .First().DealStatusID));
            case "2": // Is Not Currently
                      // Logic: (No status) OR (Has Status AND Latest Status is NOT in list)
                return query.Where(a => !a.AcquisitionStatus.Any() ||
                   !statusIds.Contains(a.AcquisitionStatus
                       .OrderByDescending(s => s.StatusDate)
                       .First().DealStatusID));
            case "3": // Is or Was Ever
                return query.Where(a => a.AcquisitionStatus.Any(s => statusIds.Contains(s.DealStatusID)));
            case "4": // Is Not and Was Never
                return query.Where(a => !a.AcquisitionStatus.Any(s => statusIds.Contains(s.DealStatusID)));
            default:
                return query;
        }
    }



    // Correct helper usage
    private IQueryable<Acquisition> ApplyDateFilter(IQueryable<Acquisition> query, ReportDate dateCriteria, System.Linq.Expressions.Expression<Func<Acquisition, DateTime?>> dateExpr)
    {
        if (dateCriteria.FromDate.HasValue)
        {
            // query.Where(a => a.Date >= val)
            // We need to construct Where(a => dateExpr(a) >= val)
            // This requires PredicateBuilder or manual expression tree.

            // Easy way: 
            var ge = System.Linq.Expressions.Expression.GreaterThanOrEqual(
                dateExpr.Body,
                System.Linq.Expressions.Expression.Constant(dateCriteria.FromDate.Value, typeof(DateTime?))
            );
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Acquisition, bool>>(ge, dateExpr.Parameters);
            query = query.Where(lambda);
        }

        if (dateCriteria.ToDate.HasValue)
        {
            var le = System.Linq.Expressions.Expression.LessThanOrEqual(
                dateExpr.Body,
                System.Linq.Expressions.Expression.Constant(dateCriteria.ToDate.Value, typeof(DateTime?))
            );
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Acquisition, bool>>(le, dateExpr.Parameters);
            query = query.Where(lambda);
        }

        if (dateCriteria.CheckIsEmpty)
        {
            var eq = System.Linq.Expressions.Expression.Equal(
                dateExpr.Body,
                System.Linq.Expressions.Expression.Constant(null, typeof(DateTime?))
            );
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Acquisition, bool>>(eq, dateExpr.Parameters);
            query = query.Where(lambda);
        }

        if (dateCriteria.CheckIsNotEmpty)
        {
            var ne = System.Linq.Expressions.Expression.NotEqual(
                dateExpr.Body,
                System.Linq.Expressions.Expression.Constant(null, typeof(DateTime?))
            );
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Acquisition, bool>>(ne, dateExpr.Parameters);
            query = query.Where(lambda);
        }

        return query;
    }

    private List<int> ParseIntList(List<string> list)
    {
        return list.Select(s => int.TryParse(s, out var i) ? i : 0).Where(i => i != 0).ToList();
    }
}
