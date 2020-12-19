using System;
using System.Threading;
using LabHelper.Tools;

namespace SPLP.CMCS
{
    class TransitionCounters
    {
        private readonly int[][] improvedCounters;
        private readonly int[][] unimprovedCounters;

        public TransitionCounters(int numberOfComponents)
        {
            improvedCounters = new int[numberOfComponents][];
            unimprovedCounters = new int[numberOfComponents][];
            for (int i = numberOfComponents - 1; i >= 0; i--)
            {
                improvedCounters[i] = new int[numberOfComponents];
                unimprovedCounters[i] = new int[numberOfComponents];
            }
        }

        public int GetCounter(int from, int to, bool improved)
        {
            return (improved ? improvedCounters : unimprovedCounters)[from][to];
        }

        public void Increase(int from, int to, bool improved)
        {
            Interlocked.Increment(ref (improved ? improvedCounters : unimprovedCounters)[from][to]);
        }

        public int GetTotal()
        {
            return GetTotal(false) + GetTotal(true);
        }

        public int GetTotal(bool improved)
        {
            int[][] counters = improved ? improvedCounters : unimprovedCounters;
            int total = 0;
            for (int i = 0; i < improvedCounters.Length; i++)
            for (int j = 0; j < improvedCounters.Length; j++)
                total += counters[i][j];

            return total;
        }

        public int GetMax()
        {
            int max = 0;
            for (int i = 0; i < improvedCounters.Length; i++)
            for (int j = 0; j < improvedCounters.Length; j++)
            {
                max = Math.Max(max, improvedCounters[i][j]);
                max = Math.Max(max, unimprovedCounters[i][j]);
            }

            return max;
        }
    }
}
