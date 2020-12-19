#!/bin/bash -e

# This scripts recieves a series of directories or particular problems and runs them using bash or sbash
#
# Usage:
#
# ./solver_problem_set <name> <bashargs> <folders> <command> <arg1> <arg2> ...
# ./solver_problem_set example "--partition=slims --array=1" "folder1 folder2" ./dc2/bin/dc -L rank:5
#
# The last 2 arguments of the command (the input and output file) will be passed to the command.
#

# Check that at least the strategy and problem groups were provided
if [ "$#" -lt 3 ]; then
  echo "Usage: bash $0 <strategyname> <sbashargs> <probgroups> {command}" >&2
  exit 1
fi

strategyname=$1
sbashargs=$2
probgroups=$3

# # Empty results/$1
# rm -rf results/$strategyname || true

mkdir -p results/$strategyname

# Save the command line arguments
CALLFILE="results/$strategyname/call.txt"
if [ ! -f "$$CALLFILE" ]; then
  echo "$sbashargs" > "$CALLFILE"
  echo $0 $1 \"$2\" \"$3\" ${@:4} >> "$CALLFILE"
fi



# Solve problems on each problem group
if [ -x "$(command -v sbatch)" ]; then
    echo 'sbatch command detected, running script job array.'
    mkdir -p out
    sbatch $sbashargs -J "$strategyname" solve_problem_job.sh "$strategyname" "$probgroups" ${@:4}
else
    echo 'sbatch command not detected, running directly.'
    bash -e ./solve_problem_job.sh "$strategyname" "$probgroups" ${@:4}
fi

