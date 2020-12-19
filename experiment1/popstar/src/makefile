#
# source files
#

SOURCES = bossa_random.cpp cover_instance.cpp distance.cpp indep_instance.cpp\
          graph_instance.cpp geo_instance.cpp matrix_instance.cpp euclidean_instance.cpp \
	  search_tables.cpp solution.cpp search.cpp hybrid_instance.cpp bossa_timer.cpp \
          popstar.cpp path_relink.cpp constructive.cpp basics.cpp elite.cpp


#
# parameters for various compilers
#

GCC_NAME    = g++
GCC_FLAGS   = -Wall -O4 -g
GCC_LIBS    = -lm -L/usr/lib32/
GCC_DEFINES = -DBOSSA_RUSAGE
GCC_OBJECTS = $(SOURCES:.cpp=.o)
GCC_REMOVE  = 'rm' *.o

VCC_NAME    = cl
VCC_FLAGS   = /W3 /O2 /nologo
VCC_DEFINES = -DWIN32 -DNDEBUG -D_CONSOLE
VCC_LIBS    =
VCC_OBJECTS = $(SOURCES:.cpp=.obj)
VCC_REMOVE  = del *.obj

CC_NAME     = CC
CC_FLAGS    = -O3 -OPT:Olimit=6586
CC_DEFINES  = -DBOSSA_RUSAGE
CC_LIBS     = -lm -L/usr/lib32/mips3/
CC_OBJECTS  = $(SOURCES:.cpp=.o)
CC_REMOVE   = 'rm' *.o

#
# CHANGE THESE LINES TO USE YOUR FAVORITE COMPILER
#
CCC      = $(GCC_NAME)
FLAGS    = $(GCC_FLAGS)
LIBS     = $(GCC_LIBS)
DEFINES  = $(GCC_DEFINES)
OBJECTS  = $(GCC_OBJECTS)
REMOVE   = $(GCC_REMOVE)
INCLUDES = -I.

.SUFFIXES: .cpp

popstar: $(OBJECTS)
	$(CCC) $(FLAGS) $(DEFINES) $(INCLUDES) $(OBJECTS) $(LIBS) -o popstar

all: clean popstar

clean:
	$(REMOVE)
#	'rm' *.o

.cpp.o:
	$(CCC) $(DEFINES) $(FLAGS) -c $<

.cpp.obj:
	$(CCC) $(DEFINES) $(FLAGS) -c $<

