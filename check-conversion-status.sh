#!/bin/bash

# SSRBusiness VB to C# Entity Repository Conversion Script
# This script will help track and convert all 76 VB repository classes to C#

echo "🔄 SSRBusiness Repository Conversion Tracker"
echo "============================================="
echo ""

# Count total files
TOTAL_VB_FILES=$(ls /Users/gqadonis/RiderProjects/SSRBusiness/Entities/*.vb 2>/dev/null | wc -l | tr -d ' ')
TOTAL_CS_FILES=$(ls /Users/gqadonis/RiderProjects/SSRBusiness.NET10/BusinessClasses/*Repository.cs 2>/dev/null | wc -l | tr -d ' ')

echo "📊 Conversion Status:"
echo "   VB Files to convert: $TOTAL_VB_FILES"
echo "   C# Files created: $TOTAL_CS_FILES"
echo "   Remaining: $((TOTAL_VB_FILES - TOTAL_CS_FILES))"
echo ""

# List all VB entity files
echo "📁 VB Repository Classes to Convert:"
echo "======================================"
ls -1 /Users/gqadonis/RiderProjects/SSRBusiness/Entities/*.vb | while read file; do
    basename="$(basename "$file" .vb)"
    csname="${basename/Entity/Repository}"
    
    if [ -f "/Users/gqadonis/RiderProjects/SSRBusiness.NET10/BusinessClasses/${csname}.cs" ]; then
        echo "✅ $basename -> $csname.cs"
    else
        echo "⏳ $basename -> $csname.cs (TODO)"
    fi
done

echo ""
echo "💡 Conversion Pattern:"
echo "   VB: AcquisitionEntity.vb     ->  C#: AcquisitionRepository.cs"
echo "   VB: CountyEntity.vb          ->  C#: CountyRepository.cs"
echo "   VB: UserEntity.vb            ->  C#: UserRepository.cs"
echo ""
