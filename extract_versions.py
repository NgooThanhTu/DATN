
import sys

def extract_versions(filename):
    with open(filename, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    def recursive_extract(lines, side):
        # side can be 'ours' or 'theirs'
        result = []
        i = 0
        while i < len(lines):
            line = lines[i]
            if line.startswith('<<<<<<< ours'):
                # Start of conflict
                inner_ours = []
                inner_theirs = []
                # Find the middle '=======' and ending '>>>>>>> theirs'
                # But handle nesting
                depth = 1
                i += 1
                start_i = i
                # Find ======= for THIS level
                while i < len(lines):
                    if lines[i].startswith('<<<<<<< ours'):
                        depth += 1
                    elif lines[i].startswith('>>>>>>> theirs'):
                        depth -= 1
                    elif lines[i].startswith('=======') and depth == 1:
                        break
                    i += 1
                
                middle_i = i
                depth = 1
                i += 1
                # Find >>>>>>> theirs for THIS level
                while i < len(lines):
                    if lines[i].startswith('<<<<<<< ours'):
                        depth += 1
                    elif lines[i].startswith('>>>>>>> theirs'):
                        depth -= 1
                        if depth == 0:
                            break
                    i += 1
                end_i = i
                
                if side == 'ours':
                    result.extend(recursive_extract(lines[start_i:middle_i], side))
                else:
                    result.extend(recursive_extract(lines[middle_i+1:end_i], side))
                i += 1
            else:
                result.append(line)
                i += 1
        return result

    ours_version = recursive_extract(lines, 'ours')
    theirs_version = recursive_extract(lines, 'theirs')

    with open(filename + '.ours', 'w', encoding='utf-8') as f:
        f.writelines(ours_version)
    with open(filename + '.theirs', 'w', encoding='utf-8') as f:
        f.writelines(theirs_version)

if __name__ == "__main__":
    extract_versions(sys.argv[1])
