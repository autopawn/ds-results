using System.Diagnostics;
using LabHelper.DataStructures;

namespace LabHelper.Tools
{
	public class AccurateRealTimeCounter : ITimeCounter
	{
		private Stopwatch stopwatch;

		public void Start()
		{
			if (stopwatch != null)
				throw new LabException("The timer is already running.");

			stopwatch = new Stopwatch();
		    Debug.Assert(Stopwatch.IsHighResolution);

            stopwatch.Start();
		}

		public void Stop()
		{
			if (!stopwatch.IsRunning)
				throw new LabException("The timer is not running.");

			stopwatch.Stop();
		}

		public void Resume()
		{
			if (stopwatch.IsRunning)
				throw new LabException("The timer is already running.");

			stopwatch.Start();
		}

		public TimeInterval Elapsed
		{
			get
			{
				if (stopwatch.IsRunning)
					throw new LabException("The timer is still running.");

				return (TimeInterval)stopwatch.Elapsed;
			}
		}

		public TimeInterval CurrentElapsed
		{
			get { return (TimeInterval)stopwatch.Elapsed; }
		}

		public bool IsRunning
		{
			get { return stopwatch.IsRunning; }
		}

		public static AccurateRealTimeCounter StartNew()
		{
			var counter = new AccurateRealTimeCounter();
			counter.Start();
			return counter;
		}
	}
}
