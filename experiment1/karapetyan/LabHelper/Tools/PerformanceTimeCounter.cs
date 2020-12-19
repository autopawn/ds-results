using LabHelper.DataStructures;

namespace LabHelper.Tools
{
    public class PerformanceTimeCounter : ITimeCounter
    {
        private long start;
        private long stop;

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long perfcount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long freq);

        private static readonly long freq;

        static PerformanceTimeCounter()
        {
            QueryPerformanceFrequency(out freq);
        }

        public void Start()
        {
            QueryPerformanceCounter(out start);
        }

        public void Stop()
        {
            QueryPerformanceCounter(out stop);
        }

        public void Resume()
        {
            long cur;
            QueryPerformanceCounter(out cur);
            if (start != stop && cur < start)
                throw new LabException();

            if (stop == 0 && start != 0)
                throw new LabException();

            start = cur - (stop - start);
            stop = 0;
        }

        public TimeInterval Elapsed
        {
            get
            {
                return TimeInterval.FromSeconds((stop - start) * 1.0 / freq);
            }
        }

        public TimeInterval CurrentElapsed
        {
            get
            {
                long cur;
                QueryPerformanceCounter(out cur);
                return TimeInterval.FromSeconds((cur - start) * 1.0 / freq);
            }
        }

        public void Reset()
        {
            start = stop = 0;
        }
    }
}
