#! /bin/bash -xe

# Load mono module in the cluster
if [[ -v SLURM_ARRAY_TASK_ID ]]; then
	ml Mono/6.8.0.96 || true
fi

multipliers="
250a 40
250b 40
250c 40
500a 100
500b 50
500c 40
750a 200
750b 100
750c 40
3000d 200
3000e 200
"

if (($# != 3 )); then
    echo "Illegal number of parameters"
    exit
fi

nothers=$(($#-2))

input="${@: -2:1}"
target="${@: -1}"
timebudget="${@:1:$nothers}"

problemtype=$(basename "$input" | cut -d'_' -f2 | cut -d'-' -f1)
timemult=$(echo "$multipliers" | grep $problemtype | cut -d' ' -f2)
timebudget=$(python -c "print($timemult*$timebudget/1000.0)")

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
