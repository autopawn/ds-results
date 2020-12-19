using System;
using System.IO;
using System.Linq;
using LabHelper.DataStructures;

namespace SPLP.CMCS
{
    class DeterministicCmcsConfiguration
    {
        private readonly CmcsComponent[] components;
        private readonly int[] improvedNext;
        private readonly int[] unimprovedNext;

        public DeterministicCmcsConfiguration(CmcsComponent[] components, int[] improvedNext, int[] unimprovedNext)
        {
            this.improvedNext = improvedNext;
            this.unimprovedNext = unimprovedNext;
            this.components = components;
        }

        public static DeterministicCmcsConfiguration Load(string filename, CmcsComponent[] componentsPool)
        {
            using (var reader = File.OpenText(filename))
            {
                var line = reader.ReadLine().Trim();
                var numberOfComponentsHeader = "Number of components:";
                if (!line.StartsWith(numberOfComponentsHeader))
                    throw new LabException("Wrong format");
                var componentsCount = int.Parse(line.Substring(numberOfComponentsHeader.Length).Trim());
                CmcsComponent[] components = new CmcsComponent[componentsCount];
                for (int i = 0; i < componentsCount; i++)
                {
                    var componentName = reader.ReadLine().Trim();
                    var component = componentsPool.First(c => c.ToString() == componentName);
                    components[i] = component;
                }

                var improvedNext = ReadMatrix("Improved:", componentsCount, reader);
                var unimprovedNext = ReadMatrix("Unimproved:", componentsCount, reader);

                return new DeterministicCmcsConfiguration(components, improvedNext, unimprovedNext);
            }
        }

        private static int[] ReadMatrix(string section, int componentsCount, StreamReader reader)
        {
            var firstLine = reader.ReadLine().Trim();
            if (firstLine != section)
                throw new LabException("Wrong format.");

            int[] next = new int[componentsCount];

            for (int from = 0; @from < componentsCount; @from++)
            {
                var strings = reader.ReadLine().Trim().Split('\t');
                if (strings.Length != componentsCount)
                    throw new LabException("Wrong format");

                double[] weights = strings.Select(s => double.Parse(s)).ToArray();
                int index = -1;
                for (int i = 0; i < weights.Length; i++)
                    if (Math.Abs(weights[i]) > 1e-5)
                    {
                        if (index >= 0)
                            throw new LabException("Non-deterministic");

                        index = i;
                    }

                next[@from] = index;
            }

            return next;
        }

        public int[] ImprovedNext => improvedNext;

        public int[] UnimprovedNext => unimprovedNext;

        public CmcsComponent[] Components => components;


        public void Save(string filename)
        {
            using (var writer = File.CreateText(filename))
            {
                writer.WriteLine("Number of components: " + components.Length);
                foreach (var component in components)
                    writer.WriteLine(component);

                SaveMatrix(writer, "Improved", improvedNext);
                SaveMatrix(writer, "Unimproved", unimprovedNext);
            }
        }

        private void SaveMatrix(StreamWriter writer, string matrixName, int[] matrix)
        {
            writer.WriteLine(matrixName + ":");
            for (int from = 0; @from < components.Length; @from++)
            {
                for (int to = 0; to < components.Length; to++)
                    writer.Write(matrix[@from] == to ? "1\t" : "0\t");
                writer.WriteLine();
            }
        }
    }
}
