using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LabHelper.DataStructures;
using LabHelper.Tools;

namespace SPLP.CMCS
{
    class DeterministicCmcs
    {
        private readonly CmcsComponent[] components;
        private readonly int[] improvedNext;
        private readonly int[] unimprovedNext;

        public DeterministicCmcs(DeterministicCmcsConfiguration configuration)
        {
            this.improvedNext = configuration.ImprovedNext;
            this.unimprovedNext = configuration.UnimprovedNext;
            this.components = configuration.Components;

            Debug.Assert(improvedNext.Length == components.Length);
            Debug.Assert(unimprovedNext.Length == components.Length);
        }

        public void Solve(SplpSolution solution, TimeInterval timeBudget, Random random,
            TransitionCounters counters = null)
        {
            int[] bestSolution = solution.OpenedList.ToArray();
            int currentComponent = 0;
            int bestFound = solution.ObjectiveValue;

            var timeCounter = AccurateRealTimeCounter.StartNew();
            int prevObjective = solution.ObjectiveValue;
            while (timeCounter.CurrentElapsed < timeBudget)
            {
                components[currentComponent].Apply(solution, random);
                bool improved = solution.ObjectiveValue < prevObjective;
                var nextComponent = (improved ? improvedNext : unimprovedNext)[currentComponent];

                counters?.Increase(currentComponent, nextComponent, improved);

                currentComponent = nextComponent;
                prevObjective = solution.ObjectiveValue;

                if (solution.ObjectiveValue < bestFound)
                {
                    bestFound = solution.ObjectiveValue;
                    bestSolution = solution.OpenedList.ToArray();
                }

/*
                if (Helper.EnsureDebugInfoInterval("worst-index", TimeInterval.Second) && solution.OpenedList.Count < 50 && solution.OpenedList.Count > 10)
                {
                    var first = new Range(solution.Instance.NumberOfClients).Select(solution.GetFirstClosestIndex);
                    var second = new Range(solution.Instance.NumberOfClients).Select(solution.GetSecondClosestIndex);
                    Helper.DebugMessage($"Number of locations: {solution.OpenedList.Count}; worst first = {first.Max()}, 90% {first.Percentile(0.9)}; worst second = {second.Max()}, 90% {second.Percentile(0.9)}");
                }
*/

            }

            bool[] inBest = new bool[solution.Instance.NumberOfLocations];
            for (int i = bestSolution.Length - 1; i >= 0; i--)
            {
                int location = bestSolution[i];
                inBest[location] = true;

                if (!solution.IsOpened(location))
                    solution.OpenLocation(location);
            }

            for (int i = solution.OpenedList.Count - 1; i >= 0; i--)
            {
                int location = solution.OpenedList[i];
                if (!inBest[location])
                    solution.CloseLocation(location);
            }

            Debug.Assert(solution.ObjectiveValue == bestFound);
        }
    }
}