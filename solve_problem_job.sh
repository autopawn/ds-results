#!/bin/bash -e
#SBATCH --output=out/%x_%A_%a.out
#SBATCH --error=out/%x_%A_%a.err
#SBATCH --mail-user=franciscojacb@gmail.com
#SBATCH --mail-type=ALL
#SBATCH --array=1-100%15   ## Separate into different tasks, up to 15 runing at the same time

# This scripts recieves a series of directories or particular problems and runs them, on a task array or a single process
#
# Usage:
#
# ./solver_problem_job.sh <name> <folders> <command> <arg1> <arg2> ...
# ./solver_problem_job.sh example "folder1 folder2" ./dc2/bin/dc -L rank:5
#
# The last 2 arguments of the command (the input and output file) will be passed to the command.
#

strategyname=$1
probgroups=$2

# Get all problems on the problem groups
problems=""
for pgroup in $probgroups ; do
    ppath=$pgroup
    if [ -f $ppath ]; then
        # Append problem fname
        problems=$ppath" "$problems
    elif [ -d $ppath ]; then
        # Get filenames of the problems of this group, ignore optimal and best solution files
        fnames=$(find $ppath/* -type f | grep -v '\.opt' | grep -v '\.bub')
        problems=$fnames" "$problems
    else
        exit 1
    fi

done

# Just pick the problems that correspond to this thread
problems=$(echo $problems | tr " " "\n" | sort)
if [[ -v SLURM_ARRAY_TASK_ID ]]; then
    problems=$(echo "$problems" | tr " " "\n" | awk "NR % $SLURM_ARRAY_TASK_COUNT == $SLURM_ARRAY_TASK_ID - 1")
    echo "=== $SLURM_ARRAY_TASK_ID"
    echo $problems | tr " " "\n"
    echo "==="
fi

# Solve each problem
for fname in $problems ; do
    target=results/$strategyname/"$fname"
    mkdir -p "$(dirname $target)"
    if test -s "$target" ; then
        echo $target already exists. Skipping.
    else
        # Run dc2, discard output if running on SLURM
        if [[ -v SLURM_ARRAY_TASK_ID ]]; then
            ${@:3} $fname $target >/dev/null
        else
            ${@:3} $fname $target
        fi
    fi
done

# [ -s results/cmcs-3comp_1000/problems_kg_custom_d/asym_3000d-1 ]
