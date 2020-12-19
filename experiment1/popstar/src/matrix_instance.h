/*---------------------------------------------------------------
 | class PMInstance: 
 |   represents the distance matrix for the problem and the list
 |   of potential facilities. Instances can be read from an input
 |   file.                   
 |
 | author: Renato Werneck (rwerneck@princeton.edu)
 | log: 
 |      May 29, 2002: file created
 *---------------------------------------------------------------*/

#ifndef matrix_instance_h
#define matrix_instance_h

#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include "basics.h"
#include "distance.h"
#include "instance.h"

class PMMatrixInstance:public PMInstance {
	private:
		double oracle_time;
		void reset();
		void fatal (const char *func, const char *msg);
		PMDistanceOracle *oracle;

	protected:
		void resetDistances();
		void initOracle() {oracle = new PMDistanceOracle (this);}
		double **d; //distance matrix
		double *fc; //facility cost
		int n; //number of nodes (users)
		int p; //number of facilities we are aiming for
		int m; //number of potential facilities

		inline void checkFacility(int f) {
			if ((f<1) || (f>m)) {
				fprintf (stderr, "ERROR: facility %d is out of range.\n", f);
				exit(1);
			}
		}

		inline void checkUser (int u) {
			if (u<1 || u>m) {
				fprintf (stderr, "ERROR: user %d is out of range.\n", u);
				exit(-1);
			}
		}

	public:
		PMMatrixInstance();		
		PMMatrixInstance (PMInstance *original, int *of, int *oc);
		~PMMatrixInstance();		

		virtual double getOracleTime() {return oracle->getInitTime();}
		virtual int getMetric() {return MATRIX;}
		virtual IntDouble *getCloser (int i, double v) {return oracle->getCloser(i,v);}
		void readPMM (FILE *file, int _p=0);
		void readUFL (FILE *file);
		void printMatrix (FILE *file);

		virtual double getFacDist (int f, int g) {
			if (f>m || g>m) fatal ("getFacDist", "facility number out of range");
			return d[f][g]; //assumes facilities and users are the same thing
		}
		
		virtual double getDist (int u, int f) {
			if (u>n) fatal ("getDist", "customer number out of range");
			if (f>m) fatal ("getDist", "facility number out of range");
			return d[u][f];
		}

		/*
		virtual void getDistances (int i, IntDouble *array) {
			int m = getM();
			for (int f=m; f>0; f--) { //build a list of distances to facilities
				array[f].id = f;
				array[f].value = d[i][f]; //WARNING: THIS COULD BE MORE EFFICIENT
			}
		}
		*/

		virtual void getDistances (int i, double *array) {
			int m = getM();
			for (int f=m; f>0; f--) {
				array[f] = d[i][f];
			}
		}


		virtual double getFacCost(int f) {return (fc==NULL) ? 0 : fc[f];}
		virtual int  getM() {return m;} 
		virtual int  getN() {return n;}
		virtual int  getP() {return p;}
		virtual void setP (int _p) {p = _p;}
		virtual bool fixedP() {return (fc==NULL);}
};

#endif
