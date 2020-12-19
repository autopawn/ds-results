using System;
using LabHelper.Tools;

namespace SPLP.CMCS
{
    class RandomCloseImprovement : CmcsComponent
    {
        public override void Apply(SplpSolution solution, Random random)
        {
            if (solution.OpenedList.Count <= 2)
                return;

            var location = solution.OpenedList.Random(random);
            int oldObjective = solution.ObjectiveValue;
            solution.CloseLocation(location);
            if (solution.ObjectiveValue > oldObjective)
                solution.OpenLocation(location);
        }

        public override bool IsDeterministicLocalSearch => false;
        public override bool FindsLocalMinimum => false;
        public override bool GeneratesImprovementPressure => true;
        public override bool CanWorsen => false;

        public override string ToString()
        {
            return "RndCloseImp";
        }
    }
}
