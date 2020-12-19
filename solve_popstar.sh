#! /bin/bash -xe

if (($# < 2 )); then
    echo "Illegal number of parameters"
    exit
fi

nothers=$(($#-2))

input="${@: -2:1}"
target="${@: -1}"
options="${@:1:$nothers}"

mkdir -p tmp
export TMPDIR=$(mktemp -d --tmpdir=tmp)

tmpfile=$(mktemp --suffix=".ufl")
python experiment1/simple2ufl_converter.py "$input" "$tmpfile"

experiment1/popstar/src/popstar "$tmpfile" $options > "$target"

rm "$tmpfile"
