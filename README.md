- Comprobar qué versión de dc2 es más moderna.
- Observar sobre qué instancias reporta resultados Karapetyan.
- Crear archivo de tareas por lotes para eplicar experimentos sobre esas instancias con Karapetyan y POPSTAR.


# ds-results

**Review 1 branch**.

Results of DS applied to generated KG and k-median problems.

# Compiling dc2

Make sure that dc2 is compiled in order to execute the experiments.
```
cd dc2
make
cd ..
```

**NOTE**: This is the version on which these results were obtained. A more recent version can be found in the [dc2 repository](https://github.com/autopawn/dc2).

# Step 2: compile POPSTAR

```
bash -xe experiment1/compile_popstar.sh
```

# Step 3: compile CMCS

```
bash -xe experiment1/compile_karapetyan.sh
```

# Step 4.1: compute DS components performance for k-median

This solves the custom k-median problems for all DS variants shown in the Thesis.

```
bash -xe ./experimentT2_solve.sh
```

This will create the results in the `results` folder, move them.

```
mv results results-t2
```

# Step 5.2: compute rel. cost vs time curves for all algorithms

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

Same for kmedian_custom algorithms:

```
bash -xe ./experiment5_searches_solve.sh
```

This will create the results in the `results` folder, move them.

```
mv results results-exp5
```

# Step 5: parse all results

Condense the results in their respective JSONs:

```
mkdir -p jsons
python3 result_parser.py results-t2 jsons/results-t2.json
python3 result_parser.py results-exp1 jsons/results-exp1.json
python3 result_parser.py results-exp4d jsons/results-exp4d.json
python3 result_parser.py results-exp5 jsons/results-exp5.json
```

# Step 6: plot results

Plot the JSONs:

```
mkdir -p result-figs-t2
python3 experiment1/plot_kmedian_groups.py jsons/results-t2.json result-figs-t2

mkdir -p result-figs-1
python3 experiment1/plot_kg_groups.py problems_kg_custom jsons/results-exp1.json result-figs-1

mkdir -p result-figs-4d
python3 experiment1/plot_kg_groups.py problems_kg_custom_d jsons/results-exp4d.json result-figs-4d

mkdir -p result-figs-5
python3 experiment1/plot_kg_groups.py problems_kmedian_custom jsons/results-exp5.json result-figs-5
```
