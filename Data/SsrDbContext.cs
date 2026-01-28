using Microsoft.EntityFrameworkCore;
using SSRBusiness.Entities;

namespace SSRBusiness.Data;

public class SsrDbContext : DbContext
{
    public SsrDbContext(DbContextOptions<SsrDbContext> options) : base(options)
    {
    }

    // Acquisition-related tables
    public DbSet<Acquisition> Acquisitions { get; set; }
    public DbSet<AcquisitionBuyer> AcquisitionBuyers { get; set; }
    public DbSet<AcquisitionChange> AcquisitionChanges { get; set; }
    public DbSet<AcquisitionCounty> AcquisitionCounties { get; set; }
    public DbSet<AcquisitionDocument> AcquisitionDocuments { get; set; }
    public DbSet<AcquisitionLien> AcquisitionLiens { get; set; }
    public DbSet<AcquisitionLienCounty> AcquisitionLienCounties { get; set; }
    public DbSet<AcquisitionLienUnit> AcquisitionLienUnits { get; set; }
    public DbSet<AcquisitionNote> AcquisitionNotes { get; set; }
    public DbSet<AcquisitionOperator> AcquisitionOperators { get; set; }
    public DbSet<AcquisitionReferrer> AcquisitionReferrers { get; set; }
    public DbSet<AcquisitionSeller> AcquisitionSellers { get; set; }
    public DbSet<AcquisitionStatus> AcquisitionStatuses { get; set; }
    public DbSet<AcquisitionUnit> AcquisitionUnits { get; set; }
    public DbSet<AcqCurativeRequirement> AcqCurativeRequirements { get; set; }
    public DbSet<AcqUnitCounty> AcqUnitCounties { get; set; }
    public DbSet<AcqUnitCountyOperator> AcqUnitCountyOperators { get; set; }
    public DbSet<AcqUnitWell> AcqUnitWells { get; set; }

    // Letter Agreement tables
    public DbSet<LetterAgreement> LetterAgreements { get; set; }
    public DbSet<LetterAgreementChange> LetterAgreementChanges { get; set; }
    public DbSet<LetterAgreementCounty> LetterAgreementCounties { get; set; }
    public DbSet<LetterAgreementDealStatus> LetterAgreementDealStatuses { get; set; }
    public DbSet<LetterAgreementFilter> LetterAgreementFilters { get; set; }
    public DbSet<LetterAgreementNote> LetterAgreementNotes { get; set; }
    public DbSet<LetterAgreementOperator> LetterAgreementOperators { get; set; }
    public DbSet<LetterAgreementReferrer> LetterAgreementReferrers { get; set; }
    public DbSet<LetterAgreementSeller> LetterAgreementSellers { get; set; }
    public DbSet<LetterAgreementStatus> LetterAgreementStatuses { get; set; }
    public DbSet<LetterAgreementUnit> LetterAgreementUnits { get; set; }
    public DbSet<LetAgUnitCounty> LetAgUnitCounties { get; set; }
    public DbSet<LetAgUnitCountyOperator> LetAgUnitCountyOperators { get; set; }
    public DbSet<LetAgUnitWell> LetAgUnitWells { get; set; }

    // Lookup tables
    public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
    public DbSet<AppraisalGroup> AppraisalGroups { get; set; }
    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<BuyerContact> BuyerContacts { get; set; }
    public DbSet<CadTable> CadTables { get; set; }
    public DbSet<ComparisonType> ComparisonTypes { get; set; }
    public DbSet<County> Counties { get; set; }
    public DbSet<CountyAppraisalGroup> CountyAppraisalGroups { get; set; }
    public DbSet<CountyContact> CountyContacts { get; set; }
    public DbSet<CurativeType> CurativeTypes { get; set; }
    public DbSet<DealStatus> DealStatuses { get; set; }
    public DbSet<ChangeType> ChangeTypes { get; set; }
    public DbSet<DisplayField> DisplayFields { get; set; }
    public DbSet<DocumentTemplate> DocumentTemplates { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<DocTemplateCustomField> DocTemplateCustomFields { get; set; }
    public DbSet<Filter> Filters { get; set; }
    public DbSet<FilterField> FilterFields { get; set; }
    public DbSet<FolderLocation> FolderLocations { get; set; }
    public DbSet<LienType> LienTypes { get; set; }
    public DbSet<LoginStatus> LoginStatuses { get; set; }
    public DbSet<Lookup> Lookups { get; set; }
    public DbSet<LookupField> LookupFields { get; set; }
    public DbSet<LookupFieldDataType> LookupFieldDataTypes { get; set; }
    public DbSet<NoteType> NoteTypes { get; set; }
    public DbSet<Operator> Operators { get; set; }
    public DbSet<OperatorContact> OperatorContacts { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Referrer> Referrers { get; set; }
    public DbSet<ReferrerForm> ReferrerForms { get; set; }
    public DbSet<ReferrerFormType> ReferrerFormTypes { get; set; }
    public DbSet<ReferrerType> ReferrerTypes { get; set; }
    // public DbSet<Report> Reports { get; set; } // Table doesn't exist in database
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Sequence> Sequences { get; set; }
    public DbSet<SigningPartner> SigningPartners { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<UnitType> UnitTypes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAcquisitionSetup> UserAcquisitionSetups { get; set; }
    public DbSet<UserHistory> UserHistories { get; set; }
    public DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<View> Views { get; set; }
    public DbSet<ViewField> ViewFields { get; set; }
    public DbSet<UserPagePreference> UserPagePreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore the LandMan navigation property due to type mismatch
        // LandManID is int? but User.UserId is string
        // This relationship needs to be fixed in the data model
        modelBuilder.Entity<Acquisition>()
            .Ignore(a => a.LandMan);

        // Configure DocumentTemplate.DocumentType relationship to use DocumentTypeCode
        modelBuilder.Entity<DocumentTemplate>()
            .HasOne(d => d.DocumentType)
            .WithMany()
            .HasForeignKey(d => d.DocumentTypeCode)
            .IsRequired();

        // Configure Buyer.DefaultCommission precision
        modelBuilder.Entity<Buyer>()
            .Property(b => b.DefaultCommission)
            .HasPrecision(18, 2);

        // Apply configurations from the same assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SsrDbContext).Assembly);
    }
}
