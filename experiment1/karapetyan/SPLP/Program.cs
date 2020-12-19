using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using LabHelper.DataStructures;
using SPLP.CMCS;


namespace SPLP
{
    class Program
    {
	    static void Main(string[] args)
	    {
		    var config2 = "two-component.cmcs";
		    var config3 = "three-component.cmcs";

            if(args.Length==2){
                // The instances are read from the input
                float seconds = float.Parse(args[0], CultureInfo.InvariantCulture.NumberFormat);
                Solve(seconds * TimeInterval.Second, config3, args[1]);
            }else{
                Console.WriteLine("Usage:");
                Console.WriteLine("mono ./SPLP.exe <time budge [s]> <input problem>");
            }

	    }



	    private static void Solve(TimeInterval timeBudget, string configuration, string instanceName)
	    {
		    var cmcs = new DeterministicCmcs(DeterministicCmcsConfiguration.Load(configuration, ComponentsPool));

		    var random = new Random();

		    var instance = new Instance(instanceName);
		    var splpInstance = new SplpInstance(instance);
		    SplpSolution solution = new SplpSolution(splpInstance, 2, random);
		    cmcs.Solve(solution, timeBudget, random);
		    Console.WriteLine("Solution found: " + solution.ObjectiveValue);
	    }

	    private static readonly CmcsComponent[] ComponentsPool =
            new CmcsComponent[]
            {
                new OpenBest(),
                new CloseBest(),
                new RandomBestExchange(),
                new BestExchange(),
            }.Concat(GetMutations(8)).ToArray();


        private static IEnumerable<CmcsComponent> GetMutations(int maxStrength)
        {
            for (int strength = 1; strength <= maxStrength; strength++)
            {
                yield return new RandomMove(strength, 0);
                yield return new RandomMove(0, strength);
            }
        }
    }
}
