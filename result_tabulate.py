import sys
import json
import numpy as np

# Check arguments
reg_args = sys.argv
if len(reg_args) != 2:
    print("usage: python3 %s <input_json> [<output_folder>]"%(reg_args[0]))
    sys.exit(1)
arg_jsonfile  = reg_args[1]

# Load input data
with open(arg_jsonfile) as fo:
    data = json.load(fo)


methods = sorted(list(data.keys()))
print(methods)

# Identify problem groups
groups = set()
for method in data:
    for group in data[method]['vals'].keys():
        groups.add(group)

groups = sorted(list(groups))


# For each group
for group in groups:
    # Identify all the group problems
    problems = set()
    for method in data:
        if group not in data[method]['vals']: continue

        for problem in data[method]['vals'][group]:
            problems.add(problem)
    problems = sorted(list(problems))

    # Table header
    print("===================== "+group+" =====================")
    row = "method:;"
    for method in methods:
        row += method+";;"
    print(row)
    row = "problem;"
    for method in methods:
        row += "value;time;"
    print(row)


    # For each problem of the group print a row
    for problem in problems:
        row = problem + ";"

        for method in methods:
            if group not in data[method]['vals'] or problem not in data[method]['vals'][group]:
                # Entry not present
                row += "?;?;"
            else:
                # Entry present, calculate averages
                entry = np.array(data[method]['vals'][group][problem],dtype=np.float64)
                avg_val,avg_time = np.mean(entry,axis=0)
                row += "%.2f;%d;"%(avg_val,np.ceil(avg_time))

        # Print the row
        print(row)







