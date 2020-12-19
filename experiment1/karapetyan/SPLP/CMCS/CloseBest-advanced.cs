using System;
using System.Diagnostics;

namespace SPLP.CMCS
{
    class CloseBest : CmcsComponent
    {
        public override void Apply(SplpSolution solution, Random random)
        {
            if (solution.OpenedList.Count <= 2)
                return;

            int[] deltas = new int[solution.Instance.NumberOfLocations];

            for (int client = solution.Instance.NumberOfClients - 1; client >= 0; client--)
            {
                var firstIndex = solution.GetFirstClosestIndex(client);
                var secondIndex = solution.GetSecondClosestIndex(client);
                int curCost = solution.Instance.GetClientCostByClosenessIndex(client, firstIndex);
                int newCost = solution.Instance.GetClientCostByClosenessIndex(client, secondIndex);
                int location = solution.Instance.GetLocationByClosenessIndex(client, firstIndex);
                deltas[location] += newCost - curCost;
            }

            int min = 0;
            int best = -1;
            for (int i = solution.OpenedList.Count - 1; i >= 0; i--)
            {
                var location = solution.OpenedList[i];
                var fullDelta = deltas[location] - solution.Instance.GetLocationCost(location);
                if (fullDelta < min)
                {
                    min = fullDelta;
                    best = location;
                }
            }

            if (best >= 0)
            {
                var expectedObjective = solution.ObjectiveValue + min;
                solution.CloseLocation(best);
                Debug.Assert(solution.ObjectiveValue == expectedObjective);
            }

        }

        public override bool IsDeterministicLocalSearch => true;
        public override bool FindsLocalMinimum => false;
        public override bool GeneratesImprovementPressure => true;
        public override bool CanWorsen => false;

        public override string ToString()
        {
            return "CloseBest";
        }
    }
}
