using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSRBusiness.Migrations
{
    /// <inheritdoc />
    public partial class AddDataTypeToDisplayFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add DataType column to DisplayFields table
            migrationBuilder.AddColumn<string>(
                name: "DataType",
                table: "DisplayFields",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            // Update existing Acquisition DisplayFields with appropriate DataTypes
            migrationBuilder.Sql(@"
                -- Update DataType for existing Acquisition DisplayFields
                UPDATE DisplayFields SET DataType = 'integer' WHERE FieldID = 100 AND Module = 'Acquisition'; -- Acquisition ID
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 3400 AND Module = 'Acquisition'; -- Acquisition Number
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 3300 AND Module = 'Acquisition'; -- Buyer Name
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 3500 AND Module = 'Acquisition'; -- Assignee
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 1005 AND Module = 'Acquisition'; -- Folder Location
                
                -- Date Fields
                UPDATE DisplayFields SET DataType = 'date' WHERE FieldID = 1300 AND Module = 'Acquisition'; -- Effective Date
                UPDATE DisplayFields SET DataType = 'date' WHERE FieldID = 1400 AND Module = 'Acquisition'; -- Buyer Effective Date
                UPDATE DisplayFields SET DataType = 'date' WHERE FieldID = 1800 AND Module = 'Acquisition'; -- Due Date
                UPDATE DisplayFields SET DataType = 'date' WHERE FieldID = 1900 AND Module = 'Acquisition'; -- Paid Date
                
                -- Financial Fields (money)
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 2000 AND Module = 'Acquisition'; -- Total Bonus
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 1011 AND Module = 'Acquisition'; -- Consideration Fee
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 2200 AND Module = 'Acquisition'; -- Total Bonus and Fee
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 2100 AND Module = 'Acquisition'; -- Draft Fee
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 2400 AND Module = 'Acquisition'; -- Draft/Check Number
                
                -- Acreage Fields (decimal)
                UPDATE DisplayFields SET DataType = 'decimal' WHERE FieldID = 1500 AND Module = 'Acquisition'; -- Gross Acres
                UPDATE DisplayFields SET DataType = 'decimal' WHERE FieldID = 1600 AND Module = 'Acquisition'; -- Net Acres
                
                -- Status Fields
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 2600 AND Module = 'Acquisition'; -- Field Check
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 3200 AND Module = 'Acquisition'; -- Closing Status
                UPDATE DisplayFields SET DataType = 'boolean' WHERE FieldID = 5800 AND Module = 'Acquisition'; -- SSR In Pay
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 3100 AND Module = 'Acquisition'; -- Title Opinion
                
                -- Lien Fields
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 2800 AND Module = 'Acquisition'; -- Liens
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 3000 AND Module = 'Acquisition'; -- Lien Amount
                
                -- Tax Fields
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 1023 AND Module = 'Acquisition'; -- Tax Due
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 1024 AND Module = 'Acquisition'; -- Tax Paid
                
                -- Invoice Fields
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 3800 AND Module = 'Acquisition'; -- Invoice Number
                UPDATE DisplayFields SET DataType = 'date' WHERE FieldID = 3900 AND Module = 'Acquisition'; -- Invoice Date
                UPDATE DisplayFields SET DataType = 'date' WHERE FieldID = 4000 AND Module = 'Acquisition'; -- Invoice Due Date
                UPDATE DisplayFields SET DataType = 'date' WHERE FieldID = 4100 AND Module = 'Acquisition'; -- Invoice Paid Date
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 4200 AND Module = 'Acquisition'; -- Invoice Total
                
                -- Commission Fields
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 4400 AND Module = 'Acquisition'; -- Commission
                UPDATE DisplayFields SET DataType = 'decimal' WHERE FieldID = 1031 AND Module = 'Acquisition'; -- Comm %
                
                -- Check Fields
                UPDATE DisplayFields SET DataType = 'boolean' WHERE FieldID = 2500 AND Module = 'Acquisition'; -- Check Stub
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 1033 AND Module = 'Acquisition'; -- Stub Desc
                UPDATE DisplayFields SET DataType = 'money' WHERE FieldID = 1034 AND Module = 'Acquisition'; -- Referral Fee

                -- Relational Fields (text)
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 200 AND Module = 'Acquisition'; -- Seller Last Name
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 300 AND Module = 'Acquisition'; -- Seller Name
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 700 AND Module = 'Acquisition'; -- Seller Address
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 1000 AND Module = 'Acquisition'; -- Seller City
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 1100 AND Module = 'Acquisition'; -- Seller State
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 1200 AND Module = 'Acquisition'; -- Seller Zip Code
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 400 AND Module = 'Acquisition'; -- Seller Email
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 500 AND Module = 'Acquisition'; -- Seller Phone
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 600 AND Module = 'Acquisition'; -- Seller Fax
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 5100 AND Module = 'Acquisition'; -- Operator Name
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 4600 AND Module = 'Acquisition'; -- County Name
                UPDATE DisplayFields SET DataType = 'text' WHERE FieldID = 5500 AND Module = 'Acquisition'; -- Unit Name
                UPDATE DisplayFields SET DataType = 'decimal' WHERE FieldID = 5600 AND Module = 'Acquisition'; -- Unit Interest
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataType",
                table: "DisplayFields");
        }
    }
}
