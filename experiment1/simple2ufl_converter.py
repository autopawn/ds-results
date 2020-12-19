import sys


"""
Script to transform instances in simple format
(https://resources.mpi-inf.mpg.de/departments/d1/projects/benchmarks/UflLib/data-format.html)
to .ufl format compatible with POPSTAR
usage: python3 simple2ufl_converter.py <input_file> <output_file>
"""

floati = lambda x: int(x) if float(x)==int(x) else float(x)

assert(len(sys.argv)==3)

input_fname = sys.argv[1]
output_fname = sys.argv[2]

# Read input file
with open(input_fname,"r") as file:

    # Read lines
    lines = file.readlines()

    # Check that the header is correct
    header = lines[0]
    assert(header.split(":")[0]=="FILE")

    # Get the number of facilities and clients
    n,m,s_max = [int(x) for x in lines[1].split(" ")]

    # Read lines to get cost_matrix and fcosts
    fcosts = []
    cost_matrix = []
    for lin in lines[2:]:
        lin = lin.strip().split(" ")
        fid = int(lin[0])
        assert(fid==len(cost_matrix)+1)
        fcost = floati(lin[1])
        ccosts = [floati(x) for x in lin[2:]]
        assert(len(ccosts)==m)
        fcosts.append(fcost)
        cost_matrix.append(ccosts)

    assert(len(cost_matrix)==n)

# Write output file
with open(output_fname,"w") as file:
    file.write("p %d %d\n"%(m,n))
    # Add distances
    for u in range(m):
        for f in range(n):
            file.write("a %d %d %s\n"%(u+1,f+1,str(cost_matrix[f][u])))
    # Add facilty costs
    for f in range(n):
        file.write("f %d %s\n"%(f+1,str(fcosts[f])))














