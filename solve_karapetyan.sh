#! /bin/bash -xe

# Load mono module in the cluster
if [[ -v SLURM_ARRAY_TASK_ID ]]; then
	ml Mono/6.8.0.96 || true
fi

multipliers="
sym_250a 40
asym_250a 40
sym_250b 40
asym_250b 40
sym_250c 40
asym_250c 40
sym_500a 100
asym_500a 100
sym_500b 50
asym_500b 50
sym_500c 40
asym_500c 40
sym_750a 200
asym_750a 200
sym_750b 100
asym_750b 100
sym_750c 40
asym_750c 40
sym_3000d 200
asym_3000d 200
sym_3000e 200
asym_3000e 200
2000_10 200
"

if (($# != 3 )); then
    echo "Illegal number of parameters"
    exit
fi

nothers=$(($#-2))

input="${@: -2:1}"
target="${@: -1}"
timebudget="${@:1:$nothers}"

problemtype=$(basename "$input" | cut -d'-' -f1)
timemult=$(echo "$multipliers" | grep $problemtype | cut -d' ' -f2)
timebudget=$(python -c "print($timemult*$timebudget/1000.0)")

echo "problemtype: $problemtype"
echo "input: $input"
echo "timebudget: $timebudget"

mkdir -p tmp
export TMPDIR=$(mktemp -d --tmpdir=tmp)

tmptimefile=$(pwd)/$(mktemp --suffix=".txt")

cd experiment1/karapetyan/bin;
{ command time --format='Elapsed: %e' mono ./SPLP.exe "$timebudget" ../../../"$input" 1> ../../../"$target"; } 2> "$tmptimefile"
cd - > /dev/null

cat "$tmptimefile" >> "$target"
echo "Time: $timebudget" >> "$target"

rm "$tmptimefile"
