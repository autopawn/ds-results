/**************************************************
 *
 * class PMCoverInstance: instances for the maxcover
 *       problem
 *
 * author: Renato Werneck (rwerneck@princeton.edu)
 * log: 
 *      May 21, 2003: file created
 *      Jul 27, 2003: fixed reduction
 * 
 **************************************************/

/*
  Each pop corresponds to a facility.
  Each region corresponds to a user.
*/


#ifndef COVER_INSTANCE_H
#define COVER_INSTANCE_H

#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include "basics.h"
#include "distance.h"
#include "bossa_timer.h"
#include "matrix_instance.h"
#include "rfwsort.h"

class PMCoverInstance:public PMInstance {
	private:
		double *efactor;  //if we decide set s will cover element f, 
		double *sfactor;  //the cost will be sfactor[s] * efactor[e]
		
		double maxserve;  //max of sfactor[s]*efactor[e], for all valid pairs (e,s).

		int **covered;    //covered[u] = list of elements that cover u
		double *weight;   //weight[u] = weight of element u
		double *opencost; //opencost[f] = cost of opening f

		IntDouble *templist;

		double infinity;  //large enough to never be selected
		int special_facility; //facility that will cover uncovered sets
		int special_user;     //the user that is convered only by the special facility
		bool use_special; //use the special facility (otherwise leave with infinity)
		int p;            //number of sets to select
		int nsets;        //total number of sets
		int nelements;    //total number of elements

		//incidence array: that's big, quadradic
		//We could do away with it with a minor penalty in running time
		//For now, let's keep it
		bool **incidence;  //incidence[u][f] = true iff element u belongs to set f
		inline bool isCovered (int u, int f) {return incidence[u][f];}
		inline void makeCovered (int u, int f) {incidence [u][f] = true;}
		void resetIncidence() {
			incidence = new bool *[nelements+1];
			for (int i=1; i<=nelements; i++) {
				incidence[i] = new bool [nsets+1];
				for (int j=1; j<=nsets; j++) {
					incidence[i][j] = false;
				}
			}
			fprintf (stderr, "INCIDENCE MATRIX INITIALIZED...\n");
		}

		void deleteIncidence() {
			if (incidence) {
				for (int i=1; i<=nelements; i++) delete [] incidence[i];
				delete [] incidence;
			}
		}

	
		//checks if an element x belongs to a list a
		inline bool contains (int *a, int x) {
			for (int i=a[0]; i>0; i--) {if (a[i]==x) return true;}
			return false;
		}

		void fatal (const char *func, const char *msg) {
			fprintf (stderr, "PMCoverInstance::%s: %s.\n", func, msg);
			exit (-1);
		}
		
		PMDistanceOracle *oracle;

	protected:
		virtual int getMetric() {return COVER;}
		void initOracle() {
			oracle = new PMDistanceOracle(this);
		}

	public:
		//PMCoverInstance(PMInstance *original, int *of, int *oc);
		PMCoverInstance(bool _use_special);		

		void readCover (FILE *file, int _p);
		virtual ~PMCoverInstance();
		virtual IntDouble *getCloser (int i, double v);
		virtual double getFacDist (int f, int g) {
			fprintf (stderr, "WARNING: PMCoverInstance::getFacDisc(%d,%d) should not be called.\n", f, g);
			return 0.0;
		}
		
		virtual void setP (int _p) {
			if (p<=0 || p>nsets) {
				fprintf (stderr, "Setting facilities to %d.\n", p);
				//fatal ("setP", "invalid number of facilities");
				
			}
			p = _p;			
		};

		//warning: this function should be avoided --- it takes a really long time to run
		virtual double getDist (int u, int f) {
			//fprintf (stderr, "#");
			double d;
			if (f==special_facility) {
				if (!use_special) fatal ("getDist", "facility out of range");
				d = (u==special_user) ? 0 : weight[u];
			} 
			else {
				if (u==special_user) {
					d = infinity;
				} else {
					//fprintf (stderr, "(%d %d)\n", contains(covered[u],f), isCovered(u,f));
					//if (contains(covered[u],f)!=isCovered(u,f)) {exit(-1);}


					//d = (contains(covered[u],f)) ? efactor[u]*sfactor[f] : infinity;
					d = (isCovered(u,f)) ? efactor[u]*sfactor[f] : infinity;
				}
			}
			return d;
		}

		/*
		virtual void getDistances (int u, IntDouble *array) {
			//fprintf (stderr, "!");
			int m = getM();

			//first, set all values to inifinty
			for (int f=m; f>0; f--) { //build a list of distances to facilities
				array[f].id = f;
				//array[f].value = getDist(i,f); //WARNING: THIS COULD BE MORE EFFICIENT
				array[f].value = infinity;
			}

			//now update those that are not infinite
			int *c = covered[u];
			for (int i=c[0]; i>0; i--) {
				int f = c[i];
				array[f].value = efactor[u] * sfactor[f];
			}
		}*/

		virtual void getDistances (int u, double *array) {
			int m = getM();
			for (int f=m; f>0; f--) array[f] = infinity;

			int *c = covered[u];
			for (int i=c[0]; i>0; i--) {
				int f = c[i];
				array[f] = efactor[u] * sfactor[f];
			}
		}



		virtual double getFloydTime() {return 0.0;}
		virtual double getOracleTime() {
			return oracle->getInitTime();
		}

		//parameters as viewd from the outside world
		virtual int getM() {return nsets + (use_special ? 1 : 0);} 
		virtual int getN() {return nelements + (use_special ? 1 : 0);}
		virtual int getP() {return p + (use_special ? 1 : 0);} 

		virtual double getFacCost(int f) {return (opencost==NULL) ? 0 : opencost[f];}
		virtual bool fixedP() {
			return p>0;
		}

		virtual void outputParameters (FILE *file) {
			PMInstance::outputParameters (file);
			fprintf (file, "sets %d\n", nsets);
			fprintf (file, "elements %d\n", nelements);
			fprintf (file, "setgoal %d\n", p);
		}
};

#endif
