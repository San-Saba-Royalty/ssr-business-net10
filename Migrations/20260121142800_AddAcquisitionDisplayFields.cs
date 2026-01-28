using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSRBusiness.Migrations
{
    /// <inheritdoc />
    public partial class AddAcquisitionDisplayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Delete existing Acquisition DisplayFields if any
                DELETE FROM DisplayFields WHERE Module = 'Acquisition';

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields ON;

                -- Insert all Acquisition DisplayFields using Legacy IDs and Names where possible
                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module) VALUES 
                -- Legacy Core Fields
                (100, 'Acquisition ID', 'AcquisitionID', 1, 'Acquisition'),
                (3400, 'Acquisition Number', 'AcquisitionNumber', 2, 'Acquisition'),
                (3300, 'Buyer Name', 'Buyer', 3, 'Acquisition'),
                (3500, 'Assignee', 'Assignee', 4, 'Acquisition'),
                (1005, 'Folder Location', 'FolderLocation', 5, 'Acquisition'), -- No Legacy ID found, keeping new
                
                -- Legacy Date Fields
                (1300, 'Effective Date', 'EffectiveDate', 6, 'Acquisition'),
                (1400, 'Buyer Effective Date', 'BuyerEffectiveDate', 7, 'Acquisition'),
                (1800, 'Due Date', 'DueDate', 8, 'Acquisition'),
                (1900, 'Paid Date', 'PaidDate', 9, 'Acquisition'),
                
                -- Legacy Financial Fields
                (2000, 'Total Bonus', 'TotalBonus', 10, 'Acquisition'),
                (1011, 'Consideration Fee', 'ConsiderationFee', 11, 'Acquisition'), -- New field
                (2200, 'Total Bonus and Fee', 'TotalBonusAndFee', 12, 'Acquisition'),
                (2100, 'Draft Fee', 'DraftFee', 13, 'Acquisition'),
                (2400, 'Draft/Check Number', 'DraftCheckNumber', 14, 'Acquisition'),
                
                -- Legacy Acreage Fields
                (1500, 'Gross Acres', 'TotalGrossAcres', 15, 'Acquisition'),
                (1600, 'Net Acres', 'TotalNetAcres', 16, 'Acquisition'),
                
                -- Legacy Status Fields
                (2600, 'Field Check', 'FieldCheck', 17, 'Acquisition'),
                (3200, 'Closing Status', 'ClosingStatus', 18, 'Acquisition'),
                (5800, 'SSR In Pay', 'SsrInPay', 19, 'Acquisition'),
                (3100, 'Title Opinion', 'TitleOpinion', 20, 'Acquisition'),
                
                -- Legacy Lien Fields
                (2800, 'Liens', 'Liens', 21, 'Acquisition'),
                (3000, 'Lien Amount', 'LienAmount', 22, 'Acquisition'),
                
                -- Legacy Tax Fields
                (1023, 'Tax Due', 'TaxAmountDue', 23, 'Acquisition'), -- New IDs likely
                (1024, 'Tax Paid', 'TaxAmountPaid', 24, 'Acquisition'),
                
                -- Legacy Invoice Fields
                (3800, 'Invoice Number', 'InvoiceNumber', 25, 'Acquisition'),
                (3900, 'Invoice Date', 'InvoiceDate', 26, 'Acquisition'),
                (4000, 'Invoice Due Date', 'InvoiceDueDate', 27, 'Acquisition'),
                (4100, 'Invoice Paid Date', 'InvoicePaidDate', 28, 'Acquisition'),
                (4200, 'Invoice Total', 'InvoiceTotal', 29, 'Acquisition'),
                
                -- Legacy Commission Fields
                (4400, 'Commission', 'Commission', 30, 'Acquisition'),
                (1031, 'Comm %', 'CommissionPercent', 31, 'Acquisition'),
                
                -- Legacy Check Fields
                (2500, 'Check Stub', 'HaveCheckStub', 32, 'Acquisition'),
                (1033, 'Stub Desc', 'CheckStubDesc', 33, 'Acquisition'),
                (1034, 'Referral Fee', 'ReferralFee', 34, 'Acquisition'),

                -- Legacy Relational Fields (Crucial for Views)
                (200, 'Seller Last Name', 'SellerLastName', 50, 'Acquisition'),
                (300, 'Seller Name', 'SellerName', 51, 'Acquisition'),
                (700, 'Seller Address', 'SellerAddressLine1', 52, 'Acquisition'),
                (1000, 'Seller City', 'SellerCity', 53, 'Acquisition'),
                (1100, 'Seller State', 'SellerState', 54, 'Acquisition'),
                (1200, 'Seller Zip Code', 'SellerZipCode', 55, 'Acquisition'),
                (400, 'Seller Email', 'SellerEmail', 56, 'Acquisition'),
                (500, 'Seller Phone', 'SellerPhone', 57, 'Acquisition'),
                (600, 'Seller Fax', 'SellerFax', 58, 'Acquisition'),
                (5100, 'Operator Name', 'OperatorName', 60, 'Acquisition'),
                (4600, 'County Name', 'CountyName', 61, 'Acquisition'),
                (5500, 'Unit Name', 'UnitName', 62, 'Acquisition'),
                (5600, 'Unit Interest', 'UnitInterest', 63, 'Acquisition');

                IF OBJECTPROPERTY(OBJECT_ID('DisplayFields'), 'TableHasIdentity') = 1
                    SET IDENTITY_INSERT DisplayFields OFF;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Remove Acquisition DisplayFields
                DELETE FROM DisplayFields WHERE Module = 'Acquisition';
            ");
        }
    }
}
