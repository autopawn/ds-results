#!/bin/bash -xe

# Load variables
source ./variables.sh

# EXPERIMENT 5!

# pref="echo"
pref=

poolsizs100="$(seq 2 2 30)
$(seq 35 5 100)"

poolsizs200="$(seq 4 4 60)
$(seq 70 10 200)"

poolsizs250="$(seq 5 5 80)
$(seq 90 15 250)"

poolsizs400="$(seq 5 5 100)
$(seq 120 20 400)"

# target=problems_kg_custom_d
target=problems_kmedian_custom

# =================================================================
# Branching all solutions
# =================================================================

# {grasp10}
# GRASP, expanding all the child solutions, always selecting 1 between the best 10. With $\alpha$ restarts.
for alpha in $poolsizs200; do
    name="grasp10"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$preset1cpu2gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t1 -R"$alpha" best:10 rand:1
done

# {grasp40}
# GRASP, expanding all the child solutions, always selecting 1 between the best 40. With $\alpha$ restarts.
for alpha in $poolsizs200; do
    name="grasp40"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$preset1cpu2gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t1 -R"$alpha" best:40 rand:1
done

# {rand-sdbs-mgesum}
# Diverse search, with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS.
for alpha in $poolsizs200; do
    name="rand-sdbs-mgesum"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t10 rand1:"$((8*alpha))" sdbs:"$alpha":mgesum
done

# {rand-sdbsp-mgesum}
# Diverse search with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS+.
for alpha in $poolsizs200; do
    name="rand-sdbsp-mgesum"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t10 rand1:"$((8*alpha))" sdbs+:"$alpha":mgesum
done

# {beam}
# Beam search, expanding all the child solutions and selecting the $\alpha$ best.
for alpha in $poolsizs250; do
    name="beam"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t10 best:"$alpha"
done

# {rand}
# Stochastic beam search, expanding all the child solutions and selecting $\alpha$ solutions at random.
for alpha in $poolsizs200; do
    name="rand"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t10 rand:"$alpha"
done

# =================================================================
# Small branching factor
# =================================================================

# {beam8}
# Beam search, with branching factor $b=8$ and selecting the $\alpha$ best solutions.
for alpha in $poolsizs250; do
    name="beam8"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -B8 -t10 best:"$alpha"
done

# {sdbs-mgesum}
# Diverse search, with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS.
for alpha in $poolsizs250; do
    name="sdbs-mgesum"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -B8 -t10 sdbs:"$alpha":mgesum
done

# {sdbsp-mgesum}
# Diverse search with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS+.
for alpha in $poolsizs250; do
    name="sdbsp-mgesum"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -B8 -t10 sdbs+:"$alpha":mgesum
done

# {rand8}
# Stochastic beam search, with branching factor $b=8$ and selecting $\alpha$ solutions at random.
for alpha in $poolsizs250; do
    name="rand8"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -B8 -t10 rand:"$alpha"
done

# {sample8}
# With branching factor $b=8$, always select the best solution. With $\alpha$ restarts.
# NOTE: This is a really weird algorithm, maybe deserves a paper on its own! Inverted GRASP?
for alpha in $poolsizs250; do
    name="sample8"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$preset1cpu2gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t1 -B8 -R"$alpha" best:1
done

# =================================================================
# State of the art
# =================================================================

for alpha in $poolsizs200; do
    name="sdbsp-mgesum-pr-best"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -M -B8 -t10 sdbs+:"$alpha":mgesum _best:"$alpha"
done

for alpha in $poolsizs100; do
    name="sdbsp-mgesum-pr-best2"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -M -B8 -t10 sdbs+:"$alpha":mgesum _best:"$((2*alpha))"
done

for alpha in $poolsizs200; do
    name="sdbsp-mgemin-pr-best"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -M -B8 -t10 sdbs+:"$alpha":mgemin _best:"$alpha"
done

for alpha in $poolsizs100; do
    name="sdbsp-mgemin-pr-best2"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -M -B8 -t10 sdbs+:"$alpha":mgemin _best:"$((2*alpha))"
done


for alpha in $poolsizs200; do
    name="rand8-pr-best"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -M -B8 -t10 rand:"$alpha" _best:"$alpha"
done

for alpha in $poolsizs100; do
    name="rand8-pr-best2"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -M -B8 -t10 rand:"$alpha" _best:"$((2*alpha))"
done

for alpha in $poolsizs200; do
    name="best8-pr-best"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -M -B8 -t10 best:"$alpha" _best:"$alpha"
done

for alpha in $poolsizs100; do
    name="best8-pr-best2"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -M -B8 -t10 best:"$alpha" _best:"$((2*alpha))"
done



# Run Karapetyan's best 3 component CMCS result
# NOTE: must be run several times because some times it doesn't print output!
params3comp="5 10 15 20 25 30 40 50 75 100 150 200 250 300 400 500 600 700 800 1000" #1000 is 100%, minimum is 25
for alpha in $params3comp; do
    name="3comp"
    bash ./solve_problem_set.sh cmcs-"$name"_"$alpha" "$preset1cpu2gb" "$target" \
        $pref "bash -e ./solve_karapetyan.sh" "$alpha"
done

# Solve using POPSTAR S
popstarSRP="4-14 6-20 8-25 9-29 10-32 11-35 12-38 13-40 14-45 15-50 17-54 18-57 19-61 20-64"
for pair in $popstarSRP; do
    poolsize=$(echo "$pair" | cut -d'-' -f1)
    restarts=$(echo "$pair" | cut -d'-' -f2)
    bash ./solve_problem_set.sh popstar-S_"$poolsize"_"$restarts"r "$preset1cpu2gb" "$target" \
        $pref "bash -xe ./solve_popstar.sh" -graspit $restarts -elite $poolsize
done

# cost = 3200/50 * time
# alpha = sqrt(cost/3200)
# poolsize = round(10 * alpha**0.5)
# restarts = round(32 * alpha)

# time	cost	a	k	r
# 10	640	0.447213595499958	4	14
# 20	1280	0.632455532033676	6	20
# 30	1920	0.774596669241483	8	25
# 40	2560	0.894427190999916	9	29
# 50	3200	1	10	32
# 60	3840	1.09544511501033	11	35
# 70	4480	1.18321595661992	12	38
# 80	5120	1.26491106406735	13	40
# 100	6400	1.4142135623731	14	45
# 120	7680	1.54919333848297	15	50
# 140	8960	1.67332005306815	17	54
# 160	10240	1.78885438199983	18	57
# 180	11520	1.89736659610103	19	61
# 200	12800	2	20	64

# # Solve using POPSTAR L
# popstarLRP="6-20 10-32 13-42 16-50 18-57 22-70 24-76 25-81"
# for pair in $popstarLRP; do
#     poolsize=$(echo "$pair" | cut -d'-' -f1)
#     restarts=$(echo "$pair" | cut -d'-' -f2)
#     bash ./solve_problem_set.sh popstar-L_"$poolsize"_"$restarts"r "$preset1cpu2gb" "$target" \
#         $prefix "bash -xe ./solve_popstar.sh" -graspit $restarts -elite $poolsize
# done


