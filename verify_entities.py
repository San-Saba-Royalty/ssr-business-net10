import os
import re
import xml.etree.ElementTree as ET
import glob
import sys

# Paths
DBML_PATH = "/Users/gqadonis/Projects/sansaba/SanSaba/old/ssr/SSRBusiness/Model/SsrDbModel.dbml"
ENTITIES_DIR = "/Users/gqadonis/RiderProjects/SSRBusiness.NET10/Entities"

# XML Namespaces
NS = "{http://schemas.microsoft.com/linqtosql/dbml/2007}"

def log(msg):
    print(msg)
    sys.stdout.flush()

def parse_dbml(path):
    log(f"Parsing DBML at: {path}")
    if not os.path.exists(path):
        log("ERROR: DBML file does not exist!")
        return {}
    
    try:
        tree = ET.parse(path)
        root = tree.getroot()
        log(f"Root tag: {root.tag}")
        
        schema = {}
        
        tables = root.findall(f"{NS}Table")
        log(f"Found {len(tables)} tables in DBML.")
        
        for table in tables:
            full_name = table.get("Name")
            parts = full_name.split('.')
            table_name = parts[-1] 
            
            type_elem = table.find(f"{NS}Type")
            if type_elem is None:
                continue
            class_name = type_elem.get("Name")
            
            columns = {}
            for col in type_elem.findall(f"{NS}Column"):
                col_name = col.get("Name")
                col_type = col.get("DbType")
                is_pk = col.get("IsPrimaryKey") == "true"
                columns[col_name] = {
                    'type': col_type,
                    'is_pk': is_pk
                }
                
            associations = []
            for assoc in type_elem.findall(f"{NS}Association"):
                is_fk = assoc.get("IsForeignKey") == "true"
                if is_fk:
                    fk_col = assoc.get("ThisKey")
                    other_key = assoc.get("OtherKey")
                    other_table_type = assoc.get("Type")
                    member = assoc.get("Member")
                    associations.append({
                        'fk_col': fk_col,
                        'other_key': other_key,
                        'other_type': other_table_type,
                        'member': member
                    })
            
            schema[table_name] = {
                'class_name': class_name,
                'columns': columns,
                'associations': associations
            }
        return schema
    except Exception as e:
        log(f"Exception parsing DBML: {e}")
        return {}

def parse_csharp_entities(directory):
    log(f"Parsing Entities in: {directory}")
    entities = {}
    
    table_attr_re = re.compile(r'\[Table\("([^"]+)"\)(?:,\s*Schema\s*=\s*"([^"]+)")?\]')
    class_re = re.compile(r'public\s+class\s+(\w+)')
    prop_re = re.compile(r'public\s+([\w\?<>]+)\s+(\w+)\s*\{\s*get;\s*set;\s*\}')
    attr_re = re.compile(r'\[(.*?)\]')
    
    files = glob.glob(os.path.join(directory, "*.cs"))
    log(f"Found {len(files)} cs files.")
    
    for file_path in files:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        lines = content.split('\n')
        current_class = None
        current_attributes = []
        
        for line in lines:
            line = line.strip()
            
            attr_match = attr_re.match(line)
            if attr_match:
                current_attributes.extend(line.strip('[]').split(','))
                continue
                
            class_match = class_re.search(line)
            if class_match:
                class_name = class_match.group(1)
                current_class = class_name
                
                table_name = class_name # Default
                
                for attr in current_attributes:
                    if 'Table(' in attr:
                        m = re.search(r'Table\("([^"]+)"\)', attr)
                        if m:
                            table_name = m.group(1)
                            
                entities[class_name] = {
                    'table_name': table_name,
                    'properties': {},
                    'navigations': {}
                }
                current_attributes = []
                continue
                
            if current_class:
                prop_match = prop_re.search(line)
                if prop_match:
                    p_type = prop_match.group(1)
                    p_name = prop_match.group(2)
                    
                    fk_name = None
                    col_name = None
                    
                    for attr in current_attributes:
                        if 'ForeignKey' in attr:
                            m = re.search(r'ForeignKey\("([^"]+)"\)', attr)
                            if m:
                                fk_name = m.group(1)
                        if 'Column' in attr:
                            m = re.search(r'Column\("([^"]+)"\)', attr)
                            if m:
                                col_name = m.group(1)

                    simple_types = ['int', 'long', 'string', 'bool', 'decimal', 'float', 'double', 'DateTime', 'Guid', 'byte[]', 'char']
                    is_simple = any(t in p_type for t in simple_types)
                    
                    if is_simple and 'List<' not in p_type and 'ICollection' not in p_type:
                        final_col_name = col_name if col_name else p_name
                        entities[current_class]['properties'][final_col_name] = {
                            'prop_name': p_name,
                            'type': p_type
                        }
                    else:
                        entities[current_class]['navigations'][p_name] = {
                            'type': p_type,
                            'fk_linked_col': fk_name
                        }
                    
                    current_attributes = []

    return entities

def compare(db_schema, entities, output_file):
    log("Comparing...")
    with open(output_file, 'w') as f:
        f.write("# Schema Verification Report\n\n")
        
        f.write("## Table Verification\n")
        
        table_to_entity = {v['table_name']: k for k, v in entities.items()}
        
        for db_table, db_info in db_schema.items():
            if db_table not in table_to_entity:
                f.write(f"- [WARNING] Table `{db_table}` found in DBML but not mapped to an Entity with `[Table(\"{db_table}\")]`.\n")
                if db_info['class_name'] in entities:
                     ent = entities[db_info['class_name']]
                     f.write(f"  - Found suspect entity `{db_info['class_name']}` but it maps to table `{ent['table_name']}`.\n")
                else:
                     f.write(f"  - No corresponding entity class `{db_info['class_name']}` found.\n")
            else:
                entity_class = table_to_entity[db_table]
                compare_table(db_table, db_info, entities[entity_class], entity_class, f)

def compare_table(table_name, db_info, entity, class_name, f):
    # Check Columns
    db_cols = set(db_info['columns'].keys())
    ent_cols = set(entity['properties'].keys())
    
    missing_in_entity = db_cols - ent_cols
    extra_in_entity = ent_cols - db_cols
    
    has_issues = False
    issues = []
    
    if missing_in_entity:
        has_issues = True
        issues.append("  - **Missing Columns in Entity:**")
        for c in missing_in_entity:
            issues.append(f"    - `{c}`")
            
    if extra_in_entity:
        has_issues = True
        issues.append("  - **Extra Properties in Entity (Possible DB Mismatch):**")
        for c in extra_in_entity:
            issues.append(f"    - `{c}`")
            
    # Check Associations / Foreign Keys
    for assoc in db_info['associations']:
        fk_col = assoc['fk_col']
        member = assoc['member']
        
        if fk_col and fk_col not in entity['properties']:
            has_issues = True
            issues.append(f"  - **Missing FK Column for Relationship:** `{fk_col}` (Relationship to `{assoc['other_type']}`)")
        
        found_nav = False
        for prop_name, nav_info in entity['navigations'].items():
            if prop_name == member:
                found_nav = True
                if not nav_info['fk_linked_col']:
                     if fk_col != prop_name + "ID" and fk_col != prop_name + "Id":
                         has_issues = True
                         issues.append(f"    - [WARNING] Navigation `{prop_name}` for FK `{fk_col}` missing explicit `[ForeignKey]` attribute.")
                elif nav_info['fk_linked_col'] != fk_col:
                     has_issues = True
                     issues.append(f"    - [ERROR] Navigation `{prop_name}` points to FK `{nav_info['fk_linked_col']}` but DB expects `{fk_col}`.")
                break
    
    if has_issues:
        f.write(f"\n### Table: `{table_name}` (Class: `{class_name}`)\n")
        for line in issues:
            f.write(line + "\n")

if __name__ == "__main__":
    log("Script starting...")
    db_schema = parse_dbml(DBML_PATH)
    entities = parse_csharp_entities(ENTITIES_DIR)
    compare(db_schema, entities, "verification_report.md")
    log("Done.")
