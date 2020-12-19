using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LabHelper.DataStructures;
using LabHelper.Tools;

namespace SPLP
{
    class Instance
    {
        /// <summary>
        /// Number of locations
        /// </summary>
        private readonly int numberOfLocations;

        /// <summary>
        /// Number of customers
        /// </summary>
        private readonly int numberOfClients;

        /// <summary>
        /// Opening costs
        /// </summary>
        private readonly int[] f;

        /// <summary>
        /// Allocation costs [location][client]
        /// </summary>
        private readonly int[][] c;

        /// <summary>
        /// Instance name
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The index of a location according to its cost for a given client; [client][location]
        /// </summary>
        private readonly int[][] closenessIndex;


        public Instance(string instanceName)
        {
            using (var reader = File.OpenText(instanceName))
            {
                var line = reader.ReadLine();
                var match = Regex.Match(line, @"^FILE:\s*(.+)\s*$");
                if (!match.Success)
                    throw new LabException("Wrong first line format.");

                name = match.Groups[1].Value;

                line = reader.ReadLine();
                match = Regex.Match(line, @"^\s*(\d+)\s+(\d+)\s+(\d+)\s*$");
                numberOfLocations = Int32.Parse(match.Groups[1].Value);
                numberOfClients = Int32.Parse(match.Groups[2].Value);

                f = new int[numberOfLocations];
                c = new int[numberOfLocations][];

                for (int i = 0; i < numberOfLocations; i++)
                {
                    line = reader.ReadLine();
                    var strings = line.Split(Helper.SpaceAndTab, StringSplitOptions.RemoveEmptyEntries);

                    if (strings.Length != numberOfClients + 2)
                        throw new LabException("Wrong number of elements in ");

                    var index = Int32.Parse(strings[0]);
                    if (index != i + 1)
                        throw new LabException("Wrong line number");

                    f[i] = Int32.Parse(strings[1]);

                    c[i] = new int[numberOfClients];
                    for (int j = 0; j < numberOfClients; j++)
                        c[i][j] = Int32.Parse(strings[j + 2]);
                }
            }


            closenessIndex = new int[NumberOfClients][];
            for (int client = 0; client < NumberOfClients; client++)
            {
                closenessIndex[client] = new Range(NumberOfLocations).ToArray();

                var client1 = client;
                var costs = new Range(NumberOfLocations).Select(location => GetC(location, client1)).ToArray();
                Array.Sort(costs, closenessIndex[client]);
            }
        }

        public int[] GetF()
        {
            return f;
        }

        public int[][] GetC()
        {
            return c;
        }


        public int GetClosenessIndex(int client, int location)
        {
            return closenessIndex[client][location];
        }

        public int GetF(int i)
        {
            return f[i];
        }

        public int GetC(int location, int client)
        {
            return c[location][client];
        }

        public int NumberOfLocations => numberOfLocations;

        public int NumberOfClients => numberOfClients;

        public string Name => name;

        public int GetObjectiveValue(bool[] solution)
        {
            int result = 0;
            int[] opened = new Range(numberOfLocations).Where(i => solution[i]).ToArray();
            for (int client = 0; client < numberOfClients; client++)
            {
                int minCost = Int32.MaxValue;  
                for (int i = 0; i < opened.Length; i++)
                {
                    int cost = GetC(opened[i], client);
                    if (cost < minCost)
                        minCost = cost;
                }

                result += minCost;
            }

            for (int i = 0; i < opened.Length; i++)
                result += GetF(opened[i]);

            return result;
        }
    }
}
