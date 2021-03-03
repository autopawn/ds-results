import sys
import os

# Check arguments
reg_args = sys.argv
if len(reg_args) not in (2,3):
    print("usage: python3 %s <input_json> [<output_folder>]"%(reg_args[0]))
    sys.exit(1)

arg_jsonfile  = reg_args[1]
arg_outputfolder = reg_args[2] if len(reg_args)>2 else None


if arg_outputfolder:
    import matplotlib
    matplotlib.use('Agg')

import matplotlib.pyplot as plt

import numpy as np
import json

import subprocess

PROBLEMSET = "problems_kmedian_custom"

# Variants present on each plot
PLOTS_VARIANTS = [
    {"title": "Different pool sizes $k$",
        "methods" : ["ds-pool_40","ds-pool_60","ds-standard","ds-pool_100","ds-pool_120","ds-pool_160"],
        "fname" : "poolsizes",
        "alias" : {"ds-standard" : "ds-pool_80"},
    },
    {"title": "Different branching factor $B$",
        "methods" : ["ds-B_4","ds-B_8","ds-B_12","ds-standard","ds-B_20","ds-B_24"],
        "fname" : "branching",
        "alias" : {"ds-standard" : "ds-B_16"},
    },
    {"title": "Comparison with random selection methods",
        "methods" : ["ds-standard","ds-rand1","ds-rank1","ds-rand","ds-rank"],
        "fname" : "selmethod",
        "alias" : {"ds-standard" : "ds-sdbs+"},
    },
    {"title": "Different solution distances" ,
        "methods" : ["ds-standard","ds-mgemin","ds-hausum","ds-haumin","ds-pcd","ds-indexval"], # "ds-autosum","ds-automin"],
        "fname" : "distances",
        "alias" : {"ds-standard" : "ds-mgesum"},
    },
    {"title": "Different selection procedures" ,
        "methods" : ["ds-standard","ds-sdbs","ds-vrh1","ds-vrh2","ds-vrh4"], #FIXME: check for "ds-vrh4" ?
        "fname" : "selections",
        "alias" : {"ds-standard" : "ds-sdbs+"},
    },
    {"title": "Different filters" ,
        "methods" : ["ds-f2","ds-standard","ds-f4"],
        "fname" : "filters",
        "alias" : {"ds-standard" : "ds-f3"},
    },
    {"title": "Comparison with different local search policies" ,
        "methods" : ["ds-standard","ds-all","ds-allx","ds-x"],
        "fname" : "lspolicy1",
    },
    {"title": "Comparison with different local search policies (excluding ds-x)" ,
        "methods" : ["ds-standard","ds-all","ds-allx"],
        "fname" : "lspolicy2",
    },
    {"title": "Different local search methods" ,
        "methods" : ["ds-standard","ds-whitaker-best","ds-whitaker-1st","ds-no-ls"],
        "fname" : "lsmethod1",
        "alias" : {"ds-standard" : "ds-resende-werneck"},
    },
    {"title": "Different local search methods (excluding ds-no-ls)" ,
        "methods" : ["ds-standard","ds-whitaker-best","ds-whitaker-1st"],
        "fname" : "lsmethod2",
        "alias" : {"ds-standard" : "ds-resende-werneck"},
    },
    {"title": "Usage of restarts and different pool sizes $k$",
        "methods" : ["ds-standard","ds-R4_80","ds-R8_80","ds-R4_40","ds-R16_20"],
        "fname" : "restarts",
        "alias" : {"ds-standard" : "ds-R1_80"},
    },
    {"title": "Usage of Path Relinking post-optimization" ,
        "methods" : ["ds-standard","ds-P_best80","ds-M_best80","ds-P_best160","ds-M_best160"],
        "fname" : "postopt",
    },
]

# All algorithms in order of apparison
METHODS = []
for plot_variant in PLOTS_VARIANTS:
    for var in plot_variant["methods"]:
        if var not in METHODS: METHODS.append(var)

# Abbreviates a call command, deleting unnecessary arguments
def method_call_name_abreviate(call):
    goodargs = [x.split(':')[0] for x in call.split() if (x[0]=='-' and x[1]!='-') or ':' in x]
    return ' '.join(goodargs)

# Load input data
with open(arg_jsonfile) as fo:
    data = json.load(fo)

# Just select the kg problems
entries = {}
calls = {}
for method in data:
    # Method name
    mname = method

    # Add method results to entries
    if PROBLEMSET in data[method]["vals"]:
        entries[method] = data[method]["vals"][PROBLEMSET]
        calls[method]   = method_call_name_abreviate(data[method]["call"])

# Print, for checking
for mname,val in entries.items():
    print(mname)
    print(val)
for mname,val in calls.items():
    print(mname)
    print(val)

# For each problem the minimum known solution
min_known_sol = {}

# Get the minimum known value for each solution
for method in entries:

    # Update the minimum known for this problem:
    for prob in entries[method]:
        entrs = np.array(entries[method][prob])
        if prob not in min_known_sol:
            min_known_sol[prob] = float('inf')
        min_known_sol[prob] = min(min_known_sol[prob],np.min(entrs[:,0]))

print("Minimum values known:")
for prob in sorted(list(min_known_sol.keys())):
    print(("%16s : %.2f")%(prob,min_known_sol[prob]))

# For each method, get the average relative error and average time
avg_rel_value = {}
avg_time      = {}

all_rel_value = {}
all_time      = {}

for method in entries:
    # Get the problems solved with this method
    problems_filtered = list(entries[method].keys())

    # Assert that we got all the problems for this method
    total_n_problems = len(min_known_sol.keys())
    method_n_problems = len(entries[method].keys())
    if method_n_problems!=total_n_problems:
        print("WARNING: %s incomplete: %d/%d problems"%(method,method_n_problems,total_n_problems))
        # sys.exit(1)

    # Read the values for each problem
    rel_value = []
    times  = []

    for prob in entries[method]:
        # Find the best relative error among restarts
        entrs = np.array(entries[method][prob])
        best_value = np.min(entrs[:,0])/min_known_sol[prob]
        assert(best_value>=1.0)
        # Add the times of all restarts
        sumtime = np.sum(entrs[:,1])
        # Append the best relative error and total times of this method
        rel_value.append(best_value)
        times.append(sumtime)

    # Merge relative values and times
    all_rel_value[method] = rel_value
    avg_rel_value[method] = np.mean(rel_value)
    all_time[method]      = times
    avg_time[method]      = np.mean(times)

print(avg_rel_value)
print(avg_time)

COLORS = ['tab:blue', 'tab:orange', 'tab:green', 'tab:red', 'tab:purple', 'tab:brown', 'tab:pink', 'tab:gray', 'tab:olive', 'tab:cyan']

first_line = None

# === Plot
for k,plot_variant in enumerate(PLOTS_VARIANTS):

    plt.clf()
    fig = plt.Figure(figsize=(28,10))
    plt.suptitle("Relative cost v/s CPU time for kmedian_custom problem set")
    plt.title(plot_variant["title"])

    max_time = 0
    max_rel_value = 0

    for phase in (1,0):

        if phase==0:
            time_methods = [x for x in METHODS] # if x not in plot_variant["methods"]]
        else:
            time_methods = plot_variant["methods"]

        for i,method in enumerate(time_methods):
            args = {}

            if phase==0:
                args["color"] = "black"
                # args["alpha"] = 0.2
                args["markersize"] = 4
                args["linewidth"] = 1
            else:
                args["color"] = COLORS[0] if method=="ds-standard" else COLORS[1+i]
                args["label"] = method
                if "alias" in plot_variant:
                    if method in plot_variant["alias"]:
                        args["label"] = plot_variant["alias"][method] + " (" + method + ")"
                args["markersize"] = 9

            fmt = "s" if method=="ds-standard" else "o"

            plt.plot(avg_time[method],avg_rel_value[method],fmt,**args)

            if phase==1 and k==0:
                if not first_line:
                    first_line = [[],[]]
                first_line[0].append(avg_time[method])
                first_line[1].append(avg_rel_value[method])

            if phase==1:
                del args["label"]
                plt.plot(all_time[method],all_rel_value[method],fmt,marker='4',alpha=0.5,**args)

                time_end = np.ceil(np.max(all_time[method])/10)*10
                max_time = max(max_time,time_end)
                max_rel_value = max(max_rel_value,np.max(all_rel_value[method]))

            if phase==0 and i==0:
                plt.plot(first_line[0],first_line[1],"-",**args)




    plt.xlim((-0.5,max_time))
    plt.ylim((1-3e-5,1+(max_rel_value-1)*1.02))
    plt.grid()
    plt.legend()


    if arg_outputfolder:
        # fig.set_size_inches(24,10, forward=True)
        img_file = os.path.join(arg_outputfolder,"kmed_"+plot_variant["fname"]+".png")
        plt.savefig(img_file)
    else:
        plt.show()


# Copy json file
if arg_outputfolder:
    subprocess.run(["cp",arg_jsonfile,os.path.join(arg_outputfolder,"data.json")])

