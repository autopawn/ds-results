#!/bin/bash -xe

# Load variables
source ./variables.sh

# pref="echo"
pref=

$preset4cpumem="--partition=slims --cpus-per-task=4 --mem-per-cpu=130M"

# NOTE: may need to use "$preset4cpumem" instead of "$preset4cpumem" for LEFTRARU warnings regarding inneficient usage of reosurces.

targets="problems_kg_original problems_kmedian_original"

for target in $targets; do

    # DS standard algorithm
    bash ./solve_problem_set.sh "ds-100_200" "$preset4cpumem" "$target" \
        $pref ./dc2/bin/dc -V -W -M -t4 -B8 sdbs+:100:mgesum _best:200

    bash ./solve_problem_set.sh "ds-200_400" "$preset4cpumem" "$target" \
        $pref ./dc2/bin/dc -V -W -M -t4 -B8 sdbs+:200:mgesum _best:400

    bash ./solve_problem_set.sh "ds-300_600" "$preset4cpumem" "$target" \
        $pref ./dc2/bin/dc -V -W -M -t4 -B8 sdbs+:300:mgesum _best:600

    bash ./solve_problem_set.sh "ds-400_800" "$preset4cpumem" "$target" \
        $pref ./dc2/bin/dc -V -W -M -t4 -B8 sdbs+:400:mgesum _best:800


    # POPSTAR
    bash ./solve_problem_set.sh "popstar-61_1200" "$preset1cpu2gb" "$target" \
            $pref "bash -xe ./solve_popstar.sh" -graspit 1200 -elite 61

    bash ./solve_problem_set.sh "popstar-86_2400" "$preset1cpu2gb" "$target" \
            $pref "bash -xe ./solve_popstar.sh" -graspit 2400 -elite 86

    bash ./solve_problem_set.sh "popstar-105_3600" "$preset1cpu2gb" "$target" \
            $pref "bash -xe ./solve_popstar.sh" -graspit 3600 -elite 105

    bash ./solve_problem_set.sh "popstar-122_4800" "$preset1cpu2gb" "$target" \
            $pref "bash -xe ./solve_popstar.sh" -graspit 4800 -elite 122

    # Karapetyan
    bash ./solve_problem_set.sh "cmcs3-250" "$preset1cpu2gb" "$target" \
        $pref "bash -e ./solve_karapetyan.sh" 250s

    bash ./solve_problem_set.sh "cmcs3-500" "$preset1cpu2gb" "$target" \
        $pref "bash -e ./solve_karapetyan.sh" 500s

    bash ./solve_problem_set.sh "cmcs3-750" "$preset1cpu2gb" "$target" \
        $pref "bash -e ./solve_karapetyan.sh" 750s

    bash ./solve_problem_set.sh "cmcs3-1000" "$preset1cpu2gb" "$target" \
        $pref "bash -e ./solve_karapetyan.sh" 1000s

done
