using System;

namespace SPLP.CMCS
{
    class SplpInstance
    {
        private int[] locationCost;
        private int[][] clientCost; // [location][client]
        private int[][] closenessIndex; // [client][location]
        private int[][] locationByClosenessIndex; // [client][closenessIndex]
        private int[][] clientCostsByCloseness; //[client][closenessIndex]

        private int numberOfClients;

        private string name;

        public int NumberOfClients => numberOfClients;
        public int NumberOfLocations => locationCost.Length;

        public SplpInstance(int[] f, int[][] c, string name)
        {
            locationCost = f;
            clientCost = c;
            this.name = name;
            numberOfClients = clientCost[0].Length;
            locationByClosenessIndex = new int[numberOfClients][];
            clientCostsByCloseness = new int[numberOfClients][];
            closenessIndex = new int[numberOfClients][];


            for (int client = numberOfClients - 1; client >= 0; client--)
            {
                var costs = new int[NumberOfLocations];
                var locations = new int[NumberOfLocations];

                for (int location = costs.Length - 1; location >= 0; location--)
                {
                    costs[location] = GetClientCost(location, client);
                    locations[location] = location;
                }

                Array.Sort(costs, locations);

                var indices = new int[NumberOfLocations];
                for (int index = NumberOfLocations - 1; index >= 0; index--)
                    indices[locations[index]] = index;

                clientCostsByCloseness[client] = costs;
                locationByClosenessIndex[client] = locations;
                closenessIndex[client] = indices;
            }
        }

        public SplpInstance(Instance instance) : this(instance.GetF(), instance.GetC(), instance.Name)
        {
        }

        public int GetLocationCost(int location)
        {
            return locationCost[location];
        }

        /// <summary>
        /// Cache inefficient
        /// </summary>
        /// <param name="client"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public int GetClosenessIndex(int client, int location)
        {
            return closenessIndex[client][location];
        }

        public int GetClientCost(int location, int client)
        {
            return clientCost[location][client];
        }

        public int GetClientCostByClosenessIndex(int client, int index)
        {
            return clientCostsByCloseness[client][index];
        }

        public int GetLocationByClosenessIndex(int client, int closenessIndex)
        {
            return locationByClosenessIndex[client][closenessIndex];
        }

        public override string ToString()
        {
            return name;
        }
    }
}
