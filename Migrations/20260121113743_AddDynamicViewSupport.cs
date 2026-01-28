using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSRBusiness.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicViewSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add Module column to DisplayFields if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[DisplayFields]') AND name = 'Module')
                BEGIN
                    ALTER TABLE [DisplayFields] ADD [Module] nvarchar(50) NOT NULL DEFAULT N'Acquisition';
                END
            ");

            // 2. Add Module column to Views if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Views]') AND name = 'Module')
                BEGIN
                    ALTER TABLE [Views] ADD [Module] nvarchar(50) NOT NULL DEFAULT N'Acquisition';
                END
            ");

            // 3. Create UserPagePreferences table if not exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[UserPagePreferences]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [UserPagePreferences] (
                        [PreferenceID] int NOT NULL IDENTITY,
                        [UserID] int NOT NULL,
                        [PageName] nvarchar(50) NOT NULL,
                        [ViewID] int NOT NULL,
                        CONSTRAINT [PK_UserPagePreferences] PRIMARY KEY ([PreferenceID]),
                        CONSTRAINT [FK_UserPagePreferences_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE,
                        CONSTRAINT [FK_UserPagePreferences_Views_ViewID] FOREIGN KEY ([ViewID]) REFERENCES [Views] ([ViewID]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_UserPagePreferences_UserID] ON [UserPagePreferences] ([UserID]);
                    CREATE INDEX [IX_UserPagePreferences_ViewID] ON [UserPagePreferences] ([ViewID]);
                END
            ");

            // 4. Seed DisplayFields for new modules
            // Since FieldID is NOT Identity (based on error), we must generate IDs explicitly.
            migrationBuilder.Sql(@"
                DECLARE @MaxId INT;
                SELECT @MaxId = ISNULL(MAX(FieldID), 0) FROM DisplayFields;

                INSERT INTO DisplayFields (FieldID, FieldName, ColumnName, DisplayOrder, Module)
                VALUES
                -- Operator Module
                (@MaxId + 1, 'OperatorID', 'OperatorID', 1, 'Operator'),
                (@MaxId + 2, 'OperatorName', 'OperatorName', 2, 'Operator'),
                (@MaxId + 3, 'Address', 'Address', 3, 'Operator'),
                (@MaxId + 4, 'City', 'City', 4, 'Operator'),
                (@MaxId + 5, 'State', 'State', 5, 'Operator'),
                (@MaxId + 6, 'Zip', 'Zip', 6, 'Operator'),
                (@MaxId + 7, 'Phone', 'Phone', 7, 'Operator'),
                (@MaxId + 8, 'Fax', 'Fax', 8, 'Operator'),
                (@MaxId + 9, 'Email', 'Email', 9, 'Operator'),
                (@MaxId + 10, 'ContactName', 'ContactName', 10, 'Operator'),
                (@MaxId + 11, 'Active', 'Active', 11, 'Operator'),

                -- Document Module
                (@MaxId + 12, 'DocumentID', 'DocumentID', 1, 'Document'),
                (@MaxId + 13, 'DocumentType', 'DocumentType', 2, 'Document'),
                (@MaxId + 14, 'DocumentDate', 'DocumentDate', 3, 'Document'),
                (@MaxId + 15, 'Description', 'Description', 4, 'Document'),
                (@MaxId + 16, 'FilePath', 'FilePath', 5, 'Document'),
                (@MaxId + 17, 'UploadedBy', 'UploadedBy', 6, 'Document'),
                (@MaxId + 18, 'UploadedDate', 'UploadedDate', 7, 'Document'),

                -- LetterAgreement Module
                (@MaxId + 19, 'LetterAgreementID', 'LetterAgreementID', 1, 'LetterAgreement'),
                (@MaxId + 20, 'LanNumber', 'LanNumber', 2, 'LetterAgreement'),
                (@MaxId + 21, 'Status', 'Status', 3, 'LetterAgreement'),
                (@MaxId + 22, 'EffectiveDate', 'EffectiveDate', 4, 'LetterAgreement'),
                (@MaxId + 23, 'ExpirationDate', 'ExpirationDate', 5, 'LetterAgreement'),
                (@MaxId + 24, 'Description', 'Description', 6, 'LetterAgreement'),
                (@MaxId + 25, 'Notes', 'Notes', 7, 'LetterAgreement'),
                
                -- Referrer Module
                (@MaxId + 26, 'ReferrerID', 'ReferrerID', 1, 'Referrer'),
                (@MaxId + 27, 'FirstName', 'FirstName', 2, 'Referrer'),
                (@MaxId + 28, 'LastName', 'LastName', 3, 'Referrer'),
                (@MaxId + 29, 'Company', 'Company', 4, 'Referrer'),
                (@MaxId + 30, 'Phone', 'Phone', 5, 'Referrer'),
                (@MaxId + 31, 'Email', 'Email', 6, 'Referrer'),
                (@MaxId + 32, 'Address', 'Address', 7, 'Referrer'),
                (@MaxId + 33, 'City', 'City', 8, 'Referrer'),
                (@MaxId + 34, 'State', 'State', 9, 'Referrer'),
                (@MaxId + 35, 'Zip', 'Zip', 10, 'Referrer'),
                (@MaxId + 36, 'ReferrerType', 'ReferrerType', 11, 'Referrer'),

                -- Filter Module
                (@MaxId + 37, 'FilterID', 'FilterID', 1, 'Filter'),
                (@MaxId + 38, 'FilterName', 'FilterName', 2, 'Filter'),
                (@MaxId + 39, 'Description', 'Description', 3, 'Filter'),
                (@MaxId + 40, 'CreatedBy', 'CreatedBy', 4, 'Filter'),
                (@MaxId + 41, 'CreatedDate', 'CreatedDate', 5, 'Filter'),
                (@MaxId + 42, 'IsPublic', 'IsPublic', 6, 'Filter');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse of Up()

            // 1. Delete seeded display fields
            migrationBuilder.Sql("DELETE FROM DisplayFields WHERE Module IN ('Operator', 'Document', 'LetterAgreement', 'Referrer', 'Filter')");

            // 2. Drop UserPagePreferences table
            migrationBuilder.Sql("IF OBJECT_ID(N'[UserPagePreferences]', N'U') IS NOT NULL DROP TABLE [UserPagePreferences]");

            // 3. Drop Module column from Views
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Views]') AND name = 'Module')
                BEGIN
                    DECLARE @ConstraintName nvarchar(200)
                    SELECT @ConstraintName = Name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('Views') AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Views') AND name = 'Module')
                    IF @ConstraintName IS NOT NULL
                    EXEC('ALTER TABLE [Views] DROP CONSTRAINT ' + @ConstraintName)
                    
                    ALTER TABLE [Views] DROP COLUMN [Module]
                END
            ");

            // 4. Drop Module column from DisplayFields
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[DisplayFields]') AND name = 'Module')
                BEGIN
                     DECLARE @ConstraintName nvarchar(200)
                    SELECT @ConstraintName = Name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('DisplayFields') AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('DisplayFields') AND name = 'Module')
                    IF @ConstraintName IS NOT NULL
                    EXEC('ALTER TABLE [DisplayFields] DROP CONSTRAINT ' + @ConstraintName)

                    ALTER TABLE [DisplayFields] DROP COLUMN [Module]
                END
            ");
        }
    }
}
