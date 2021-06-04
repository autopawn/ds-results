

# Generate tiny problems
for size in 10 20 30; do
    ./experiment1/koerkel_ghosh/genprob_asym "$size" "$size"    100    200  1000 2000 problems_kg_tiny/asym_"$size"a- 10
    ./experiment1/koerkel_ghosh/genprob_asym "$size" "$size"   1000   2000  1000 2000 problems_kg_tiny/asym_"$size"b- 10
    ./experiment1/koerkel_ghosh/genprob_asym "$size" "$size"  10000  20000  1000 2000 problems_kg_tiny/asym_"$size"c- 10
done


# cd ~/dc2
# make && ./bin/dc -d -t1 -W -M -B8 sdbs+:4:mgesum _best:8 ../ds-results/problems_kg_tiny/asym_10b-3 ~/Downloads/out | grep '##'


# ./bin/dc -d -t1 -W -M -B3 sdbs+:3:mgesum _best:8 ../ds-results/problems_kg_tiny/asym_10b-1 ~/Downloads/out | grep '##'
