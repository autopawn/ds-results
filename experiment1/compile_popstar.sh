#! /bin/bash -xe

# This script's directory
cdir=$(dirname "$(readlink -f "$0")")

cd "$cdir"/popstar/src
make
cd -
