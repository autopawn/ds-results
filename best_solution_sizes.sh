#!/bin/bash -xe

# This script prints the sizes of the solutions that appear as best-known in the UflLib benchmark.

target="problems_kg_original"

for foo in $(ls $target | grep '.bub\|.opt'); do
    nfacs=$(python -c "print(len(set(\"$(cat $target/$foo)\".split(\" \"))))")
    echo $foo $nfacs
done
