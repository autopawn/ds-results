import sys
import os

# Check arguments
reg_args = sys.argv
if len(reg_args) not in (3,4):
    print("usage: python3 %s [probles_kg_custom|probles_kg_custom_d] <input_json> [<output_folder>]"%(reg_args[0]))
    sys.exit(1)

arg_jsonfile  = reg_args[2]
arg_outputfolder = reg_args[3] if len(reg_args)>3 else None


# Parse the PROBLEMSET
assert(reg_args[1] in ("problems_kg_custom","problems_kg_custom_d","problems_kg_custom_e"))
PROBLEMSET = reg_args[1]

if PROBLEMSET=="problems_kg_custom_d":
    REL_ERROR_PLOT_YLIM = [0.0002,0.0002,0.0002]
elif PROBLEMSET=="problems_kg_custom_e":
    REL_ERROR_PLOT_YLIM = [0.0002,0.0002,0.0002]
else:
    REL_ERROR_PLOT_YLIM = [0.0002,0.0001,0.00003]


if arg_outputfolder:
    import matplotlib
    matplotlib.use('Agg')

import matplotlib.pyplot as plt

import numpy as np
import json

import subprocess

# Amount of error to show for each experiment
EXPERIMENT_TITLES   = ["Strategies with $b=N$", "Strategies with $b=8$", "Strategies w/post-optimization and state-of-the-art"]
NUMBER_EXPERIMENTS = len(REL_ERROR_PLOT_YLIM)

# Xranges for showing the values
xranges = {
    "250a" :  20,
    "250b" :  10,
    "250c" :   2,
    "500a" : 100,
    "500b" :  50,
    "500c" :  10,
    "750a" : 200,
    "750b" : 100,
    "750c" :  20,
    "3000d" :200,
    "3000e" :200,
}

# Get colors
ax = plt.gca() # Color palette
colors = [next(ax._get_lines.prop_cycler)['color'] for i in range(8)]

c_beam = colors[6]
c_rand = colors[1]
c_gras = colors[5]
c_divs = colors[0]
c_samp = colors[4]
c_pops = colors[3]
c_cmcs = colors[2]
c_othe = colors[7]

# Line styles:
plot_styles = {                   # (plot, color, style, line width, dots each)
    "search-beam"               : ( 0 , c_beam , "2-"   ,  1 ,   0,    "beam"), #200),
    "search-rand"               : ( 0 , c_rand , "2-"   ,  1 ,   0,    "rand"), #200),
    "search-grasp10"            : ( 0 , c_gras , "2-"   ,  1 ,   0,    "grasp10"), #200),
    "search-grasp40"            : ( 0 , c_gras , "2-"   ,  2 ,   0,    "grasp40"), #200),
    "search-rand-sdbs-mgesum"   : ( 0 , c_divs , "2--"  ,  1 ,   0,    "ds-rand-sdbs"), #200),
    "search-rand-sdbsp-mgesum"  : ( 0 , c_divs , "2-"   ,  1 ,   0,    "ds-rand-sdbsp"), #200),

    "search-beam8"              : ( 1 ,  c_beam , "2-"  ,  1 ,   0,    "beam8"), #200),
    "search-rand8"              : ( 1 ,  c_rand , "2-"  ,  1 ,   0,    "rand8"), #200),
    "search-sample8"            : ( 1 ,  c_samp , "2-"  ,  1 ,   0,    "sample8"), #200),
    "search-sdbs-mgesum"        : ( 1 ,  c_divs , "2--"  , 1 ,   0,    "ds-sdbs"), #200),
    "search-sdbsp-mgesum"       : ( 1 ,  c_divs , "2-"  ,  1 ,   0,    "ds-sdbsp"), #200),

    "dc-best8-pr-best"          : ( 2 , c_beam , "2:"   ,  1 ,   0,    "best8-pr"),
    "dc-best8-pr-best2"         : ( 2 , c_beam , "2-"   ,  1 ,   0,    "best8-pr2"),
    "dc-rand8-pr-best"          : ( 2 , c_rand , "2:"   ,  1 ,   0,    "rand8-pr"),
    "dc-rand8-pr-best2"         : ( 2 , c_rand , "2-"   ,  1 ,   0,    "rand8-pr2"),
    "dc-sdbsp-mgesum-pr-best"   : ( 2 , c_divs , "2:"   ,  1 ,   0,    "ds-sdbsp-pr"),
    "dc-sdbsp-mgesum-pr-best2"  : ( 2 , c_divs , "2-"   ,  1 ,   0,    "ds-sdbsp-pr2"),
#   "dc-sdbs-mgesum-pr-best"    : ( 2 , c_othe , "2:"   ,  1 ,   0),
#   "dc-sdbs-mgesum-pr-best2"   : ( 2 , c_othe , "2-"   ,  1 ,   0),
#   "popstar-L"                 : ( 2 , c_pops , "2:"   ,  1 ,   0),
    "popstar-S"                 : ( 2 , c_pops , "2-"   ,  1 ,   0,    "popstar-S"),
    "cmcs-3comp"                : ( 2 , c_cmcs , "2-"   ,  1 ,   0,    "cmcs-3comp"),
}

# Wether to put algorithm paramaters in the legend
ALGORITHM_PARAMS_IN_LEGEND = False

def component_dicofdic_unionmap(dic,fn):
    keys = []
    for k in dic:
        for p in dic[k]:
            keys.append( list(map(fn,dic[k][p])) )
    keys = sorted(list(set(keys)))
    return keys

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
    # Ommit methods to hide
    mname = method.split("_")[0]

    # Add method results to entries
    if PROBLEMSET in data[method]["vals"]:
        entries[method] = data[method]["vals"][PROBLEMSET]
        calls[method] = method_call_name_abreviate(data[method]["call"])

del data

# Get the problem groups
pgroups = []
for method in entries:
    pgroups += entries[method].keys()
pgroups = sorted(list(set( map( lambda s: s.split("_")[1].split("-")[0] , pgroups )  )))

print("problem groups: %s"%str(pgroups))

# For each problem the minimum known solution
min_known_sol = {}

# Get the minimum known value for each solution
for pgroup in pgroups:
    for method in entries:

        # Get the problems on this pgroup
        problems_filtered = [p for p in entries[method] if ("_"+pgroup+"-") in p]

        # Update the minimum known for this problem:
        for prob in problems_filtered:
            entrs = np.array(entries[method][prob])
            if prob not in min_known_sol:
                min_known_sol[prob] = float('inf')
            min_known_sol[prob] = min(min_known_sol[prob],np.min(entrs[:,0]))

print("Minimum values known:")
for prob in sorted(list(min_known_sol.keys())):
    print(("%16s : %.2f")%(prob,min_known_sol[prob]))

# For each group of problems, print the data for each method
for pgroup in pgroups:

    # Number of problems in this group
    nprobs_group = None

    # === Get the data for the dc2 line and plot it
    results_data = {}
    results_call = {}

    for method in entries:

        # Size in the name
        size = float(method.split("_")[1])
        mname = method.split("_")[0]

        # Get the problems on this pgroup
        problems_filtered = [p for p in entries[method] if ("_"+pgroup+"-") in p]

        # Assert that we got all the problems of this group for this method
        if nprobs_group is None: nprobs_group = len(problems_filtered)
        if len(problems_filtered)!=nprobs_group:
            print("%s incomplete for %s: %d/%d"%(method,pgroup,len(problems_filtered),nprobs_group))
            # sys.exit(1)

        # Read the values for each problem
        values = []
        for prob in problems_filtered:
            entrs = np.array(entries[method][prob])
            bestval = np.min(entrs[:,0])/min_known_sol[prob] # relative error
            assert(bestval>=1.0)
            sumtime = np.sum(entrs[:,1])
            values.append( (bestval,sumtime) )
        values = np.array(values)

        avg_value = np.mean(values[:,0])
        avg_time = np.mean(values[:,1])

        if mname not in results_data: results_data[mname] = []
        results_data[mname].append( (size,avg_value,avg_time) )
        results_call[mname] = calls[method]

    # === Plot

    for experiment in range(NUMBER_EXPERIMENTS):

        rel_error_ylim = REL_ERROR_PLOT_YLIM[experiment]

        plt.clf()
        fig = plt.Figure(figsize=(28,10))
        plt.suptitle("Problem group "+pgroup)
        plt.title(EXPERIMENT_TITLES[experiment])

        i = 0;

        best_val = float('inf') # Best solution value so far

        mnames = sorted(results_data.keys())

        for time in (1,2):
            if time==1:
                time_mnames = [m for m in mnames if m in plot_styles and experiment != plot_styles[m][0]]
            else:
                time_mnames = [m for m in mnames if m in plot_styles and experiment == plot_styles[m][0]]

            for mname in time_mnames:

                data = np.array(sorted(results_data[mname]))

                grayline = experiment != plot_styles[mname][0]

                color     = "gray" if grayline else plot_styles[mname][1]
                linestyle = plot_styles[mname][2]
                width     = plot_styles[mname][3]
                dotstep   = plot_styles[mname][4]


                if grayline:
                    label = "void"
                    alpha = 0.25
                else:
                    mname2 = plot_styles[mname][5]

                    if ALGORITHM_PARAMS_IN_LEGEND:
                        label = mname2+" : "+results_call[mname]
                    else:
                        label = mname2
                    alpha = 1.0

                plt.plot(
                    data[:,2],
                    data[:,1],
                    linestyle,label=label,color=color,alpha=alpha,linewidth=width)

                if dotstep!=0:
                    plt.plot(
                        data[:,2][data[:,0]%dotstep==0],
                        data[:,1][data[:,0]%dotstep==0],
                        "o",label=label,color=color,alpha=alpha)

                best_val = min(best_val,np.min(data[:,1]))

                """
                labs = map(lambda x: str(int(x)),list(data[:,0]))

                labfrec = np.round(np.max(data[:,0])/500)*100
                nextlab = labfrec
                for k, txt in enumerate(labs):
                    if data[k,0]>=nextlab:
                        plt.annotate(txt,(data[k,2], data[k,1]))
                        nextlab += labfrec
                """

        #remove duplicated labels for legend
        handles, labels = plt.gca().get_legend_handles_labels()
        newLabels, newHandles = [], []
        for handle, label in zip(handles, labels):
            if label not in newLabels and label!="void":
                newLabels.append(label)
                newHandles.append(handle)

        plt.legend(newHandles, newLabels)

        if pgroup in xranges:
            plt.xlim((0,xranges[pgroup]))
        plt.ylim((best_val,best_val*(1+rel_error_ylim)))

        plt.grid()
        if arg_outputfolder:
            # fig.set_size_inches(24,10, forward=True)
            img_file = os.path.join(arg_outputfolder,pgroup+"_"+str(experiment)+".png")
            plt.savefig(img_file)
        else:
            plt.show()

# Copy json file
if arg_outputfolder:
    subprocess.run(["cp",arg_jsonfile,os.path.join(arg_outputfolder,"data.json")])


