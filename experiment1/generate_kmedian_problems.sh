#! /bin/bash -xe

# This script's directory
cdir=$(dirname "$(readlink -f "$0")")

# Number of problems
nproblems=100

# Compile instance generators
g++ -g "$cdir"/kmedian/kmgen.c -o "$cdir"/kmedian/kmgen


# ------------------ custom problems ------------------
target="../problems_kmedian_custom"

# Delete current folder
rm -rf "$target" || true
mkdir -p "$target"

# Problem size
size="2000"
denom="10" # opening cost is sqrt(size)/denom
metric=1

# Generate instances
for ii in $(seq $nproblems); do
    ii0=$(printf "%04d" "$ii")
    "$cdir"/kmedian/kmgen "$size" "$denom" "$metric" "$target"/"$size"_"$denom"-"$ii0"
done

