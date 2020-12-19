using System;

namespace SPLP.CMCS
{
    abstract class CmcsComponent
    {
        public abstract void Apply(SplpSolution solution, Random random);
        public abstract bool IsDeterministicLocalSearch { get; }
        public abstract bool FindsLocalMinimum { get; }
        public abstract bool GeneratesImprovementPressure { get; }
        public abstract bool CanWorsen { get; }
    }
}
