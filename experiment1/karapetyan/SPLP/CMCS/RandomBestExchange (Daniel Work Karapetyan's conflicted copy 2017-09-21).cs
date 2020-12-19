using System;
using System.Diagnostics;
using LabHelper.Tools;

namespace SPLP.CMCS
{
    class RandomBestExchange : CmcsComponent
    {
        public override void Apply(SplpSolution solution, Random random)
        {
            int closingLocation = solution.OpenedList.Random(random);

            int[] deltas = new int[solution.Instance.NumberOfLocations];

            for (int client = solution.Instance.NumberOfClients - 1; client >= 0; client--)
            {
                var firstIndex = solution.GetFirstClosestIndex(client);
                var firstLocation = solution.Instance.GetLocationByClosenessIndex(client, firstIndex);

                int curCost = solution.Instance.GetClientCostByClosenessIndex(client, firstIndex);

                if (firstLocation == closingLocation)
                {
                    var secondIndex = solution.GetSecondClosestIndex(client);
                    int secondCost = solution.Instance.GetClientCostByClosenessIndex(client, secondIndex);
                    for (int index = secondIndex - 1; index >= 0; index--)
                    {
                        int location = solution.Instance.GetLocationByClosenessIndex(client, index);
                        deltas[location] += solution.Instance.GetClientCostByClosenessIndex(client, index) - curCost;
                    }

                    for (int index = solution.Instance.NumberOfLocations - 1; index > secondIndex; index--)
                    {
                        int location = solution.Instance.GetLocationByClosenessIndex(client, index);
                        deltas[location] += secondCost - curCost;
                    }
                }
                else
                {
                    for (int index = firstIndex - 1; index >= 0; index--)
                    {
                        int location = solution.Instance.GetLocationByClosenessIndex(client, index);
                        deltas[location] += solution.Instance.GetClientCostByClosenessIndex(client, index) - curCost;
                    }
                }
            }

            int min = 0;
            int best = -1;
            int closingLocationCost = solution.Instance.GetLocationCost(closingLocation);

            for (int location = deltas.Length - 1; location >= 0; location--)
            {
                if (solution.IsOpened(location))
                    continue;

                var fullDelta = deltas[location] + solution.Instance.GetLocationCost(location) - closingLocationCost;
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
                solution.CloseLocation(closingLocation);
                Debug.Assert(solution.ObjectiveValue == expectedObjective);
            }

        }

        public override bool IsDeterministicLocalSearch => false;
        public override bool FindsLocalMinimum => false;
        public override bool GeneratesImprovementPressure => true;
        public override bool CanWorsen => false;

        public override string ToString()
        {
            return "RndBestExch";
        }
    }
}
