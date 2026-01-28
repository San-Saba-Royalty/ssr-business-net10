using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSRBusiness.Migrations
{
    /// <inheritdoc />
    public partial class ExpandLetterAgreementDisplayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Delete existing LetterAgreement DisplayFields
                DELETE FROM DisplayFields WHERE Module = 'LetterAgreement';

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields ON;

                -- Insert all LetterAgreement DisplayFields (19 columns)
                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module) VALUES 
                -- Always visible columns
                (4001, 'LetterAgreementID', 'ID', 1, 'LetterAgreement'),
                (4002, 'BankingDays', 'Banking Days', 2, 'LetterAgreement'),
                (4003, 'AcquisitionID', 'Acq ID', 3, 'LetterAgreement'),

                -- Seller Information (hidden by default)
                (4010, 'SellerLastName', 'Seller Last Name', 10, 'LetterAgreement'),
                (4011, 'SellerName', 'Seller Name', 11, 'LetterAgreement'),
                (4012, 'SellerEmail', 'Seller Email', 12, 'LetterAgreement'),
                (4013, 'SellerPhone', 'Seller Phone', 13, 'LetterAgreement'),
                (4014, 'SellerCity', 'Seller City', 14, 'LetterAgreement'),
                (4015, 'SellerState', 'Seller State', 15, 'LetterAgreement'),
                (4016, 'SellerZipCode', 'Seller Zip Code', 16, 'LetterAgreement'),

                -- Date Fields (hidden by default)
                (4020, 'CreatedOn', 'Created On', 20, 'LetterAgreement'),
                (4021, 'EffectiveDate', 'Effective Date', 21, 'LetterAgreement'),

                -- Financial Fields (hidden by default)
                (4030, 'TotalBonus', 'Purchase Price', 30, 'LetterAgreement'),
                (4031, 'ConsiderationFee', 'Consideration Fee', 31, 'LetterAgreement'),
                (4032, 'ReferralFee', 'Referral Fee', 32, 'LetterAgreement'),

                -- Acreage Fields (hidden by default)
                (4040, 'TotalGrossAcres', 'Total Gross Acres', 40, 'LetterAgreement'),
                (4041, 'TotalNetAcres', 'Total Net Acres', 41, 'LetterAgreement'),

                -- Other Fields (hidden by default)
                (4050, 'LandMan', 'Land Man', 50, 'LetterAgreement'),
                (4051, 'DealStatus', 'Status', 51, 'LetterAgreement'),
                (4052, 'CountyName', 'County Name', 52, 'LetterAgreement'),
                (4053, 'OperatorName', 'Operator Name', 53, 'LetterAgreement'),
                (4054, 'UnitName', 'Unit Name', 54, 'LetterAgreement');

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields OFF;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Restore to previous 5 fields
                DELETE FROM DisplayFields WHERE Module = 'LetterAgreement';

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields ON;

                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module) VALUES
                (4001, 'ID', 'LetterAgreementID', 10, 'LetterAgreement'),
                (4002, 'Acquisition ID', 'AcquisitionID', 20, 'LetterAgreement'),
                (4003, 'Effective Date', 'EffectiveDate', 30, 'LetterAgreement'),
                (4004, 'Total Bonus', 'TotalBonus', 40, 'LetterAgreement'),
                (4005, 'Net Acres', 'TotalNetAcres', 50, 'LetterAgreement');

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields OFF;
            ");
        }
    }
}
