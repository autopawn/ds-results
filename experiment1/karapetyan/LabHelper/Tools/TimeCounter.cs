using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using LabHelper.DataStructures;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace LabHelper.Tools
{
	/// <summary>
	/// Summary description for TimeCounter.
	/// </summary>
	public class TimeCounter : ITimeCounter
	{
		private static ulong FileTimeToLong(FILETIME time)
		{
			ulong h = (uint)time.dwHighDateTime;
			ulong l = (uint)time.dwLowDateTime;
			return (h << 32) | l;
		}

		private static ulong GetCurTime(out FILETIME filetime)
		{
			FILETIME lpCreationTime, lpExitTime, lpKernelTime, lpUserTime;

			if (!GetThreadTimes(
					GetCurrentThread(),
					out lpCreationTime,
					out lpExitTime,
					out lpKernelTime,
					out lpUserTime
					))
			{
				throw new LabException("Failed to run GetThreadTimes().");
			}

			filetime = lpUserTime;
			return FileTimeToLong(lpUserTime);
		}

		[DllImport("Kernel32.dll")]
		private static extern IntPtr GetCurrentThread();

		[DllImport("Kernel32.dll")]
		private static extern bool GetThreadTimes(
			IntPtr hProcess,
			out FILETIME lpCreationTime,
			out FILETIME lpExitTime,
			out FILETIME lpKernelTime,
			out FILETIME lpUserTime
			);

		private ulong start, stop;
		private FILETIME startFt, stopFt;

		public void Start()
		{
			if (start != 0)
				throw new LabException("Already started.");
			stop = 0;
			start = GetCurTime(out startFt);
		}

		public void Stop()
		{
			stop = GetCurTime(out stopFt);
			if (start == 0)
				throw new LabException("The TimeCounter was not started.");
		}

		[Obsolete]
		public double Seconds
		{
			get { return (stop - start) * 1e-7f; }
		}

		public TimeInterval Elapsed
		{
			get
			{
				if (stop == 0)
					throw new LabException("The TimeCounter was not stopped.");

				return ElapsedTo(stop);
			}
		}

		private TimeInterval ElapsedTo(ulong until)
		{
			if (until < start)
				throw new LabException("duration < 0:\nstart {0}, stop {1}.",
					start, until);
			var span = new TimeInterval((long)((until - start + 5) / 10));
			return span;
		}

		public TimeInterval CurrentElapsed
		{
			get
			{
				FILETIME temp;
				return ElapsedTo(GetCurTime(out temp));
			}
		}

		public static TimeInterval Measure(Action action, bool runOnce = false)
		{
			for (int i = 0; ; i++)
			{
				var userTimeCounter = new TimeCounter();
				var stopwatch = new Stopwatch();
				userTimeCounter.Start();
				stopwatch.Start();
				action();
				stopwatch.Stop();
				userTimeCounter.Stop();

				if (userTimeCounter.Seconds < 0.1)
				{
					if (runOnce || stopwatch.ElapsedMilliseconds < 150)
					{
						if (stopwatch.Elapsed.TotalSeconds < 0.000001)
							throw new LabException("Too small running time: {0} sec.", stopwatch.Elapsed.TotalSeconds);
						return TimeInterval.FromSeconds(stopwatch.Elapsed.TotalSeconds);
					}
				}
				else
					return TimeInterval.FromSeconds(userTimeCounter.Seconds);

				if (i == 20)
					throw new LabException("Unsuccessful measurement.");

				Thread.Sleep(1000 * i);
			}
		}

		public void Resume()
		{
			FILETIME curFt;
			ulong cur = GetCurTime(out curFt);

			start = cur - (stop - start);
		}

		public void Reset()
		{
			start = stop = 0;
		}

		public static TimeCounter StartNew()
		{
			var timeCounter = new TimeCounter();
			timeCounter.Start();
			return timeCounter;
		}
	}
}
