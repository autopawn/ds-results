import sys
import os

import re
import json

# Wether to compact all restats as a single one to make a smaller JSON
AVERAGE_RESTARTS = True


# Check arguments
if len(sys.argv)!=3:
    print("usage: python3 %s <input_folder> <output_json>"%(sys.argv[0]))
    sys.exit(1)


arg_inputfolder  = sys.argv[1]
arg_outputfile = sys.argv[2]


# Regular expressions
dc2_cputime = re.compile(r'# CPU_TIME:?\s+(\S+)')
dc2_rst = re.compile(r'# RST:?\s+(\S+)\s+(\S+)\s+(\S+)')
popstar_cputime = re.compile(r'cputime (\S+)')
popstar_bestsol = re.compile(r'bestsol (\S+)')

karapetyan_cputime = re.compile(r'Time: (\S+)')
karapetyan_bestsol = re.compile(r'Solution found: (\S+)')




results = {}

for dirpath, dirnames, filenames in os.walk(arg_inputfolder):
    # Get call data
    if "call.txt" in filenames:
        filepath = dirpath+"/"+"call.txt"
        with open(filepath) as fo:
            last_call = fo.read()

    # This is not a terminal folder, continue
    if len(dirnames)>0: continue

    # Get the name of the group and problem set
    dirpath_split = dirpath.split("/")
    if len(dirpath_split)<3: continue


    group    = dirpath_split[1]
        # note that dirpath_split[2:-1] are not used if present
    probset  = "/".join(dirpath_split[2:])

    if group not in results:
        results[group] = {}
        results[group]["call"] = last_call
        results[group]["vals"] = {}
    if probset not in results[group]["vals"]: results[group]["vals"][probset] = {}

    for fname in filenames:

        filepath = dirpath+"/"+fname
        with open(filepath) as fo:
            # print("filepath: "+filepath)
            
            txt = fo.read()

            # Parse solutions according to expected format
            if ("dc" in group) or ("grasp" in group) or ("search" in group):
                # Parse according to dc
                entries = dc2_rst.findall(txt)
                entries = list(map(lambda x: (-float(x[1]),float(x[2])), entries))

                # Compute preprocesing time
                cputime = float(dc2_cputime.findall(txt)[0])
                # print("cputime")
                # print(cputime)
                preproc_time = cputime - sum([e[1] for e in entries])
                preproc_time = max(0,preproc_time)
                # print(preproc_time)

                # Add preprocesing time as entry 0
                entries = [[float("inf"),preproc_time]] + entries

                # Compact all entries into a single one
                if AVERAGE_RESTARTS:
                    best_cost  = min([x[0] for x in entries])
                    total_time = sum([x[1] for x in entries])
                    entries = ((best_cost,total_time),)

            elif ("popstar" in group):
                try:
                    cputime = float(popstar_cputime.findall(txt)[0])
                    bestsol = float(popstar_bestsol.findall(txt)[0])
                    entries = ((bestsol,cputime),)
                except:
                    print("Parse error on: "+filepath)
                    exit(1)

            elif ("cmcs" in group):
                try:
                    cputime = float(karapetyan_cputime.findall(txt)[0])
                    bestsol = float(karapetyan_bestsol.findall(txt)[0])
                    entries = ((bestsol,cputime),)
                except:
                    print("Parse error on: "+filepath)
                    # exit(1)

            else: continue

            results[group]["vals"][probset][fname] = entries

with open(arg_outputfile,'w') as outfile:
    json.dump(results, outfile, indent=2)


