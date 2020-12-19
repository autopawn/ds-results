using System;
using System.Diagnostics;

namespace SPLP.CMCS
{
    class BestExchange : CmcsComponent
    {
        public override void Apply(SplpSolution solution, Random random)
        {
            int[][] deltas = new int[solution.OpenedList.Count][];
            for (int i = deltas.Length - 1; i >= 0; i--)
                deltas[i] = new int[solution.Instance.NumberOfLocations];

            int[] rowByLocation = new int[solution.Instance.NumberOfLocations];
            for (int i = solution.OpenedList.Count - 1; i >= 0; i--)
                rowByLocation[solution.OpenedList[i]] = i;
    
            for (int client = solution.Instance.NumberOfClients - 1; client >= 0; client--)
            {
                var firstLocation = solution.GetFirstClosestLocation(client);
                var secondLocation = solution.GetSecondClosestLocation(client);
                var curCost = solution.Instance.GetClientCost(firstLocation, client);

                // New location to open is better than current best
                int index;
                for (index = 0; ; index++)
                {
                    int locationToOpen = solution.Instance.GetLocationByClosenessIndex(client, index);
                    if (locationToOpen == firstLocation)
                        break;

                    var newCost = solution.Instance.GetClientCost(locationToOpen, client);
                    for (int i = solution.OpenedList.Count - 1; i >= 0; i--)
                        deltas[i][locationToOpen] += newCost - curCost;
                }

                var secondCost = solution.Instance.GetClientCost(secondLocation, client);

                // If closing the closest location will not affect the cost
                if (curCost == secondCost)
                    continue;

                // New location is between first and second closest (hence only matters if we close first)
                int row = rowByLocation[firstLocation];
                for (index++; ; index++)
                {
                    int locationToOpen = solution.Instance.GetLocationByClosenessIndex(client, index);
                    if (locationToOpen == secondLocation)
                        break;

                    var newCost = solution.Instance.GetClientCost(locationToOpen, client);

                    deltas[row][locationToOpen] += newCost - curCost;
                }

                for (index++; index < solution.Instance.NumberOfLocations; index++)
                {
                    int locationToOpen = solution.Instance.GetLocationByClosenessIndex(client, index);

                    deltas[row][locationToOpen] += secondCost - curCost;
                }
            }

            int min = 0;
            int bestToClose = -1;
            int bestToOpen = -1;
            for (int row = deltas.Length - 1; row >= 0; row--)
            {
                int locationToClose = solution.OpenedList[row];
                int closingCost = solution.Instance.GetLocationCost(locationToClose);
                for (int locationToOpen = solution.Instance.NumberOfLocations - 1; locationToOpen >= 0; locationToOpen--)
                {
                    if (solution.IsOpened(locationToOpen))
                        continue;

                    int openingCost = solution.Instance.GetLocationCost(locationToOpen);
                    var fullDelta = deltas[row][locationToOpen] + openingCost - closingCost;
                    if (fullDelta < min)
                    {
                        min = fullDelta;
                        bestToClose = locationToClose;
                        bestToOpen = locationToOpen;
                    }
                }
            }

            if (bestToClose >= 0)
            {
                int oldObjective = solution.ObjectiveValue;
                solution.OpenLocation(bestToOpen);
                solution.CloseLocation(bestToClose);
                Debug.Assert(oldObjective + min == solution.ObjectiveValue);
            }
        }
/*
        public override void Apply(SplpSolution solution, Random random)
        {
            int[][] deltas = new int[solution.OpenedList.Count][];
            for (int i = deltas.Length - 1; i >= 0; i--)
                deltas[i] = new int[solution.Instance.NumberOfLocations];

            int[] rowByLocation = new int[solution.Instance.NumberOfLocations];
            for (int i = solution.OpenedList.Count - 1; i >= 0; i--)
                rowByLocation[solution.OpenedList[i]] = i;
    
            for (int client = solution.Instance.NumberOfClients - 1; client >= 0; client--)
            {
                var firstIndex = solution.GetFirstClosestIndex(client);
                var secondIndex = solution.GetSecondClosestIndex(client);
                var curCost = solution.Instance.GetClientCostByClosenessIndex(client, firstIndex);

                // New location to open is better than current best
                for (int index = firstIndex - 1; index >= 0; index--)
                {
                    var newCost = solution.Instance.GetClientCostByClosenessIndex(client, index);
                    int locationToOpen = solution.Instance.GetLocationByClosenessIndex(client, index);
                    for (int i = solution.OpenedList.Count - 1; i >= 0; i--)
                        deltas[i][locationToOpen] += newCost - curCost;
                }

                // New location is between first and second closest (hence only matters if we close first)
                int row = rowByLocation[solution.Instance.GetLocationByClosenessIndex(client, firstIndex)];
                for (int index = secondIndex - 1; index > firstIndex; index--)
                {
                    var newCost = solution.Instance.GetClientCostByClosenessIndex(client, index);
                    int locationToOpen = solution.Instance.GetLocationByClosenessIndex(client, index);

                    deltas[row][locationToOpen] += newCost - curCost;
                }

                var secondCost = solution.Instance.GetClientCostByClosenessIndex(client, secondIndex);
                for (int index = solution.Instance.NumberOfLocations - 1; index > secondIndex; index--)
                {
                    int locationToOpen = solution.Instance.GetLocationByClosenessIndex(client, index);

                    deltas[row][locationToOpen] += secondCost - curCost;
                }
            }

            int min = 0;
            int bestToClose = -1;
            int bestToOpen = -1;
            for (int row = deltas.Length - 1; row >= 0; row--)
            {
                int locationToClose = solution.OpenedList[row];
                int closingCost = solution.Instance.GetLocationCost(locationToClose);
                for (int locationToOpen = solution.Instance.NumberOfLocations - 1; locationToOpen >= 0; locationToOpen--)
                {
                    if (solution.IsOpened(locationToOpen))
                        continue;

                    int openingCost = solution.Instance.GetLocationCost(locationToOpen);
                    var fullDelta = deltas[row][locationToOpen] + openingCost - closingCost;
                    if (fullDelta < min)
                    {
                        min = fullDelta;
                        bestToClose = locationToClose;
                        bestToOpen = locationToOpen;
                    }
                }
            }

            if (bestToClose >= 0)
            {
                int oldObjective = solution.ObjectiveValue;
                solution.OpenLocation(bestToOpen);
                solution.CloseLocation(bestToClose);
                Debug.Assert(oldObjective + min == solution.ObjectiveValue);
            }
        }
*/

        public override bool IsDeterministicLocalSearch => true;
        public override bool FindsLocalMinimum => false;
        public override bool GeneratesImprovementPressure => true;
        public override bool CanWorsen => false;

        public override string ToString()
        {
            return "BestExchange";
        }
    }
}
