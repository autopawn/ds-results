using System;
using LabHelper.Tools;

namespace SPLP.CMCS
{
    class RandomOpenImprovement : CmcsComponent
    {
        public RandomOpenImprovement()
        {
        }

        public override void Apply(SplpSolution solution, Random random)
        {
            int location;
            do
            {
                location = random.Next(solution.Instance.NumberOfLocations);
            } while (solution.IsOpened(location));

            int oldObjective = solution.ObjectiveValue;
            solution.OpenLocation(location);
            if (solution.ObjectiveValue > oldObjective)
                solution.CloseLocation(location);
        }

        public override bool IsDeterministicLocalSearch => false;
        public override bool FindsLocalMinimum => false;
        public override bool GeneratesImprovementPressure => true;
        public override bool CanWorsen => false;

        public override string ToString()
        {
            return "RndOpenImp";
        }
    }
}
