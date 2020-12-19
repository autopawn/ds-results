# ds-results

# Compiling dc2

Make sure that dc2 is compiled in order to execute the experiments.
```
cd dc2
make
cd ..
```

**NOTE**: This is the version on which these results were obtained. A more recent version can be found in the [dc2 repository](https://github.com/autopawn/dc2).

# Step 1: Generate KG problems

To generate new KG problems. **NOTE**: this can be skipped as the problems are provided on `problems_kg_custom/` and `problems_kg_custom_d/`

```
bash -xe experiment1/generate_kg_problems.sh
```

## Step 2: compile POPSTAR

```
bash -xe experiment1/compile_popstar.sh
```

## Step 3: compile CMCS

```
bash -xe experiment1/compile_karapetyan.sh
```

## Step 4: compute results for all algorithms

This takes a considerable amount of computational resources. It was run on the NLHPC cluster. Check this script contents to see the parameters.

```
bash -xe ./experiment1_searches_solve.sh
```

This will create the results in the `results` folder, move them.

```
mv results results-exp1
```

Same for 3000d experiments:

```
bash -xe ./experiment4_searches_solve.sh
```

This will create the results in the `results` folder, move them.

```
mv results results-exp4d
```

## Step 5: parse all results

Condense the results in their respective JSONs:

```
mkdir -p jsons
python3 result_parser.py results-exp1 jsons/output_2020-12-03_paperv1_plus_sdbs_pr.json
python3 result_parser.py results-exp4d jsons/output_2020-12-09_3000d.json
```

## Step 6: plot results

Plot the JSONs:

```
mkdir -p result-figs-1
python3 experiment1/plot_kg_groups.py problems_kg_custom jsons/output_2020-12-03_paperv1_plus_sdbs_pr.json result-figs-1

mkdir -p result-figs-4d
python3 experiment1/plot_kg_groups.py problems_kg_custom_d jsons/output_2020-12-09_3000d.json result-figs-4d
```
