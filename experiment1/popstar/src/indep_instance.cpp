/**************************************************************
 * 
 * class PMIndepInstance
 * 
 **************************************************************/

#include "indep_instance.h"
#include "basics.h"
//#include "distance.h"
//#include "bossa_timer.h"
//#include "graph_instance.h"
#include <string.h>

/*******************************************************
 * 
 * readDimacs: reads file in Dimacs format and executes
 *             Floyd's algorithm to compute the full
 *             distance matrix
 *
 *******************************************************/

void PMIndepInstance::readDimacs (FILE *input) {
	int v1, v2;
	int e_count = 0;          //number of edges read
	bool size_read = false;   //flag
	const int LINESIZE = 256; 
	char buffer [LINESIZE+1]; 
	int i;

	nvertices = 0;
	nedges    = 0; 

	while (fgets (buffer, LINESIZE, input)) {
		switch (buffer[0]) {
			case 'p':
				if (size_read) fatal ("readDimacs", "duplicate header");
				if (sscanf (buffer, "p %d %d", &nvertices, &nedges) != 2) 
					fatal ("readDimacs", "syntax error in 'p' line");
				size_read = true;		
				edgelist = new Pair [nedges+1];
				templist = new IntDouble [nvertices+1];
				degree = new int [nvertices+1];
				for (i=1; i<=nvertices; i++) degree[i] = 0;
				break;
				
			case 'e':
				if (!size_read) fatal ("readDimacs", "header missing");
				if (sscanf(buffer,"e %d %d", &v1, &v2)==2) {	
					e_count++;
					edgelist[e_count].v1 = v1;
					edgelist[e_count].v2 = v2;
					degree[v1]++;
					degree[v2]++;
				} else fatal ("readDimacs", "syntax error in 'e' line");
		}
	}	
	fprintf (stderr, "Read graph with %d vertices and %d edges.\n", nvertices, nedges);

	if (nedges!=e_count) fatal ("readDimacs", "invalid number of edges");

	initNeighbors();
}
