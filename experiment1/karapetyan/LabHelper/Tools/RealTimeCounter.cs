using System;
using LabHelper.DataStructures;

namespace LabHelper.Tools
{
	public class RealTimeCounter : ITimeCounter
	{
		private DateTime start, stop;

		public void Start()
		{
			if (start != DateTime.MinValue)
				throw new LabException("Real timer already started.");
			start = DateTime.Now;
		}

		public void Stop()
		{
			stop = DateTime.Now;
		}

		public void Resume()
		{
			start = DateTime.Now - (stop - start);
		}

		public TimeInterval Elapsed
		{
			get { return new TimeInterval((stop - start).Ticks / 10); }
		}

		public TimeInterval CurrentElapsed
		{
			get { return new TimeInterval((DateTime.Now - start).Ticks / 10); }
		}

		public static ITimeCounter StartNew()
		{
			var counter = new RealTimeCounter();
			counter.Start();
			return counter;
		}
	}
}
