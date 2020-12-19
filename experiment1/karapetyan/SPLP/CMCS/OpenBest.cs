using System;
using System.Diagnostics;

namespace SPLP.CMCS
{
    class OpenBest : CmcsComponent
    {
        public override void Apply(SplpSolution solution, Random random)
        {
            int[] deltas = new int[solution.Instance.NumberOfLocations];

            for (int client = solution.Instance.NumberOfClients - 1; client >= 0; client--)
            {
                var firstLocation = solution.GetFirstClosestLocation(client);
                int curCost = solution.Instance.GetClientCost(firstLocation, client);
                for (int index = 0; ; index++)
                {
                    int location = solution.Instance.GetLocationByClosenessIndex(client, index);
					if (location == firstLocation)
						break;

                    int cost = solution.Instance.GetClientCost(location, client);
                    deltas[location] += cost - curCost;
                }
            }

            int min = 0;
            int best = -1;
            for (int location = deltas.Length - 1; location >= 0; location--)
            {
                if (solution.IsOpened(location))
                    continue;

                var fullDelta = deltas[location] + solution.Instance.GetLocationCost(location);
                if (fullDelta < min)
                {
                    min = fullDelta;
                    best = location;
                }
            }

            if (best >= 0)
            {
                var expectedObjective = solution.ObjectiveValue + min;
                solution.OpenLocation(best);
                Debug.Assert(solution.ObjectiveValue == expectedObjective);
            }
        }

        public override bool IsDeterministicLocalSearch => true;
        public override bool FindsLocalMinimum => false;
        public override bool GeneratesImprovementPressure => true;
        public override bool CanWorsen => false;

        public override string ToString()
        {
            return "OpenBest";
        }
    }
}
