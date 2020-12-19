/***********************************
 *
 * PMDistanceOracle: implementation
 *
 ***********************************/

#include "distance.h"
#include "basics.h"
#include "instance.h"
#include "bossa_timer.h"
#include "bossa_heap.h"
#include "instance.h"
#include "rfwsort.h"
#include <stdlib.h>
#include <stdio.h>
#include <math.h>

double PMDistanceOracle::cache_factor = 2;
double PMDistanceOracle::min_oracle_time = 0;

/*------------------------
 | standard error message
 *-----------------------*/

void PMDistanceOracle::fatal (const char *func, const char *msg) {
	fprintf (stderr, "PMDistanceOracle::%s: %s.\n", func, msg);
	exit (-1);
}


/*-------------
 | constructor
 *------------*/

PMDistanceOracle::PMDistanceOracle(PMInstance *_inst) {

	//for timing purposes, we might repeat this construction until we get it right
	double mintime = min_oracle_time;
	int repetitions = 0;
	double acc_time = 0;

	inst = _inst;
	p = inst->getP();
	m = inst->getM();
	n = inst->getN();
	printf("check {\n");
	fprintf (stderr, "Initializing oracle (n=%d, p=%d, m=%d)", n, p, m);
	BossaTimer t(true);
	printf("} check\n");
	for (;;) {
		repetitions ++;

		templist = new IntDouble [m+1]; //temporary list used by get closer
		distance = new double [m+1];    //temporary list of distances...
		list     = new int *[n+1];      //list[i] contains an array of facilities sorted by distance to i
		predof   = new int *[n+1];      //predof[i]: contains an array with the positions in which facilities must be inserted
		olist = new OracleList (m);     //linked list used in the reordering problem

		initArrays();

		acc_time = t.getTime();

		if (acc_time < mintime) {
			deleteArrays(); //we'll try again
		} else break; //good enough
	}
	fprintf (stderr, "\n");

	//init_time = t.getTime();
	fprintf (stdout, "oraclerep %d\n", repetitions);
	fprintf (stdout, "oraclemintime %f\n", mintime);
	init_time = acc_time / (double)repetitions;
}


/*------------------------------------------------
 | maximum distance in the list associated with i
 *-----------------------------------------------*/

double PMDistanceOracle::getMaxValue (int i) {
	int s = getSize(i);
	if (s < 1) {return - POPSTAR_INFINITY;} //empty list
	else {return inst->getDist(i,list[i][s]);}
}


/*--------------------------------------
 | Initialize the 'list' data structure
 *-------------------------------------*/

void PMDistanceOracle::initArrays() {
	int s; //size of each list

	if (inst->fixedP()) {
		if (cache_factor < 0) {s = m;}
		else {
			s = (int) ceil (PMDistanceOracle::cache_factor * (double)m/(double)p);
			if (s>m) s=m; //make sure we do not have everything...
		}
	} else {
		s = m;
	}

	double *darray = new double[m+1];
	IntDouble *array = new IntDouble[m+1];
	for (int i=1; i<=n; i++) {     //for every customer
		//sort all facilities by their distance to customer i
		/*
		for (int f=m; f>0; f--) { //build a list of distances to facilities
			array[f].id = f;
			array[f].value = inst->getDist(i,f); //WARNING: THIS COULD BE MORE EFFICIENT
		}*/
		if (s > 0) {
			//inst->getDistances(i,array);
			inst->getDistances(i,darray);
			for (int f=m; f>0; f--) {
				array[f].id = f;
				array[f].value = darray[f];
			}
		}


		//sort (&array[1], &array[m]); //WARNING: THIS COULD BE MORE EFFICIENT
		//fprintf (stderr, "s=%d, m=%d, cf=%lg, 1=%d, m=%d, s=%d\n", s, m, PMDistanceOracle::cache_factor, &array[1], &array[m], &array[s]);
		if (s > 0) partialSort (&array[1], &array[m], &array[s]);

		//WARNING! SLOW! SLOW!
		//if (!isSorted(&array[1],&array[s])) {
		//	fprintf (stderr, "Terminal failure!\n");
		//	exit(-1);
		//}


		//create individual lists of labels
		int *ilist = list[i] = new int [s+1];
		int *ipredof = predof[i] = new int [s+1];
		ilist[0] = s; //store the size of the list

		int p;

		//create individual lists of successors
		if (s>0) {
			olist->makeFull(); //build a list with everybody (sorted by label)
			for (p=m; p>s; p--) {olist->vanquish(array[p].id);} //keep just the first s
		}

		//initialize isucc and ilist simultaneously
		//this last step removes the ones we want, and saves in predof[i] the label of
		//the facility that precedes the i-th closest facility in the list at the moment
		for (p=s; p>0; p--) {ipredof[p] = olist->vanquish(ilist[p]=array[p].id);}
	}

	delete [] array;
	delete [] darray;
}


/*------------------------------------------------
 | verify whether the list associated with user
 | i makes sense (i.e., if it is actually sorted)
 *-----------------------------------------------*/

void PMDistanceOracle::checkList (int i) {
	int s = getSize(i);
	double prev = -1;
	for (int p=1; p<=s; p++) {
		int f = list[i][p];
		double current = inst->getDist(i,f);
		if (current<prev) {fprintf (stderr, "THERE'S SOMETHING WRONG WITH THE DISTANCE.\n");}
		prev = current;
	}
	fprintf (stderr, "OK ");
}


/*------------
 | destructor
 *-----------*/

void PMDistanceOracle::deleteArrays() {
	for (int i=1; i<=n; i++) {
		delete [] list[i];
		delete [] predof[i];
	}
	delete [] distance;
	delete [] predof;
	delete [] list;
	delete [] templist;
	delete olist;
}

PMDistanceOracle::~PMDistanceOracle() {
	deleteArrays();
}


/*-------------------------------------------------------------------------------
 | Build list of all facilities whose distance to user i is strictly less than v
 *------------------------------------------------------------------------------*/

IntDouble *PMDistanceOracle::getCloser (int i, double v) {
	templist[0].id = 0; //list initially empty
	if (v < getMaxValue(i) - EPSILON) { //if possible, use pre-computed list
		int *ilist = list[i];
		int *ipredof = predof[i];

		olist->makeEmpty(); //start with an empty list (constant time)
		int p;
		for (p=1; true; p++) {                 //for every position in the sorted list
			int f = ilist[p];                  //get a facility to insert in the list
			distance[f] = inst->getDist(i,f);  //get its distance to i
			//fprintf (stderr, "(%f,%f) ", distance[f], v);
			if (distance[f]>=v) break;         //if not big enough, give up
			olist->lazyInsert(f,ipredof[p]);   //insert f in the right position, right after ipredof[p]
		}
		olist->dump(templist); //fill templist with the proper id's (time proportional to size)
		for (p=templist[0].id; p>0; p--) {
			templist[p].value = distance[templist[p].id]; //get the corresponding distances
		}
		//fprintf (stderr, "@");
	} else { //pre-computed list not big enough:compute on-the-fly
		//fprintf (stderr, "#");
		/*
		for (int f=1; f<=m; f++) {
			if (inst->getDist(i,f)<v) {
				templist[++templist[0].id].id = f; //increment size, put f in last position
				templist[templist[0].id].value = inst->getDist(i,f); //also put the distance there
			}
		}*/

		//fprintf (stderr, "!");

		/*
		inst->getDistances(i, templist);
		int last = 0;
		for (int pos=1; pos<=m; pos++) {
			if (templist[pos].value < v) {
				templist[++last] = templist[pos];
			}
		}
		templist[0].id = last;
		*/
		inst->getDistances(i,distance); //get all distances
		int last = 0;
		for (int pos = 1; pos<=m; pos++) {
			if (distance[pos] < v) {
				last++;
				templist[last].id = pos;
				templist[last].value = distance[pos];
			}
		}
		templist[0].id = last;

	}
	return templist;
}
