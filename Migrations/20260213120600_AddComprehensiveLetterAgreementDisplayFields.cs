using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSRBusiness.Migrations
{
    /// <inheritdoc />
    public partial class AddComprehensiveLetterAgreementDisplayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Update existing LetterAgreement DisplayFields and add missing ones
                -- First, update DataType for existing fields
                UPDATE DisplayFields SET DataType = 'integer' WHERE FieldID = 4001 AND Module = 'LetterAgreement'; -- ID
                UPDATE DisplayFields SET DataType = 'integer' WHERE FieldID = 4002 AND Module = 'LetterAgreement'; -- Acquisition ID
                UPDATE DisplayFields SET DataType = 'date' WHERE FieldID = 4003 AND Module = 'LetterAgreement'; -- Effective Date
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 4004 AND Module = 'LetterAgreement'; -- Total Bonus
                UPDATE DisplayFields SET DataType = 'decimal' WHERE FieldID = 4005 AND Module = 'LetterAgreement'; -- Net Acres

                -- Add missing fields
                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields ON;

                -- Insert missing direct database fields
                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module, DataType) VALUES
                (4006, 'Created On', 'CreatedOn', 60, 'LetterAgreement', 'datetime'),
                (4007, 'Receipt Date', 'ReceiptDate', 70, 'LetterAgreement', 'date'),
                (4008, 'Banking Days', 'BankingDays', 80, 'LetterAgreement', 'integer'),
                (4009, 'Consideration Fee', 'ConsiderationFee', 90, 'LetterAgreement', 'money'),
                (4010, 'Take Consideration From Total', 'TakeConsiderationFromTotal', 100, 'LetterAgreement', 'boolean'),
                (4011, 'Referrals', 'Referrals', 110, 'LetterAgreement', 'boolean'),
                (4012, 'Referral Fee', 'ReferralFee', 120, 'LetterAgreement', 'money'),
                (4013, 'Take Referral Fee From Total', 'TakeReferralFeeFromTotal', 130, 'LetterAgreement', 'boolean'),
                (4014, 'Total Bonus And Fee', 'TotalBonusAndFee', 140, 'LetterAgreement', 'money'),
                (4015, 'Gross Acres', 'TotalGrossAcres', 150, 'LetterAgreement', 'decimal'),
                (4016, 'Land Man ID', 'LandManID', 160, 'LetterAgreement', 'integer');

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields OFF;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Remove added fields, restore to original 5 fields
                DELETE FROM DisplayFields WHERE FieldID IN (4006, 4007, 4008, 4009, 4010, 4011, 4012, 4013, 4014, 4015, 4016) AND Module = 'LetterAgreement';
                
                -- Reset DataType to original values (text for ID, others as they were)
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 4001 AND Module = 'LetterAgreement';
            ");
        }
    }
}
