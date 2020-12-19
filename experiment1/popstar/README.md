# Index

**NOTE**: The following text was extracted from: http://mauricio.resende.info/popstar/index.html

POPSTAR is a tool developed by Mauricio Resende and Renato Werneck at AT&T Labs Research for solving instances of the p-median and uncapacitated facility location problems. It implements the algorithms described in the following papers:

    M. G. C. Resende and R. F. Werneck, A hybrid heuristic for the p-median problem, Technical Report TD-5NWRCR, AT&T Labs Research, 2003.  Published in Journal of Heuristics, vol. 10,  pp. 59-88, 2004.

    M. G. C. Resende and R. F. Werneck, A fast swap-based local search procedure for location problems, Technical Report TD-5R3KBH, AT&T Labs Research, 2003. Published in Annals of Operations Research, vol. 150, pp. 205-230, 2007.

    M. G. C. Resende and R. F. Werneck, A hybrid multistart heuristic for the uncapacitated facility location problem, Technical Report TD-5RELRR, AT&T Labs Research, 2003. Published in European Journal of Operational Research, vol. 174, pp. 54-68, 2006.

Please see the articles for details on  the algorithms.

The software can be used freely for research purposes but may require a license for commercial use.

Please send any questions, comments, suggestions, complaints, etc. about this document or the program itself to the authors at rwerneck@cs.princeton.edu or mresende AT gmail DOT com.

Copyright (C) 2006 AT&T.

# Changes

In order to compile POPSTAR it was needed to change the line `src/popstar.cpp:459:53` from
```
PMSearch::localSearch(s1, true, true, false, false); // local search
```
to
```
PMSearch::localSearch(s1, true, true, NULL, false); //local search
```
(thanks to Stuart Rogers for the tip).

