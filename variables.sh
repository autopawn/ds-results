#!/bin/bash

smallprobgroups="
pmedian-ORLIB-normal
splp-BildeKrarup
splp-Chess
splp-CLSC
splp-Euclid
splp-Fpp11
splp-Fpp17
splp-GalvaoRaggi
splp-GapA
splp-GapB
splp-KoerkelGhosh-asym
splp-KoerkelGhosh-sym
splp-M
splp-ORLIB-cap
splp-ORLIB-uncap
splp-PCodes
splp-Uniform"

bigprobgroups="
pmedian-ORLIB-large
splp-kmedian"


allprobgroups=$bigprobgroups"
    "$smallprobgroups

kgprobgroups="
splp-KoerkelGhosh-asym
splp-KoerkelGhosh-sym"

# KG problems that took less than 100 seconds with f3_0_rand_sdcep_mgesum_1000
le100kgproblems="
splp-KoerkelGhosh-asym/ga250c-1
splp-KoerkelGhosh-sym/gs250c-2
splp-KoerkelGhosh-asym/ga250c-5
splp-KoerkelGhosh-sym/gs250c-1
splp-KoerkelGhosh-sym/gs250c-4
splp-KoerkelGhosh-sym/gs250c-3
splp-KoerkelGhosh-asym/ga250c-3
splp-KoerkelGhosh-asym/ga250c-2
splp-KoerkelGhosh-sym/gs250c-5
splp-KoerkelGhosh-asym/ga250c-4
splp-KoerkelGhosh-asym/ga250b-5
splp-KoerkelGhosh-sym/gs250b-1
splp-KoerkelGhosh-sym/gs250b-3
splp-KoerkelGhosh-asym/ga250b-1
splp-KoerkelGhosh-asym/ga250b-3
splp-KoerkelGhosh-asym/ga250b-4
splp-KoerkelGhosh-asym/ga250b-2
splp-KoerkelGhosh-sym/gs250b-2
splp-KoerkelGhosh-sym/gs250b-5
splp-KoerkelGhosh-sym/gs250b-4
splp-KoerkelGhosh-asym/ga500c-5
splp-KoerkelGhosh-asym/ga500c-1
splp-KoerkelGhosh-sym/gs500c-2
splp-KoerkelGhosh-sym/gs500c-5
splp-KoerkelGhosh-sym/gs500c-1
splp-KoerkelGhosh-sym/gs500c-3
splp-KoerkelGhosh-asym/ga500c-2
splp-KoerkelGhosh-sym/gs500c-4
splp-KoerkelGhosh-asym/ga500c-4
splp-KoerkelGhosh-asym/ga500c-3
"

le500kgproblems="
splp-KoerkelGhosh-sym/gs750c-5
splp-KoerkelGhosh-asym/ga750c-5
splp-KoerkelGhosh-sym/gs750c-4
splp-KoerkelGhosh-asym/ga750c-1
splp-KoerkelGhosh-asym/ga750c-2
splp-KoerkelGhosh-asym/ga750c-3
splp-KoerkelGhosh-sym/gs750c-3
splp-KoerkelGhosh-sym/gs750c-2
splp-KoerkelGhosh-sym/gs750c-1
splp-KoerkelGhosh-asym/ga750c-4
splp-KoerkelGhosh-asym/ga500b-1
splp-KoerkelGhosh-sym/gs500b-5
splp-KoerkelGhosh-sym/gs500b-1
splp-KoerkelGhosh-sym/gs500b-4
splp-KoerkelGhosh-asym/ga500b-5
splp-KoerkelGhosh-asym/ga500b-3
splp-KoerkelGhosh-asym/ga500b-4
splp-KoerkelGhosh-sym/gs500b-2
splp-KoerkelGhosh-asym/ga500b-2
splp-KoerkelGhosh-sym/gs500b-3
"

le4200kgproblems="
splp-KoerkelGhosh-sym/gs250a-3
splp-KoerkelGhosh-sym/gs250a-2
splp-KoerkelGhosh-asym/ga250a-1
splp-KoerkelGhosh-asym/ga250a-3
splp-KoerkelGhosh-asym/ga250a-5
splp-KoerkelGhosh-asym/ga250a-4
splp-KoerkelGhosh-sym/gs250a-1
splp-KoerkelGhosh-sym/gs250a-5
splp-KoerkelGhosh-asym/ga250a-2
splp-KoerkelGhosh-sym/gs250a-4
splp-KoerkelGhosh-sym/gs750b-1
splp-KoerkelGhosh-sym/gs750b-3
splp-KoerkelGhosh-sym/gs750b-5
splp-KoerkelGhosh-asym/ga750b-4
splp-KoerkelGhosh-sym/gs750b-4
splp-KoerkelGhosh-asym/ga750b-2
splp-KoerkelGhosh-asym/ga750b-3
splp-KoerkelGhosh-sym/gs750b-2
splp-KoerkelGhosh-asym/ga750b-5
splp-KoerkelGhosh-asym/ga750b-1
splp-KoerkelGhosh-asym/ga500a-1
splp-KoerkelGhosh-sym/gs500a-1
splp-KoerkelGhosh-sym/gs500a-5
splp-KoerkelGhosh-asym/ga500a-2
splp-KoerkelGhosh-asym/ga500a-3
splp-KoerkelGhosh-sym/gs500a-4
splp-KoerkelGhosh-sym/gs500a-2
splp-KoerkelGhosh-asym/ga500a-5
splp-KoerkelGhosh-asym/ga500a-4
splp-KoerkelGhosh-sym/gs500a-3
"

le12700kgproblems="
splp-KoerkelGhosh-asym/ga750a-5
splp-KoerkelGhosh-sym/gs750a-5
splp-KoerkelGhosh-sym/gs750a-2
splp-KoerkelGhosh-asym/ga750a-3
splp-KoerkelGhosh-sym/gs750a-3
splp-KoerkelGhosh-asym/ga750a-1
splp-KoerkelGhosh-sym/gs750a-1
splp-KoerkelGhosh-asym/ga750a-4
splp-KoerkelGhosh-asym/ga750a-2
splp-KoerkelGhosh-sym/gs750a-4
"

# These sbatch argument presents are intended for LEFTRARU's slurm
# "general" has 44 cores and 192GB per node.
# "slims" has 20 cores and 48 GB per node.

#preset1cpu6gb="--partition=general --cpus-per-task=1 --mem-per-cpu=6G"
preset1cpu2gb="--partition=slims --cpus-per-task=1 --mem-per-cpu=2G"
preset2cpu4gb="--partition=slims --cpus-per-task=2 --mem-per-cpu=4400M"
preset2cpu1gb="--partition=slims --cpus-per-task=2 --mem-per-cpu=1G"

preset4cpu8gb="--partition=slims --cpus-per-task=4 --mem-per-cpu=2G"
preset4cpu3gb="--partition=slims --cpus-per-task=4 --mem-per-cpu=768M"

preset4cpu16gb="--partition=general --cpus-per-task=4 --mem-per-cpu=4G"
preset8cpu36gb="--partition=general --cpus-per-task=8 --mem-per-cpu=4G"

presetfullslim10cpu22gb="--partition=slims --cpus-per-task=10 --mem-per-cpu=2200M"
presetfullslim20cpu44gb="--partition=slims --cpus-per-task=20 --mem-per-cpu=2200M"

