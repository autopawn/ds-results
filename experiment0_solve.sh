#!/bin/bash -xe

# == Small experiments to test performance ==

# Load variables
source ./variables.sh


# prefix="echo"
prefix=

params="46-92-5"

# ./dc2/bin/dc -r42 -V -W -M -B0 -t1 -R5 sdbs+:46:pcd _sdbs+:92:pcd

# Run representative dc2 version
for pms in $params; do
    name=testt1
    poolz1=$(echo "$pms" | cut -d'-' -f1)
    poolz2=$(echo "$pms" | cut -d'-' -f2)
    restrt=$(echo "$pms" | cut -d'-' -f3)

    bash ./solve_problem_set.sh test-"$name"_"$poolz1" "$preset1cpu2gb" problems_test \
        $prefix ./dc2/bin/dc -r42 -V -W -M -B0 -t1 -R"$restrt" sdbs+:"$poolz1":pcd _sdbs+:"$poolz2":pcd
done

# Run representative dc2 version
for pms in $params; do
    name=testt20
    poolz1=$(echo "$pms" | cut -d'-' -f1)
    poolz2=$(echo "$pms" | cut -d'-' -f2)
    restrt=$(echo "$pms" | cut -d'-' -f3)

    bash ./solve_problem_set.sh test-"$name"_"$poolz1" "$presetfullslim20cpu44gb" problems_test \
        $prefix ./dc2/bin/dc -r42 -V -W -M -B0 -t20 -R"$restrt" sdbs+:"$poolz1":pcd _sdbs+:"$poolz2":pcd
done

