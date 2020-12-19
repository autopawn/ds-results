using System;
using LabHelper.Tools;

namespace SPLP.CMCS
{
    class RandomMove : CmcsComponent
    {
        private readonly int toOpen;
        private readonly int toClose;

        public RandomMove(int toOpen, int toClose)
        {
            this.toOpen = toOpen;
            this.toClose = toClose;
        }

        public override void Apply(SplpSolution solution, Random random)
        {
            for (int i = Math.Min(toClose, solution.OpenedList.Count - 2) - 1; i >= 0; i--)
            {
                var location = solution.OpenedList.Random(random);
                solution.CloseLocation(location);
            }

            for (int i = toOpen - 1; i >= 0; i--)
            {
                var location = random.Next(solution.Instance.NumberOfLocations);
                if (!solution.IsOpened(location))
                    solution.OpenLocation(location);
            }
        }

        public override bool IsDeterministicLocalSearch => false;
        public override bool FindsLocalMinimum => false;
        public override bool GeneratesImprovementPressure => false;
        public override bool CanWorsen => true;

        public override string ToString()
        {
            return "Rnd" + (toOpen == 0 ? "" : "Open" + toOpen) + (toClose == 0 ? "" : "Close" + toClose);
            //return $"RndOpen{toOpen}-Close{toClose}";
        }
    }
}
