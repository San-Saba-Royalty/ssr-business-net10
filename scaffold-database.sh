#!/bin/bash

echo "🚀 SSRBusiness Entity Scaffolding for .NET 10"
echo "=============================================="
echo ""

# Check if connection string is still default
if grep -q "YOUR_SERVER" appsettings.json; then
    echo "❌ ERROR: Please update the connection string in appsettings.json first!"
    echo ""
    echo "Edit appsettings.json and replace:"
    echo "  - YOUR_SERVER with your SQL Server instance"
    echo "  - YOUR_USER with your database username"  
    echo "  - YOUR_PASSWORD with your database password"
    echo ""
    exit 1
fi

# Install dotnet-ef if not already installed
echo "📦 Ensuring dotnet-ef tool is installed..."
dotnet tool install --global dotnet-ef 2>/dev/null || dotnet tool update --global dotnet-ef

# Navigate to project directory
cd "$(dirname "$0")"

echo ""
echo "🗄️  Restoring NuGet packages..."
dotnet restore

echo ""
echo "🔨 Building project..."
dotnet build

# Read connection string from appsettings.json
CONNECTION_STRING=$(cat appsettings.json | grep "SanSabaConnection" | cut -d'"' -f4)

echo ""
echo "📊 Scaffolding database entities from SQL Server..."
echo "   This will generate C# classes for all 70+ tables..."
echo ""

# Scaffold ALL entities from database
dotnet ef dbcontext scaffold \
  "$CONNECTION_STRING" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Entities/Generated \
  --context-dir Data \
  --context SsrDbContext \
  --force \
  --no-pluralize \
  --data-annotations \
  --verbose

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ SUCCESS! All entities have been generated!"
    echo ""
    echo "📁 Generated files:"
    echo "   - Data/SsrDbContext.cs (DbContext)"
    echo "   - Entities/Generated/*.cs (All entity models)"
    echo ""
    echo "📝 Next steps:"
    echo "   1. Review the generated entities in Entities/Generated/"
    echo "   2. Start converting VB repository classes (use UserRepository.cs as template)"
    echo "   3. Run: dotnet build"
    echo ""
else
    echo ""
    echo "❌ ERROR: Scaffolding failed!"
    echo ""
    echo "Common issues:"
    echo "  1. Connection string is incorrect"
    echo "  2. SQL Server is not accessible"
    echo "  3. Database 'SanSaba' does not exist"
    echo "  4. Network/firewall issues"
    echo ""
    echo "Try testing your connection string manually with:"
    echo "  sqlcmd -S YOUR_SERVER -U YOUR_USER -P YOUR_PASSWORD -Q \"SELECT @@VERSION\""
    echo ""
    exit 1
fi
