#!/bin/bash -xe

# Load variables
source ./variables.sh

# pref="echo"
pref=

target=problems_kmedian_custom

# Selected preset
preset="$preset4cpu8gb"

# Standard algorithm
bash ./solve_problem_set.sh "ds-standard" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:80:mgesum

# Different pool sizes
bash ./solve_problem_set.sh "ds-pool_40" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:40:mgesum

bash ./solve_problem_set.sh "ds-pool_60" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:60:mgesum

bash ./solve_problem_set.sh "ds-pool_100" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:100:mgesum

bash ./solve_problem_set.sh "ds-pool_120" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:120:mgesum

bash ./solve_problem_set.sh "ds-pool_160" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:160:mgesum

# Different branching factors
bash ./solve_problem_set.sh "ds-B_4" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B4 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-B_8" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B8 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-B_12" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B12 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-B_20" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B20 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-B_24" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B24 sdbs+:80:mgesum

# Rank and random selection instead of branching factor
bash ./solve_problem_set.sh "ds-rand1" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 rand1:1280 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-rank1" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 rank1:1280 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-rand" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 rand:1280 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-rank" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 rank:1280 sdbs+:80:mgesum

# Solution distances
bash ./solve_problem_set.sh "ds-mgemin" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:80:mgemin

bash ./solve_problem_set.sh "ds-hausum" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:80:hausum

bash ./solve_problem_set.sh "ds-haumin" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:80:haumin

bash ./solve_problem_set.sh "ds-pcd" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:80:pcd

bash ./solve_problem_set.sh "ds-autosum" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:80:autosum

bash ./solve_problem_set.sh "ds-automin" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:80:automin

bash ./solve_problem_set.sh "ds-indexval" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs+:80:indexval

# Selection strategy
bash ./solve_problem_set.sh "ds-sdbs" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 sdbs:80:mgesum

bash ./solve_problem_set.sh "ds-vrh2" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 vrh:80:mgesum:160

bash ./solve_problem_set.sh "ds-vrh2" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 vrh:80:mgesum:320

# Filters
bash ./solve_problem_set.sh "ds-f2" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 -f2 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-f4" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 -f4 sdbs+:80:mgesum

# Local search on all solution levels, local search keeping sizes constant
bash ./solve_problem_set.sh "ds-all" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 -A sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-allx" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 -A -x sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-x" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -B16 -x sdbs+:80:mgesum

# Different locals searches
bash ./solve_problem_set.sh "ds-whitaker-best" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -w -t4 -B16 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-whitaker-1st" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -L -t4 -B16 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-no-ls" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -l -t4 -B16 sdbs+:80:mgesum

# Size versus restarts
bash ./solve_problem_set.sh "ds-R8_80" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -R8 -B16 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-R4_80" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -R4 -B16 sdbs+:80:mgesum

bash ./solve_problem_set.sh "ds-R4_40" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -R4 -B16 sdbs+:40:mgesum

bash ./solve_problem_set.sh "ds-R16_20" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -R16 -B16 sdbs+:20:mgesum

# Path relinking post optimization
bash ./solve_problem_set.sh "ds-P_best80" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -P -B16 sdbs+:80:mgesum _best:80

bash ./solve_problem_set.sh "ds-M_best80" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -M -B16 sdbs+:80:mgesum _best:80

bash ./solve_problem_set.sh "ds-P_best160" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -P -B16 sdbs+:80:mgesum _best:160

bash ./solve_problem_set.sh "ds-M_best160" "$preset" "$target" \
    $pref ./dc2/bin/dc -V -W -t4 -M -B16 sdbs+:80:mgesum _best:160


# bash ./solve_problem_set.sh "ds-P" "$preset" "$target" \
#     $pref ./dc2/bin/dc -V -W -t4 -P -B16 sdbs+:80:mgesum

# bash ./solve_problem_set.sh "ds-M" "$preset" "$target" \
#     $pref ./dc2/bin/dc -V -W -t4 -M -B16 sdbs+:80:mgesum
