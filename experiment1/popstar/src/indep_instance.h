/*-----------------------------------------------------------------------------------
 |
 | class PMIndepInstance: represents instances of the independent set problem
 |
 | The reduction works as follows. 
 | - Each edge in the input graph is interpreted as a pair of vertices. 
 | - Each such pair will be a user. 
 | - Each original vertex will be a facility (opening cost is one). 
 | - The distance from a facility v to any pair that contains it is zero; all other
 |   distances are infinite.
 | - Opening a facility v means that v does *not* belong to the indep set.
 | - Note that a solution will have finite value if and only if each edge has at 
 |   least one vertex that does not belong to the indep set: correct by definition.
 |
 *----------------------------------------------------------------------------------*/

#ifndef INDEP_INSTANCE_H
#define INDEP_INSTANCE_H

#include <stdlib.h>
#include <stdio.h>
#include <math.h>

#include "instance.h"

class PMIndepInstance:public PMInstance {
	private:

		typedef struct {
			int v1;
			int v2;
		} Pair;
	
		virtual void fatal (const char *func, const char *msg) {
			fprintf (stderr, "PMIndepInstance::%s: %s.\n", func, msg);
			exit(-1);
		}

		IntDouble *templist;

		int nvertices;  //number of vertices in the original graph
		int nedges;     //number of edges in the original graphs
		Pair *edgelist; //list of edges
		int *degree;
		int **neighbors;

		/*------------------------------------
		 | build individual list of neighbors
		 *-----------------------------------*/
		void initNeighbors() {
			int *count = new int [nvertices+1];
			neighbors = new int* [nvertices+1];
			int v, e;
			
			for (v=1; v<=nvertices; v++) {
				count[v] = 0;
				neighbors[v] = new int[degree[v]];
			}
			for (e=1; e<=nedges; e++) {	
				int v1 = edgelist[e].v1;
				int v2 = edgelist[e].v2;
				neighbors[v1][count[v1]++] = v2;
				neighbors[v2][count[v2]++] = v1;
			}

			delete [] count;
		}
	protected:
		virtual int getMetric() {return INDEP;}

	public:
		inline int *getNeighborList (int v) {return neighbors[v];}
		inline int getDegree (int v) {return degree[v];}

		PMIndepInstance() {
			nvertices = nedges = 0;
			edgelist = NULL;
			templist = NULL;
			degree = NULL;
		}

		virtual ~PMIndepInstance() {
			if (edgelist) delete [] edgelist;
			if (templist) delete [] templist;
			if (degree) delete [] degree;
		}

		virtual void setP (int _p) {
			fprintf (stderr, "setP called for ufl instance, ignoring...\n");
		}


		/*-------------------------------------------------------------------
		 | given a user (edge) i, return a list of all facilities (vertices)
		 | whose distance to i is strictly less than v
		 *------------------------------------------------------------------*/
		virtual IntDouble *getCloser (int i, double v) {
			if (v < 0) {
				templist[0].id = 0;
			} else if (v < nvertices+1) {
				templist[0].id = 2;
				templist[1].id = edgelist[i].v1;
				templist[1].value = 0;
				templist[2].id = edgelist[i].v2;
				templist[1].value = 0;
			} else {
				templist[0].id = nvertices;
				for (int k=1; k<=nvertices; k++) {
					templist[k].id = k;
					templist[k].value = nvertices+1;
				}
				templist[edgelist[i].v1].id = edgelist[i].v1;
				templist[edgelist[i].v1].value = 0;
				templist[edgelist[i].v2].id = edgelist[i].v2;
				templist[edgelist[i].v2].value = 0;
				
			}
			//fprintf (stderr, ".");

			return templist;
		}

		virtual double getDist (int u, int f) {
			//int e = matrix[u][f]; //look for the edge label

			//u represents and edge, f represents a vertex
			//return zero if f is an endpoint of f, infinity otherwise
			if (edgelist[u].v1==f) return 0; 
			if (edgelist[u].v2==f) return 0;
			return (nvertices+1); //that's actually infinity
		}
		virtual int getM() {return nvertices;} //each vertex is a facility
		virtual int getN() {return nedges;}    //each edge is a customer
		virtual int getP() {return 0;}         //p is not defined --- that's a ufl instance

		inline int getNVertices() {return nvertices;}
		inline int getNEdges() {return nedges;}



		virtual double getOracleTime () {return 0;}

		virtual double getFacDist (int f, int g) {
			fatal ("getFacDist", "function not implemented");
			return 0.0;
		}
		
		virtual double getFacCost (int f) {
			return 1.0 + 0.1 * (double)(degree[f])/(double)(2*nedges + 1);
		}
		
		virtual bool fixedP() {return false;}

		void readDimacs (FILE *file);
};

#endif

