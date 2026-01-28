#!/usr/bin/env python3
import csv
import sys
import re

def print_usage():
    print("Usage: ./merge --template=template.csv --[locale]=translation.csv output.csv")
    print("Example: ./merge --template=base.csv --ko=korean.csv result.csv")
    sys.exit(1)

def get_args():
    """Parses arguments manually to support dynamic locale flags."""
    args = {
        'template': None,
        'locales': {}, # Stores locale_code: file_path
        'output': None
    }
    
    # The last argument is the output file
    if len(sys.argv) < 4:
        print_usage()
    
    args['output'] = sys.argv[-1]
    
    # Parse flags
    for arg in sys.argv[1:-1]:
        if arg.startswith('--template='):
            args['template'] = arg.split('=', 1)[1]
        elif arg.startswith('--') and '=' in arg:
            # Detects flags like --ko=file.csv or --ja-JP=file.csv
            parts = arg[2:].split('=', 1)
            locale_code = parts[0].lower()
            file_path = parts[1]
            args['locales'][locale_code] = file_path
            
    if not args['template'] or not args['locales']:
        print_usage()
        
    return args

def find_matching_column(header_names, locale_code):
    """
    Matches a locale code (e.g., 'ko') to a header column 
    (e.g., 'Korean (South Korea)(ko-KR)')
    """
    # Pattern looks for the locale code inside parentheses: (ko- or (ko)
    pattern = re.compile(rf"\(({locale_code})\b", re.IGNORECASE)
    
    for header in header_names:
        if pattern.search(header):
            return header
    return None

def main():
    args = get_args()
    
    # Load all translation files into a nested dictionary: { locale_col_name: { key: value } }
    translations_map = {}
    
    try:
        with open(args['template'], mode='r', encoding='utf-8') as f_temp:
            reader = csv.DictReader(f_temp)
            fieldnames = reader.fieldnames
            
            # Map the flags (ko, ja) to actual column names in the CSV
            for code, path in args['locales'].items():
                col_name = find_matching_column(fieldnames, code)
                if not col_name:
                    print(f"Warning: Could not find a column in template matching locale code: {code}")
                    continue
                
                translations_map[col_name] = {}
                with open(path, mode='r', encoding='utf-8') as f_trans:
                    trans_reader = csv.reader(f_trans)
                    for row in trans_reader:
                        if len(row) >= 2:
                            translations_map[col_name][row[0].strip()] = row[1].strip()

        # Process the template and merge
        with open(args['template'], mode='r', encoding='utf-8') as f_in:
            reader = csv.DictReader(f_in)
            rows = list(reader)
            
        # Keep track of keys we've seen to handle potential new keys
        keys_in_template = set()
        
        # Update existing rows
        for row in rows:
            key = row.get('Key')
            keys_in_template.add(key)
            for col_name, trans_dict in translations_map.items():
                if key in trans_dict:
                    row[col_name] = trans_dict[key]

        # Handle keys that exist in translation files but NOT in the template
        all_keys_from_trans = set()
        for trans_dict in translations_map.values():
            all_keys_from_trans.update(trans_dict.keys())
            
        new_keys = all_keys_from_trans - keys_in_template
        for nk in new_keys:
            new_row = {fn: "" for fn in fieldnames}
            new_row['Key'] = nk
            for col_name, trans_dict in translations_map.items():
                if nk in trans_dict:
                    new_row[col_name] = trans_dict[nk]
            rows.append(new_row)

        # output
        with open(args['output'], mode='w', encoding='utf-8', newline='') as f_out:
            writer = csv.DictWriter(f_out, fieldnames=fieldnames)
            writer.writeheader()
            writer.writerows(rows)
            
        print(f"Successfully merged {len(translations_map)} locale(s) into {args['output']}")

    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
