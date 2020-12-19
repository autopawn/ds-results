using System;
using System.Collections.Generic;
using System.Diagnostics;
using LabHelper.Tools;

namespace SPLP.CMCS
{
    class SplpSolution
    {
        private readonly SplpInstance instance;
        private readonly bool[] opened;
        private readonly List<int> openedList;
        private readonly int[] firstClosestIndex;
        private readonly int[] secondClosestIndex;
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
            firstClosestIndex = int.MaxValue.FillArray(instance.NumberOfLocations);
            secondClosestIndex = int.MaxValue.FillArray(instance.NumberOfLocations);
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
                    var closenessIndex = instance.GetClosenessIndex(client, location);
                    if (closenessIndex > secondClosestIndex[client])
                        continue;

                    if (closenessIndex > firstClosestIndex[client])
                        secondClosestIndex[client] = closenessIndex;
                    else
                    {
                        secondClosestIndex[client] = firstClosestIndex[client];
                        firstClosestIndex[client] = closenessIndex;
                    }
                }
            }

            for (int client = instance.NumberOfClients - 1; client >= 0; client--)
                objectiveValue += instance.GetClientCostByClosenessIndex(client, firstClosestIndex[client]);

            Debug.Assert(IsFeasible);
        }

        public void OpenLocation(int location)
        {
            Debug.Assert(!opened[location]);

            for (int client = instance.NumberOfClients - 1; client >= 0; client--)
            {
                var closenessIndex = instance.GetClosenessIndex(client, location);
                if (closenessIndex < firstClosestIndex[client])
                {
                    int oldCost = instance.GetClientCostByClosenessIndex(client, firstClosestIndex[client]);

                    secondClosestIndex[client] = firstClosestIndex[client];
                    firstClosestIndex[client] = closenessIndex;
                    objectiveValue += instance.GetClientCostByClosenessIndex(client, closenessIndex) - oldCost;
                }
                else if (closenessIndex < secondClosestIndex[client])
                    secondClosestIndex[client] = closenessIndex;
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
                var firstLocation = instance.GetLocationByClosenessIndex(client, firstClosestIndex[client]);
                if (location == firstLocation)
                {
                    objectiveValue += instance.GetClientCostByClosenessIndex(client, secondClosestIndex[client]) -
                                      instance.GetClientCostByClosenessIndex(client, firstClosestIndex[client]);

                    firstClosestIndex[client] = secondClosestIndex[client];
                    for (int index = firstClosestIndex[client] + 1;; index++)
                    {
                        var l = instance.GetLocationByClosenessIndex(client, index);
                        if (opened[l])
                        {
                            secondClosestIndex[client] = index;
                            break;
                        }
                    }
                }
                else
                {
                    var secondLocation = instance.GetLocationByClosenessIndex(client, secondClosestIndex[client]);
                    if (location == secondLocation)
                        for (int index = secondClosestIndex[client] + 1;; index++)
                        {
                            var l = instance.GetLocationByClosenessIndex(client, index);
                            if (opened[l])
                            {
                                secondClosestIndex[client] = index;
                                break;
                            }
                        }
                }
            }

            objectiveValue -= instance.GetLocationCost(location);

            Debug.Assert(IsFeasible);
        }

        public int GetFirstClosestIndex(int client)
        {
            return firstClosestIndex[client];
        }

        public int GetSecondClosestIndex(int client)
        {
            return secondClosestIndex[client];
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
                    int first = int.MaxValue, second = int.MaxValue;
                    foreach (var location in openedList)
                    {
                        var closenessIndex = instance.GetClosenessIndex(client, location);
                        if (closenessIndex < first)
                        {
                            second = first;
                            first = closenessIndex;
                        }
                        else if (closenessIndex < second)
                            second = closenessIndex;
                    }

                    if (firstClosestIndex[client] != first)
                        return false;

                    if (secondClosestIndex[client] != second)
                        return false;

                    expectedObjectiveValue += instance.GetClientCostByClosenessIndex(client, first);
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
    class SplpAdvancedSolution
    {
        private readonly SplpInstance instance;
        private readonly bool[] opened;
        private readonly List<int> openedList;
        private readonly int[] firstClosestIndex;
        private readonly int[] secondClosestIndex;
        private int objectiveValue;

        public SplpInstance Instance => instance;

        public SplpAdvancedSolution(SplpInstance instance, int toOpenCount, Random random) : this(instance,
            random.DistinctUnordered(instance.NumberOfLocations, toOpenCount))
        {
        }

        public SplpAdvancedSolution(SplpInstance instance, int[] toOpen)
        {
            this.instance = instance;
            opened = new bool[instance.NumberOfLocations];
            firstClosestIndex = int.MaxValue.FillArray(instance.NumberOfLocations);
            secondClosestIndex = int.MaxValue.FillArray(instance.NumberOfLocations);
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
                    var closenessIndex = instance.GetClosenessIndex(client, location);
                    if (closenessIndex > secondClosestIndex[client])
                        continue;

                    if (closenessIndex > firstClosestIndex[client])
                        secondClosestIndex[client] = closenessIndex;
                    else
                    {
                        secondClosestIndex[client] = firstClosestIndex[client];
                        firstClosestIndex[client] = closenessIndex;
                    }
                }
            }

            for (int client = instance.NumberOfClients - 1; client >= 0; client--)
                objectiveValue += instance.GetClientCostByClosenessIndex(client, firstClosestIndex[client]);

            Debug.Assert(IsFeasible);
        }

        public void OpenLocation(int location)
        {
            Debug.Assert(!opened[location]);

            for (int client = instance.NumberOfClients - 1; client >= 0; client--)
            {
                var closenessIndex = instance.GetClosenessIndex(client, location);
                if (closenessIndex < firstClosestIndex[client])
                {
                    int oldCost = instance.GetClientCostByClosenessIndex(client, firstClosestIndex[client]);

                    secondClosestIndex[client] = firstClosestIndex[client];
                    firstClosestIndex[client] = closenessIndex;
                    objectiveValue += instance.GetClientCostByClosenessIndex(client, closenessIndex) - oldCost;
                }
                else if (closenessIndex < secondClosestIndex[client])
                    secondClosestIndex[client] = closenessIndex;
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
                var firstLocation = instance.GetLocationByClosenessIndex(client, firstClosestIndex[client]);
                if (location == firstLocation)
                {
                    objectiveValue += instance.GetClientCostByClosenessIndex(client, secondClosestIndex[client]) -
                                      instance.GetClientCostByClosenessIndex(client, firstClosestIndex[client]);

                    firstClosestIndex[client] = secondClosestIndex[client];
                    for (int index = firstClosestIndex[client] + 1;; index++)
                    {
                        var l = instance.GetLocationByClosenessIndex(client, index);
                        if (opened[l])
                        {
                            secondClosestIndex[client] = index;
                            break;
                        }
                    }
                }
                else
                {
                    var secondLocation = instance.GetLocationByClosenessIndex(client, secondClosestIndex[client]);
                    if (location == secondLocation)
                        for (int index = secondClosestIndex[client] + 1;; index++)
                        {
                            var l = instance.GetLocationByClosenessIndex(client, index);
                            if (opened[l])
                            {
                                secondClosestIndex[client] = index;
                                break;
                            }
                        }
                }
            }

            objectiveValue -= instance.GetLocationCost(location);

            Debug.Assert(IsFeasible);
        }

        public int GetFirstClosestIndex(int client)
        {
            return firstClosestIndex[client];
        }

        public int GetSecondClosestIndex(int client)
        {
            return secondClosestIndex[client];
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
                    int first = int.MaxValue, second = int.MaxValue;
                    foreach (var location in openedList)
                    {
                        var closenessIndex = instance.GetClosenessIndex(client, location);
                        if (closenessIndex < first)
                        {
                            second = first;
                            first = closenessIndex;
                        }
                        else if (closenessIndex < second)
                            second = closenessIndex;
                    }

                    if (firstClosestIndex[client] != first)
                        return false;

                    if (secondClosestIndex[client] != second)
                        return false;

                    expectedObjectiveValue += instance.GetClientCostByClosenessIndex(client, first);
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
