#!/bin/bash -xe

# Load variables
source ./variables.sh

# pref="echo"
pref=

poolsizs200="$(seq 4 4 60)
$(seq 70 10 200)"

poolsizs400="$(seq 5 5 100)
$(seq 120 20 400)"

poolsizs1000="$(seq 10 10 200)
$(seq 250 50 1000)"

poolsizs300020="$(seq 40 40 600)
$(seq 720 120 3000)"

poolsizs1600="$(seq 10 10 200)
$(seq 250 50 1600)"

poolsizs9600="$(seq 40 40 800)
$(seq 1000 200 9600)"

target=problems_kg_custom
# target=problems_kg_custom_d
# target=problems_kmedian_custom


# =================================================================
# Branching all solutions
# =================================================================

# {grasp10}
# GRASP, expanding all the child solutions, always selecting 1 between the best 10. With $\alpha$ restarts.
for alpha in $poolsizs1600; do
    name="grasp10"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$preset1cpu2gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t1 -R"$alpha" best:10 rand:1
done

# {grasp40}
# GRASP, expanding all the child solutions, always selecting 1 between the best 40. With $\alpha$ restarts.
for alpha in $poolsizs1600; do
    name="grasp40"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$preset1cpu2gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t1 -R"$alpha" best:40 rand:1
done

# # {sdbs-pcd}
# # Diverse search, with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS.
# # NOTE: gives slightly worse results than sdbs-mgesum
# for alpha in $poolsizs400; do
#     name="sdbs-pcd"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B8 -t10 sdbs:"$alpha"
# done

# # {sdbsp-pcd}
# # Diverse search with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS+.
# # # NOTE: gives slightly worse results than sdbsp-mgesum
# for alpha in $poolsizs400; do
#     name="sdbsp-pcd"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B8 -t10 sdbs+:"$alpha"
# done

# {rand-sdbs-mgesum}
# Diverse search, with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS.
for alpha in $poolsizs400; do
    name="rand-sdbs-mgesum"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t10 rand1:"$((8*alpha))" sdbs:"$alpha":mgesum
done

# {rand-sdbsp-mgesum}
# Diverse search with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS+.
for alpha in $poolsizs400; do
    name="rand-sdbsp-mgesum"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t10 rand1:"$((8*alpha))" sdbs+:"$alpha":mgesum
done

# {beam}
# Beam search, expanding all the child solutions and selecting the $\alpha$ best.
for alpha in $poolsizs1600; do
    name="beam"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t10 best:"$alpha"
done

# {rand}
# Stochastic beam search, expanding all the child solutions and selecting $\alpha$ solutions at random.
for alpha in $poolsizs1000; do
    name="rand"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t10 rand:"$alpha"
done

# =================================================================
# Small branching factor
# =================================================================

# {beam8}
# Beam search, with branching factor $b=8$ and selecting the $\alpha$ best solutions.
for alpha in $poolsizs9600; do
    name="beam8"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -B8 -t10 best:"$alpha"
done

# # {beam8f4}
# # Beam search, with branching factor $b=8$ and selecting the $\alpha$ best solutions, with filter 4.
# # NOTE: PENDING, probably not worth it
# for alpha in $poolsizs9600; do
#     name="beam8f4"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B8 -f4 -t10 best:"$alpha"
# done


# {sdbs-mgesum}
# Diverse search, with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS.
for alpha in $poolsizs400; do
    name="sdbs-mgesum"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -B8 -t10 sdbs:"$alpha":mgesum
done

# {sdbsp-mgesum}
# Diverse search with branching factor $b=8$ and selecting $k=\alpha$ solutions using SDBS+.
for alpha in $poolsizs400; do
    name="sdbsp-mgesum"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -B8 -t10 sdbs+:"$alpha":mgesum
done

# {rand8}
# Stochastic beam search, with branching factor $b=8$ and selecting $\alpha$ solutions at random.
for alpha in $poolsizs9600; do
    name="rand8"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $pref ./dc2/bin/dc -V -W -B8 -t10 rand:"$alpha"
done

# # {rand8f4}
# # Stochastic beam search, with branching factor $b=8$ and selecting $\alpha$ solutions at random, with filter 4.
# # NOTE: Gives worse results than rand8
# for alpha in $poolsizs9600; do
#     name="rand8f4"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B8 -f4 -t10 rand:"$alpha"
# done

# {sample8}
# With branching factor $b=8$, always select the best solution. With $\alpha$ restarts.
# NOTE: This is a really weird algorithm, maybe deserves a paper on its own! Inverted GRASP?
for alpha in $poolsizs9600; do
    name="sample8"
    bash ./solve_problem_set.sh search-"$name"_"$alpha" "$preset1cpu2gb" "$target" \
        $pref ./dc2/bin/dc -V -W -t1 -B8 -R"$alpha" best:1
done

# =================================================================
# State of the art
# =================================================================

# # {sdbsp10-mgesum}
# # Diverse search, with branching factor $b=8$ and selecting $k=10$ solutions using SDBS+, with $\alpha/10$ restarts.
# # NOTE: Gives almost the same results as rand8
# for alpha in $poolsizs300020; do
#     name="sdbsp10-mgesum"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B8 -t10 -R$((alpha/10)) sdbs+:10:mgesum
# done

# # {sdbsp20-mgesum}
# # Diverse search, with branching factor $b=8$ and selecting $k=20$ solutions using SDBS+, with $\alpha/20$ restarts.
# # NOTE: Gives almost the same results as rand8
# for alpha in $poolsizs300020; do
#     name="sdbsp20-mgesum"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B8 -t10 -R$((alpha/20)) sdbs+:20:mgesum
# done

# # {sdbsp20B0-mgesum}
# # Diverse search, with branching factor $b=0$ and selecting $k=20$ solutions using SDBS+, with $\alpha/20$ restarts.
# # NOTE: Gives almost the same results as rand8
# for alpha in $poolsizs300020; do
#     name="sdbsp20B0-mgesum"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B0 -t10 -R$((alpha/20)) sdbs+:20:mgesum
# done


# # {rand1}
# # Stochastic beam search, with branching factor $b=8$ and selecting the best solution and $1-\alpha$ at random.
# # NOTE: Gives almost the same results as rand8
# for alpha in $poolsizs9600; do
#     name="rand1"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B8 -t10 rand1:"$alpha"
# done

# # {rank1}
# # Stochastic beam search, with branching factor $b=8$ and selecting the best solution and $1-\alpha$ at random with prob. proportional to 1/ranking.
# # NOTE: Gives almost the same results as rand8
# for alpha in $poolsizs9600; do
#     name="rank1"
#     bash ./solve_problem_set.sh search-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $pref ./dc2/bin/dc -V -W -B8 -t10 rank1:"$alpha"
# done

for alpha in $poolsizs400; do
    name="sdbsp-mgesum-pr-best"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $prefix ./dc2/bin/dc -V -W -M -B8 -t10 sdbs+:"$alpha":mgesum _best:"$alpha"
done

for alpha in $poolsizs200; do
    name="sdbsp-mgesum-pr-best2"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $prefix ./dc2/bin/dc -V -W -M -B8 -t10 sdbs+:"$alpha":mgesum _best:"$((2*alpha))"
done

# # NOTE: recreative experiment
# for alpha in $poolsizs400; do
#     name="sdbs-mgesum-pr-best"
#     bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $prefix ./dc2/bin/dc -V -W -M -B8 -t10 sdbs:"$alpha":mgesum _best:"$alpha"
# done

# # NOTE: recreative experiment
# for alpha in $poolsizs200; do
#     name="sdbs-mgesum-pr-best2"
#     bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $prefix ./dc2/bin/dc -V -W -M -B8 -t10 sdbs:"$alpha":mgesum _best:"$((2*alpha))"
# done

# NOTE: slightly worst than sdbsp-mgesum-pr-best
# for alpha in $poolsizs400; do
#     name="sdbsp-pcd-pr-best"
#     bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $prefix ./dc2/bin/dc -V -W -M -B8 -t10 sdbs+:"$alpha":pcd _best:"$alpha"
# done

# NOTE: slightly worst than sdbsp-mgesum-pr-best2
# for alpha in $poolsizs200; do
#     name="sdbsp-pcd-pr-best2"
#     bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
#         $prefix ./dc2/bin/dc -V -W -M -B8 -t10 sdbs+:"$alpha":pcd _best:"$((2*alpha))"
# done


for alpha in $poolsizs400; do
    name="rand8-pr-best"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $prefix ./dc2/bin/dc -V -W -M -B8 -t10 rand:"$alpha" _best:"$alpha"
done

for alpha in $poolsizs200; do
    name="rand8-pr-best2"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $prefix ./dc2/bin/dc -V -W -M -B8 -t10 rand:"$alpha" _best:"$((2*alpha))"
done

for alpha in $poolsizs400; do
    name="best8-pr-best"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $prefix ./dc2/bin/dc -V -W -M -B8 -t10 best:"$alpha" _best:"$alpha"
done

for alpha in $poolsizs200; do
    name="best8-pr-best2"
    bash ./solve_problem_set.sh dc-"$name"_"$alpha" "$presetfullslim10cpu22gb" "$target" \
        $prefix ./dc2/bin/dc -V -W -M -B8 -t10 best:"$alpha" _best:"$((2*alpha))"
done

# Run Karapetyan's best 3 component CMCS result
# NOTE: must be run several times because some times it doesn't print output!
params3comp="5 10 15 20 25 30 40 50 75 100 150 200 250 300 400 500 600 700 800 1000" #1000 is 100%, minimum is 25
for alpha in $params3comp; do
    name="3comp"
    bash ./solve_problem_set.sh cmcs-"$name"_"$alpha" "$preset1cpu2gb" "$target" \
        $prefix "bash -e ./solve_karapetyan.sh" "$alpha"
done

# Solve using POPSTAR S
popstarSRP="7-16 10-32 12-48 14-64 16-80 19-112 20-128 21-144 22-160 32-320 39-480 45-640 50-800 55-960 59-1120 63-1280 67-1440 71-1600"
for pair in $popstarSRP; do
    poolsize=$(echo "$pair" | cut -d'-' -f1)
    restarts=$(echo "$pair" | cut -d'-' -f2)
    bash ./solve_problem_set.sh popstar-S_"$poolsize"_"$restarts"r "$preset1cpu2gb" "$target" \
        $prefix "bash -xe ./solve_popstar.sh" -graspit $restarts -elite $poolsize
done


# # Solve using POPSTAR L
# popstarLRP="6-20 10-32 13-42 16-50 18-57 22-70 24-76 25-81 27-86 40-128 50-160 58-187 66-211 73-232 79-252 85-271 90-288 95-304"
# for pair in $popstarLRP; do
#     poolsize=$(echo "$pair" | cut -d'-' -f1)
#     restarts=$(echo "$pair" | cut -d'-' -f2)
#     bash ./solve_problem_set.sh popstar-L_"$poolsize"_"$restarts"r "$preset1cpu2gb" "$target" \
#         $prefix "bash -xe ./solve_popstar.sh" -graspit $restarts -elite $poolsize
# done
