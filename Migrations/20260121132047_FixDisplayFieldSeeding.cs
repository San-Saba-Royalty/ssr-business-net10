using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSRBusiness.Migrations
{
    /// <inheritdoc />
    public partial class FixDisplayFieldSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM DisplayFields WHERE Module IN ('Document', 'LetterAgreement', 'Referrer', 'Filter');

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields ON;

                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module) VALUES 
                (3001, 'ID', 'DocumentTemplateID', 10, 'Document'),
                (3002, 'Type', 'DocumentTypeCode', 20, 'Document'),
                (3003, 'Description', 'DocumentTemplateDesc', 30, 'Document'),
                (3004, 'Location', 'DocumentTemplateLocation', 40, 'Document');

                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module) VALUES
                (4001, 'ID', 'LetterAgreementID', 10, 'LetterAgreement'),
                (4002, 'Acquisition ID', 'AcquisitionID', 20, 'LetterAgreement'),
                (4003, 'Effective Date', 'EffectiveDate', 30, 'LetterAgreement'),
                (4004, 'Total Bonus', 'TotalBonus', 40, 'LetterAgreement'),
                (4005, 'Net Acres', 'TotalNetAcres', 50, 'LetterAgreement');

                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module) VALUES
                (5001, 'ID', 'ReferrerID', 10, 'Referrer'),
                (5002, 'Name', 'ReferrerName', 20, 'Referrer'),
                (5003, 'Type', 'ReferrerTypeCode', 30, 'Referrer'),
                (5004, 'Contact Name', 'ContactName', 40, 'Referrer'),
                (5005, 'Email', 'ContactEmail', 50, 'Referrer'),
                (5006, 'Phone', 'ContactPhone', 60, 'Referrer'),
                (5007, 'Address', 'AddressLine1', 70, 'Referrer'),
                (5008, 'City', 'City', 80, 'Referrer'),
                (5009, 'State', 'StateCode', 90, 'Referrer'),
                (5010, 'Zip', 'ZipCode', 100, 'Referrer');

                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module) VALUES
                (6001, 'ID', 'FilterID', 10, 'Filter'),
                (6002, 'Name', 'FilterName', 20, 'Filter');

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields OFF;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM DisplayFields WHERE Module IN ('Document', 'LetterAgreement', 'Referrer', 'Filter');
            ");
        }
    }
}
