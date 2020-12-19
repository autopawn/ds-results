#! /bin/bash -xe

# This script's directory
cdir=$(dirname "$(readlink -f "$0")")

# Number of problems
nproblems=60

# Compile instance generators
gcc -g "$cdir"/koerkel_ghosh/kggensym.c -o "$cdir"/koerkel_ghosh/genprob_sym
gcc -g "$cdir"/koerkel_ghosh/kggenasym.c -o "$cdir"/koerkel_ghosh/genprob_asym


# ------------------ custom problems ------------------
target="../problems_kg_custom"

# Delete current folder
rm -rf "$target" || true
mkdir -p "$target"

# Problem sizes
sizes="250 500 750"

# Generate instances
for mode in sym asym; do
    for size in $sizes; do
        # Generate class a,b, and c problems.
        "$cdir"/koerkel_ghosh/genprob_$mode $size $size   100   200 1000 2000 "$target"/"$mode"_"$size"a- $nproblems
        "$cdir"/koerkel_ghosh/genprob_$mode $size $size  1000  2000 1000 2000 "$target"/"$mode"_"$size"b- $nproblems
        "$cdir"/koerkel_ghosh/genprob_$mode $size $size 10000 20000 1000 2000 "$target"/"$mode"_"$size"c- $nproblems
        #
    done
done

# ------------------ 3000d problems ------------------
target="../problems_kg_custom_d"

# Delete current folder
rm -rf "$target" || true
mkdir -p "$target"

for mode in sym asym; do
    size=3000
    "$cdir"/koerkel_ghosh/genprob_$mode $size $size 40000 80000 1000 2000 "$target"/"$mode"_"$size"d- $nproblems
done

