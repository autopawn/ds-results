/**************************************************************
 *
 * PMDistance Oracle: implements a 'distance oracle', a kind
 * of cache that keeps, for each vertex, a list of the closest
 * elements
 * 
 **************************************************************/

#ifndef DISTANCE_H
#define DISTANCE_H

#include <stdlib.h>
#include "oracle_list.h"
#include "basics.h"

class PMInstance;

class PMDistanceOracle {
	private:
		int m;            //number of potential facilities
		int n;            //number of customers
		int p;            //number of facilities in the solution
		PMInstance *inst; //instance to which this oracle refers
		int **list;       //list[i] is the list of elements associated with i
		
		//int *templist;    //buffer area; getCloser returns a pointer to this
		IntDouble *templist;
		int **predof;   //predof[i]=j means that i must be inserted right after j
		                //if a list is sorted by label
		double *distance; //temporary array used to avoid repeated calls to getDist();

		OracleList *olist;

		void fatal (const char *func, const char *msg); //error messages
		void checkList(int i);                          //debug: check if the list makes sense
		double init_time;                               //initialization time (how long it took to run the constructor)
		inline int getSize(int i) {return list[i][0];}  //number of the elements in the i-th list
		double getMaxValue(int i);                      //maximum value represented by the i-th list
		void initArrays();		                        //create all the lists
		void deleteArrays();

	public:
		static double cache_factor; //lists will have size equal to (m/p) times this
		static double min_oracle_time; //minimum time spent initializing the oracle: 
		                               //make greater than zero if you want precision in timing

		PMDistanceOracle (PMInstance *_inst);    //constructor
		~PMDistanceOracle ();                    //destructor
		double getInitTime() {return init_time;} //how long did it take to initialize?

		// return an array with a list of all elements whose distance to i 
		// is strictly smaller than v; the list is sorted by LABEL (that's
		// relevant if a sparse matrix is used)
		IntDouble *getCloser (int i, double v);       
};


#endif
