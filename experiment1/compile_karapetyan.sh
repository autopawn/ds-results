#! /bin/bash -xe

# This script's directory
cdir=$(dirname "$(readlink -f "$0")")

# Load mono module
ml Mono/6.8.0.96 || true

# Compile project
cd "$cdir"/karapetyan
xbuild
cd -





