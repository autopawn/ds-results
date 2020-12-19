using System;
using System.Diagnostics;
using System.Text;

namespace LabHelper.DataStructures
{
	public struct TimeInterval : IEquatable<TimeInterval>, IComparable<TimeInterval>
	{
		private readonly long usec;

		public TimeInterval(long usec)
		{
			this.usec = usec;
			Debug.Assert(usec >= 0);
		}

		public TimeInterval(TimeSpan interval)
		{
			usec = interval.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
		}

		public static TimeInterval FromSeconds(float seconds)
		{
			checked
			{
				return FromSeconds((double)seconds);
			}
		}

		public static TimeInterval FromSeconds(double seconds)
		{
		    if (double.IsPositiveInfinity(seconds))
		        return Infinity;

			checked
			{
				return new TimeInterval((long)(seconds * 1000000));
			}
		}

		public long Usec
		{
			get { return usec; }
		}

		public double Seconds
		{
			get
			{
				if (IsInfinity)
					return double.NaN;

				return usec / 1000000.0;
			}
		}

		public bool Equals(TimeInterval other)
		{
			return other.usec == usec;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (obj.GetType() != typeof(TimeInterval))
				return false;

			return Equals((TimeInterval)obj);
		}

		public override int GetHashCode()
		{
			return usec.GetHashCode();
		}

		public int CompareTo(TimeInterval other)
		{
			return usec.CompareTo(other.usec);
		}

		public override string ToString()
		{
			if (IsInfinity)
				return "(infinity)";

			return Seconds + " sec.";
		}

		/*
				private static long Pow10(int power)
				{
					long result = 1;
					for (; power > 0; power--)
						result *= 10;

					return result;
				}
		*/

		public TimeInterval Rounded
		{
			get
			{
				if (usec < 10)
					return new TimeInterval(usec);

				if (IsInfinity)
					return this;

				long roundBase = 1;
				long roundCoef = 1;
				while (roundCoef * roundBase * 25 < usec)
				{
					switch (roundCoef)
					{
						case 1:
							roundCoef = 2;
							break;

						case 2:
							roundCoef = 5;
							break;

						case 5:
							roundCoef = 1;
							roundBase *= 10;
							break;
					}
				}

				long round = roundCoef * roundBase;
				return new TimeInterval((usec + round / 2) / round * round);
			}
		}

		public bool IsZero
		{
			get { return usec == 0; }
		}

		public bool IsInfinity
		{
			get { return usec == long.MaxValue; }
		}

		public static TimeInterval Infinity
		{
			get { return new TimeInterval(long.MaxValue); }
		}

		public static TimeInterval Zero
		{
			get { return new TimeInterval(0); }
		}

		public double Minutes
		{
			get { return Seconds / 60; }
		}

		public double Hours
		{
			get { return Minutes / 60; }
		}

	    public string ToSingleNumberString
	    {
	        get
	        {
	            long ms = usec / 1000;
	            if (ms * 1000 != usec)
	                return usec + " us";

	            long sec = ms / 1000;
	            if (sec * 1000 != ms)
	                return ms + " ms";

	            long min = sec / 60;
	            if (min * 60 != sec)
	                return sec + " sec";

	            long hours = min / 60;
	            if (hours * 60 != min)
	                return min + " min";

	            long days = hours / 24;
	            if (days * 24 != hours)
	                return hours + " hours";

	            return days + " days";
	        }
	    }

	    public string HumanReadableTimeToSec
	    {
	        get
	        {
	            int sec = (int)(Seconds + 0.5);
	            var result = $"{sec % 60}s";

	            if (sec >= 60)
	            {
	                int min = sec / 60;
	                result = $"{min % 60}m {result}";

	                if (min >= 60)
	                {
	                    int hours = min / 60;
	                    result = $"{hours % 24}h {result}";

	                    if (hours >= 24)
	                    {
	                        int days = hours / 24;
	                        result = $"{days}d {result}";
	                    }
	                }
	            }

	            return result;
	        }
	    }

	    public static explicit operator TimeSpan(TimeInterval interval)
		{
			return new TimeSpan(interval.usec * 10);
		}

		public static explicit operator TimeInterval(TimeSpan interval)
		{
			return new TimeInterval(interval);
		}

		public static bool operator ==(TimeInterval interval1, TimeInterval interval2)
		{
			return interval1.usec == interval2.usec;
		}

		public static bool operator !=(TimeInterval interval1, TimeInterval interval2)
		{
			return interval1.usec != interval2.usec;
		}

		public static TimeInterval operator *(TimeInterval interval, int coef)
		{
			Debug.Assert(coef >= 0);

		    if (interval.IsInfinity)
		    {
		        if (coef == 0)
		            throw new LabException("Undefined results: infinite times zero.");
		        
                return TimeInterval.Infinity;
		    }

			try
			{
				checked
				{
					return new TimeInterval(interval.usec * coef);
				}
			}
			catch (OverflowException)
			{
				return Infinity;
			}
		}

		public static TimeInterval operator *(int coef, TimeInterval interval)
		{
			return interval * coef;
		}

		public static TimeInterval operator *(TimeInterval interval, double coef)
		{
            Debug.Assert(coef >= 0);

            if (interval.IsInfinity)
            {
                if (coef == 0)
                    throw new LabException("Undefined results: infinite times zero.");

                return TimeInterval.Infinity;
            }

			try
			{
				checked
				{
					return new TimeInterval((long)(interval.usec * coef));
				}
			}
			catch (OverflowException)
			{
				return Infinity;
			}
		}

		public static TimeInterval operator *(double coef, TimeInterval interval)
		{
			return interval * coef;
		}

		public static TimeInterval operator +(TimeInterval interval1, TimeInterval interval2)
		{
			try
			{
				checked
				{
					return new TimeInterval(interval1.usec + interval2.usec);
				}
			}
			catch (OverflowException)
			{
				return Infinity;
			}
		}

		public static TimeInterval operator -(TimeInterval interval1, TimeInterval interval2)
		{
			if (interval1.usec < interval2.usec)
				throw new LabException("The subtraction result is negative.");

		    if (interval1.IsInfinity)
		        return Infinity;
			
			return new TimeInterval(interval1.usec - interval2.usec);
		}

		public static bool operator <(TimeInterval interval1, TimeInterval interval2)
		{
			return interval1.usec < interval2.usec;
		}

		public static bool operator >(TimeInterval interval1, TimeInterval interval2)
		{
			return interval1.usec > interval2.usec;
		}

		public static bool operator <=(TimeInterval interval1, TimeInterval interval2)
		{
			return interval1.usec <= interval2.usec;
		}

		public static bool operator >=(TimeInterval interval1, TimeInterval interval2)
		{
			return interval1.usec >= interval2.usec;
		}

		public static readonly TimeInterval Us = new TimeInterval(1);
		public static readonly TimeInterval Ms = new TimeInterval(1000);
		public static readonly TimeInterval Second = Ms * 1000;
		public static readonly TimeInterval Minute = Second * 60;
		public static readonly TimeInterval Hour = Minute * 60;
		public static readonly TimeInterval Day = Hour * 24;


	}
}
