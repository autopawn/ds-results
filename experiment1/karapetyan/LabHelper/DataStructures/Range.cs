using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LabHelper.DataStructures
{
	public class Range : IEnumerable<int>
	{
		private readonly int first;
		private readonly int last;
		private readonly int step;

		public Range(int first, int last, int step)
		{
			this.first = first;
			this.last = last;
			this.step = step;
		}

		public Range(int first, int last)
			: this(first, last, 1)
		{
			
		}

		public Range(int count)
			: this(0, count - 1)
		{
		}

		public IEnumerator<int> GetEnumerator()
		{
			for (int i = first; i <= last; i += step)
				yield return i;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public string ToString(string separator)
		{
			return ToString(separator, i => i.ToString());
		}

		public string ToString(string separator, Func<int, string> select)
		{
			if (last < first)
				return "";

			var builder = new StringBuilder(select(first));
			for (int i = first + step; i <= last; i += step)
				builder.Append(separator + select(i));

			return builder.ToString();
		}
	}
}
