using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ILOG.CPLEX;
using LabHelper.DataStructures;
using LabHelper.OutputGeneration;
using LabHelper.Tools;
using SPLP.CMCS;
using SPLP.Output;

namespace SPLP
{
    class Program
    {
        static void Main(string[] args)
        {
            //GenerateAnalysisTable();
            //GenerateAnalysisTextTable();
            //GenerateReport();
            //SolveWithCplex(false, "b140", new[] {41, 44, 75, 80, 99, 129, 133, 134});
            //SolveWithCplex(false, "b160", null);
            //TestRelaxation("b140");
            //CheckDistances("b140");
            //TestCmcs("b10");
            GenerateCmcs();
            //RunMemetic("b500", null, TimeInterval.Minute);
            SolveUnsolved(TimeInterval.Second * 7200);
            //RunMemetic("gs750a-3", Helper.GetSubdirectory(Instance.InstancesDirectory, "KoerkelGhosh-sym", "750", "a"), TimeInterval.Hour * 7);
            return;

            //Polynomial.stats = new int[60];
            //Polynomial.missStats = new int[60];

            /*            var polynomial = new Polynomial(2, null);
                        polynomial.AddTerm(1, new PolynomialTerm(new [] {true, true}));
                        Helper.InfoMessage($"Initial:  {polynomial.VarsInUse}");
                        polynomial = polynomial.Fix(0, true, 1, FixingMethod.Branching);
                        Helper.InfoMessage($"After fixing:  {polynomial.VarsInUse}");*/

            //var instance = new Instance(Helper.GetFullFilename(Helper.GetSubdirectory(InstancesDirectory, "KoerkelGhosh-sym", "250", "b"), "gs250b-1"));
            //var instance = new Instance(Helper.GetFullFilename(InstancesDirectory, "b10.txt"));
            //var instance = new Instance(Helper.GetFullFilename(InstancesDirectory, "test.txt"));
            //foreach (var instanceName in new[] {"b30"})
            //foreach (var instanceName in new[] {"b6", "b10"})
            //foreach (var instanceName in new[] {"b10", "b20", "b30"})
            foreach (var instanceName in new[] {"b30", "b40", "b50", "b60"})
		    {
		        var instance = new Instance(instanceName);
		        //foreach (var enableLowerBounds in Helper.TrueFalse)
		        var enableLowerBounds = true;
		        foreach (var enableBranchingOnGroups in Helper.FalseTrue)
		        //foreach (var enableBranchingOnGroups in new[]{true})
		        {
		            var treeSearchSolver = new TreeSearchSolver(enableLowerBounds, false, instance, enableBranchingOnGroups, false, true, true);
		            treeSearchSolver.Solve(true);

		            Helper.InfoMessage(
		                $"Solved {instance.Name} with LB = {enableLowerBounds} and GB = {enableBranchingOnGroups}.  #nodes {treeSearchSolver.Root.NumberOfNodes}");
		        }
		    }

		    //for (int i = 0; i < Polynomial.stats.Length; i++)
		    //    Console.WriteLine($"{i} vars: {Polynomial.stats[i] * 1.0 / (Polynomial.missStats[i] + Polynomial.stats[i]):0.000} out of {Polynomial.missStats[i] + Polynomial.stats[i]:0}");
		}

        private static void SolveUnsolved(TimeInterval timeBudget)
        {
            var lines = File.ReadAllLines(Helper.GetFullFilename("unsolved.txt"));
            var outputFilename = Helper.GetFullFilename("unsolved-results.txt");
            File.Delete(outputFilename);
            File.AppendAllText(outputFilename, $"Instance\tPreviously best known\tFischetti best known\tCMCS {timeBudget}\tBest found in our experiments\n");
            var cmcs = new DeterministicCmcs(DeterministicCmcsConfiguration.Load("best-configuration.cmcs", componentsPool));
            var random = new Random();
            
            var worker = new MultithreadWorker(Helper.NumberOfPhysicalCores);

            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                var strings = line.Split(Helper.Tab);
                string instanceName = strings[0];
                int prevBestKnown = int.Parse(strings[1]);
                int fischettiBestKnown = int.Parse(strings[2]);

                var index1 = index;
                worker.Enqueue(() =>
                {
                    var random1 = new Random();
                    Helper.InfoMessage($"{index1 + 1}: Solving {instanceName}");

                    var instance = new Instance(instanceName);
                    var splpInstance = new SplpInstance(instance);
                    SplpSolution solution = new SplpSolution(splpInstance, 2, random1);
                    cmcs.Solve(solution, timeBudget, random1);

                    Instance.ImprovedBestKnown(instanceName, solution.GetOpenedMap());

                    lock (worker)
                        File.AppendAllText(outputFilename,
                            $"{instanceName}\t{prevBestKnown}\t{fischettiBestKnown}\t{solution.ObjectiveValue}\t{Instance.GetBestKnownSolution(instance)}\n");
                });

            }

            worker.Wait();
        }

        private static void RunMemetic(string instanceName, TimeInterval timeBudget)
        {
            var instance = new Instance(instanceName);
            Helper.InfoMessage($"Best known solution is {Instance.GetBestKnownSolution(instance)}");

            var splpInstance = new SplpInstance(instance);
            var configuration = DeterministicCmcsConfiguration.Load(Helper.GetFullFilename("best-configuration.cmcs"), componentsPool);
            var algorithm = new MemeticAlgorithm(splpInstance, configuration);
            algorithm.Solve(timeBudget);
        }

        private static void TestCmcs(string instanceName)
        {
            var instance = new Instance(instanceName);
            var components = new CmcsComponent[]
            {
                new RandomMove(1, 1),
                new RandomCloseImprovement(), 
                new RandomOpenImprovement(), 
            };

            var configuration = new DeterministicCmcsConfiguration(components, new [] {1, 1, 0}, new [] {1, 2, 2});
            var cmcs = new DeterministicCmcs(configuration);

            var splpInstance = new SplpInstance(instance.GetF(), instance.GetC(), instanceName);
            var random = new Random(123);
            var splpSolution = new SplpSolution(splpInstance, instance.NumberOfLocations, random);
            Helper.InfoMessage($"Old objective: {splpSolution.ObjectiveValue}");
            cmcs.Solve(splpSolution, TimeInterval.Second * 1, random);
            Helper.InfoMessage($"New objective: {splpSolution.ObjectiveValue}");
        }

        private static CmcsComponent[] componentsPool =
        {
            new RandomMove(1, 0),
            new RandomMove(0, 1),
            new RandomMove(1, 1),
            new RandomMove(1, 2),
            new RandomMove(2, 1),
            new OpenBest(),
            new CloseBest(),
            new RandomBestExchange(),
            new BestExchange(), 
        };
        private static IEnumerable<Test> GetTests(int minSize, int maxSize, TimeInterval timeBudget, int count, TimeInterval presolveTime, Random random)
        {
            DeterministicCmcs presolveCmcs = null;
            if (!presolveTime.IsZero)
            {
                var configuration = DeterministicCmcsConfiguration.Load(Helper.GetFullFilename("best-configuration.cmcs"), componentsPool);
                presolveCmcs = new DeterministicCmcs(configuration);
            }

            Dictionary<int, SplpInstance> instances = new Dictionary<int, SplpInstance>();
            Dictionary<int, int> optimalObjectiveValues = new Dictionary<int, int>();
            for (int i = 0; i < count; i++)
            {
                var size = random.Next(minSize, maxSize + 1);
                if (!instances.ContainsKey(size))
                {
                    var instance = new Instance("b" + size);
                    var splpInstance = new SplpInstance(instance.GetF(), instance.GetC(), instance.Name);
                    instances.Add(size, splpInstance);
                    var optimal = Instance.GetBestKnownSolution(instance);
                    optimalObjectiveValues.Add(size, optimal);
                }

                var solution = new SplpSolution(instances[size], random.Next(2, size / 10 + 1), random);
                if (presolveCmcs != null)
                {
                    presolveCmcs.Solve(solution, presolveTime, random);
                    for (int j = 0; j < 5; j++)
                    {
                        var location = random.Next(solution.Instance.NumberOfLocations);
                        if (!solution.IsOpened(location))
                            solution.OpenLocation(location);
                    }
                }

                yield return new Test(instances[size], timeBudget, solution.OpenedList.ToArray(), optimalObjectiveValues[size]);
                //yield return new Test(instances[size], timeBudget, random, optimalObjectiveValues[size]);
            }
        }

        private static void GenerateCmcs()
        {
            var components = new CmcsComponent[]
            {
                new RandomMove(1, 0),
                new RandomMove(0, 1),
                //new RandomMove(1, 1),
                new RandomMove(1, 2),
                new RandomMove(2, 1),
                //new RandomMove(2, 2),
                new OpenBest(), 
                new CloseBest(), 
                new RandomBestExchange(), 
            };

            var random = new Random(123);

            var tests = GetTests(300, 350, TimeInterval.Ms * 100, 500, TimeInterval.Zero, random).ToArray();
            //var tests = GetTests(200, 210, TimeInterval.Ms * 50, 200, TimeInterval.Zero, random).ToArray();

            var candidates = new DeterministicConfigurationsEnumerator(components, 2);
            var generator = new DeterministicCmcsBruteForceGenerator(candidates, tests);
            generator.Generate();
        }

        private static int GetLocationIndex(Instance instance, int location, int client)
        {
            return instance.GetClosenessIndex(client, location);
        }

        private static int Distance(Instance instance, int location1, int location2)
        {
            return instance.GetDistanceBetweenLocations(location1, location2, 10);
        }

        private static void CheckDistances(string instanceName)
        {
            var instance = new Instance(instanceName);

            var solution = new CplexClassicSolver(instance).Solve(true, true);
            List<int> distancesInSolution = new List<int>();
            for (int l1 = 0; l1 < instance.NumberOfLocations; l1++)
            {
                if (solution[l1])
                    continue;

                for (int l2 = l1 + 1; l2 < instance.NumberOfLocations; l2++)
                {
                    if (solution[l2])
                        continue;

                    distancesInSolution.Add(Distance(instance, l1, l2));
                }
            }

            Console.WriteLine("Distances in solution: " + distancesInSolution.OrderBy(v => v).ListToString(" "));

            int[] counters = new int[instance.NumberOfLocations];
            for (int client = 0; client < instance.NumberOfClients; client++)
            {
                int minOpenedIndex = int.MaxValue;
                for (int location = 0; location < instance.NumberOfLocations; location++)
                {
                    if (solution[location])
                        continue;

                    minOpenedIndex = Math.Min(minOpenedIndex, GetLocationIndex(instance, location, client));
                }

                counters[minOpenedIndex]++;
            }
            Console.WriteLine();

            for (int i = 0; i < counters.Length; i++)
            {
                if (counters[i] > 0)
                    Console.WriteLine($"Index {i}: {counters[i]} clients");
            }
            Console.WriteLine();


            for (int client = 0; client < instance.NumberOfClients; client++)
            {
                int minAvailableCost = int.MaxValue;
                int minSelected = int.MaxValue;
                for (int location = 0; location < instance.NumberOfLocations; location++)
                {
                    minAvailableCost = Math.Min(minAvailableCost, instance.GetC(location, client));
                    if (!solution[location])
                        minSelected = Math.Min(minSelected, instance.GetC(location, client));
                }

                Console.WriteLine($"Client {client}: cost diff = {minSelected - minAvailableCost}");
            }
            Console.WriteLine();

            for (int location = 0; location < instance.NumberOfLocations; location++)
            {
                List<int> distances = new List<int>();
                for (int location2 = 0; location2 < instance.NumberOfLocations; location2++)
                {
                    if (location2 == location)
                        continue;

                    distances.Add(Distance(instance, location, location2));
                }

                Console.WriteLine($"Location {location}: distances {distances.Min()} to {distances.Max()}; 5%: {distances.Percentile(0.05)}; 10%: {distances.Percentile(0.1)}");
            }
        }

        private static void TestRelaxation(string instanceName)
        {
            var instance = new Instance(instanceName);
            var polynomial = instance.GeneratePolynomial();
            var a = new Range(instance.NumberOfLocations).Select(k => polynomial.GetA(k)).ToArray();
            var aSorted = (int[])a.Clone();
            Array.Sort(aSorted);
            for (int k = 0; k < a.Length; k++)
            {
                if (a[k] >= aSorted[instance.NumberOfLocations - 16])
                    Console.WriteLine(k + " " + a[k]);
            }

            var classicCounter = AccurateRealTimeCounter.StartNew();
            var classic = polynomial.ClassicLpLowerBound;
            Console.WriteLine("LP relaxation: " + classic + " " + classicCounter.CurrentElapsed);

            var pbCounter = AccurateRealTimeCounter.StartNew();
            var pbValue = polynomial.LpLowerBound;
            Console.WriteLine("PB relaxation: " + pbValue + " " + pbCounter.CurrentElapsed);
        }

        private static void SolveWithCplex(bool pbFormulation, string instanceName, int[] mipStart)
        {
            var instance = new Instance(instanceName);
            if (pbFormulation)
                new CplexPbSolver(instance.GeneratePolynomial()).Solve(true, true, mipStart, 8);
            else
                new CplexClassicSolver(instance).Solve(true, true, mipStart);
        }

        private static void GenerateReport()
        {
            var instance = new Instance("b90");
            var treeSearchSolver = new TreeSearchSolver(true, false, instance, true, false, true, true);
            treeSearchSolver.Solve(true);
        }

        private static void GenerateAnalysisTextTable()
        {
            //var table = new AnalysisTextTable(new Range(125, 160, 5).Select(n => $"b{n}").ToArray());
            var table = new AnalysisTextTable(new Range(165, 170, 5).Select(n => $"b{n}").ToArray());
            table.Generate(true);
        }

        private static void GenerateAnalysisTable()
        {
            LaTeXOutput output = new LaTeXOutput()
            {
                DeleteTempFiles = true,
                DeleteTexFile = false,
                Pdf = true,
                Path = Helper.ProgramDirectory.FullName,
                Filename = "analysis"
            };

            output.Add(new AnalysisTable(new Range(80, 100, 5).Select(n => $"b{n}").ToArray()));
            output.ProduceLatex();
            output.CompileAsync(true);
        }
    }
}
