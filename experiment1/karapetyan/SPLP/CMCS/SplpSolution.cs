using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using LabHelper.Tools;

namespace SPLP.CMCS
{
    class SplpSolution
    {
        private readonly SplpInstance instance;
        private readonly bool[] opened;
        private readonly List<int> openedList;
        private readonly int[] firstClosestLocation;
        private readonly int[] secondClosestLocation;
        private int objectiveValue;

        public SplpInstance Instance => instance;

        public SplpSolution(SplpInstance instance, int toOpenCount, Random random) : this(instance,
            random.DistinctUnordered(instance.NumberOfLocations, toOpenCount))
        {
        }

        public SplpSolution(SplpInstance instance, int[] toOpen)
        {
            this.instance = instance;
            opened = new bool[instance.NumberOfLocations];
            firstClosestLocation = (-1).FillArray(instance.NumberOfLocations);
            secondClosestLocation = (-1).FillArray(instance.NumberOfLocations);
            openedList = new List<int>();

            Debug.Assert(toOpen.Length >= 2);
            for (int i = toOpen.Length - 1; i >= 0; i--)
            {
                int location = toOpen[i];
                Debug.Assert(!opened[location]);

                objectiveValue += instance.GetLocationCost(location);

                opened[location] = true;
                openedList.Add(location);
                for (int client = instance.NumberOfClients - 1; client >= 0; client--)
                {
                    var cost = instance.GetClientCost(location, client);
                    if (secondClosestLocation[client] >= 0 && cost >= instance.GetClientCost(secondClosestLocation[client], client))
                        continue;

                    if (firstClosestLocation[client] >= 0 && cost >= instance.GetClientCost(firstClosestLocation[client], client))
                        secondClosestLocation[client] = location;
                    else
                    {
                        secondClosestLocation[client] = firstClosestLocation[client];
	                    firstClosestLocation[client] = location;
                    }
                }
            }

	        for (int client = instance.NumberOfClients - 1; client >= 0; client--)
		        objectiveValue += instance.GetClientCost(firstClosestLocation[client], client);

	        Debug.Assert(IsFeasible);
        }

        public void OpenLocation(int location)
        {
            Debug.Assert(!opened[location]);

            for (int client = instance.NumberOfClients - 1; client >= 0; client--)
            {
                var newCost = instance.GetClientCost(location, client);
	            int oldCost = instance.GetClientCost(firstClosestLocation[client], client);

				if (newCost < oldCost)
                {
					secondClosestLocation[client] = firstClosestLocation[client];
	                firstClosestLocation[client] = location;

	                objectiveValue += newCost - oldCost;
                }
                else if (newCost < instance.GetClientCost(secondClosestLocation[client], client))
                    secondClosestLocation[client] = location;
            }

            objectiveValue += instance.GetLocationCost(location);
            opened[location] = true;
            openedList.Add(location);

            Debug.Assert(IsFeasible);
        }

        public void CloseLocation(int location)
        {
            Debug.Assert(opened[location]);

            openedList.Remove(location);
            Debug.Assert(openedList.Count >= 2);

            opened[location] = false;

            for (int client = instance.NumberOfClients - 1; client >= 0; client--)
            {
	            bool isFirst = location == firstClosestLocation[client];
				bool isSecond = location == secondClosestLocation[client];

	            if (isFirst)
	            {
		            objectiveValue += instance.GetClientCost(secondClosestLocation[client], client) -
		                              instance.GetClientCost(firstClosestLocation[client], client);

		            firstClosestLocation[client] = secondClosestLocation[client];
	            }

	            if (isFirst || isSecond)
	            {
		            secondClosestLocation[client] = -1;
		            int secondClosestCost = int.MaxValue;

		            for (int index = openedList.Count - 1; index >= 0; index--)
		            {
			            int l = openedList[index];
			            if (l == firstClosestLocation[client])
				            continue;

			            int c = instance.GetClientCost(l, client);
			            if (c < secondClosestCost)
			            {
				            secondClosestCost = c;
				            secondClosestLocation[client] = l;
			            }
		            }
	            }
            }

            objectiveValue -= instance.GetLocationCost(location);

            Debug.Assert(IsFeasible);
        }

        public int GetFirstClosestLocation(int client)
        {
            return firstClosestLocation[client];
        }

        public int GetSecondClosestLocation(int client)
        {
            return secondClosestLocation[client];
        }

        public int ObjectiveValue => objectiveValue;

        public bool IsFeasible
        {
            get
            {
                if (openedList.Count < 2)
                    return false;

                int expectedObjectiveValue = 0;
                for (int location = 0; location < instance.NumberOfLocations; location++)
                    if (opened[location])
                        expectedObjectiveValue += instance.GetLocationCost(location);

                bool[] visited = new bool[instance.NumberOfLocations];
                foreach (var location in openedList)
                {
                    if (!opened[location])
                        return false;

                    if (visited[location])
                        return false;

                    visited[location] = true;
                }

                for (int location = 0; location < instance.NumberOfLocations; location++)
                    if (visited[location] != opened[location])
                        return false;

                for (int client = 0; client < instance.NumberOfClients; client++)
                {
                    int first = -1, second = -1;
	                int firstCost = int.MaxValue;
	                int secondCost = int.MaxValue;
                    foreach (var location in openedList)
                    {
                        var cost = instance.GetClientCost(location, client);
                        if (cost < firstCost)
                        {
                            second = first;
	                        secondCost = firstCost;
                            first = location;
	                        firstCost = cost;
                        }
                        else if (cost < secondCost)
                        {
	                        second = location;
	                        secondCost = cost;
                        }
                    }

                    if (instance.GetClientCost(firstClosestLocation[client], client) != firstCost)
                        return false;

                    if (instance.GetClientCost(secondClosestLocation[client], client) != secondCost)
                        return false;

                    expectedObjectiveValue += firstCost;
                }

                if (objectiveValue != expectedObjectiveValue)
                    return false;

                return true;
            }
        }

        public bool IsOpened(int location)
        {
            return opened[location];
        }

        public IList<int> OpenedList => openedList;

        public bool[] GetOpenedMap()
        {
            return opened;
        }
    }
}
