#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define MAXRUN 10

int MAXINS;
int M;
int N;
int F_LO;
int F_HI;
int C_LO;
int C_HI;

int main(int argc, char** argv)
{
int i, j, run;
int *F, **C;
FILE *fp;
char filename[200];
char run_no[20];
int instance;

	if (argc == 1)
	{
		printf("Usage: genprob m n f_lo f_hi c_lo c_hi prefix num_prob\t(All Integers).\n");
		exit(0);
	}
	M = atoi(argv[1]);
	N = atoi(argv[2]);
	F_LO = atoi(argv[3]);
	F_HI = atoi(argv[4]);
	C_LO = atoi(argv[5]);
	C_HI = atoi(argv[6]);

	MAXINS = atoi(argv[8]);
	strcpy(filename, argv[7]);
	printf("%d %s problems: Size m = %d n = %d, f = (%d, %d), c = (%d, %d)\n\n",MAXINS, filename, M, N, F_LO, F_HI, C_LO, C_HI);

	F = (int *)calloc(M, sizeof(int));
	C = (int **)calloc(M, sizeof(int *));
	for (i = 0; i < M; i++)
		C[i] = (int *)calloc(N, sizeof(int));
	srand(123456);
	for (instance = 1; instance <= MAXINS; instance++)
	{
		strcpy(filename, argv[7]);
		sprintf(run_no,"%d",instance);
		strcat(filename, run_no);

		printf("Building file %s ", filename);

		for (i = 0; i < M; i++)
		{
			F[i] = (int)(rand()*1.0/RAND_MAX * (F_HI - F_LO) + F_LO);

			for (j = 0; j < N; j++)
				C[i][j] = (int)(rand()*1.0/RAND_MAX * (C_HI - C_LO) + C_LO);
			//for (j = i; j < N; j++)
			//	C[i][j] = C[j][i] = (int)(rand()*1.0/RAND_MAX * (C_HI - C_LO) + C_LO);

		}

		fp = fopen(filename, "w");
		fprintf(fp, "FILE: %s\n",filename);
		fprintf(fp, "%d %d 0\n", M, N);
		for (i = 0; i < M; i++)
		{
			fprintf(fp, "%d %d ", i+1, F[i]);
			for (j = 0; j < N; j++)
				fprintf(fp, "%d ", C[i][j]);
			fprintf(fp, "\n");
		}
		fclose(fp);
		printf(". . . Done\n");
	}

	for (i = 0; i < M; i++) free(C[i]);
	free(C);
	free(F);

	return 0;
}
