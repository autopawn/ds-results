using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Collections;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using LabHelper.DataStructures;

namespace LabHelper.Tools
{
	public static class Helper
	{
		public static int Random(Random random, int lessThan, int except)
		{
			int result = random.Next(lessThan - 1);

			result = result == except
				? lessThan - 1
				: result;

			Debug.Assert(result != except);
			Debug.Assert(result >= 0);
			Debug.Assert(result < lessThan);
			return result;
		}

		public static int Except(this Random random, int lessThan, int except)
		{
			Debug.Assert(lessThan > 1);

			int result = random.Next(lessThan - 1);

			result = result == except
				? lessThan - 1
				: result;

			Debug.Assert(result != except);
			Debug.Assert(result >= 0);
			Debug.Assert(result < lessThan);
			return result;
		}

		public static int Random(Random random, int lessThan, int except1, int except2)
		{
			if (except1 == except2)
				return Random(random, lessThan, except1);

			if (except1 == lessThan - 1)
				return Random(random, lessThan - 1, except2);

			int result1 = Random(random, lessThan - 1, except1);
			int result2 = result1 == except2
				? lessThan - 1
				: result1;

			Debug.Assert(result2 != except1);
			Debug.Assert(result2 != except2);
			Debug.Assert(result2 >= 0);
			Debug.Assert(result2 < lessThan);
			return result2;
		}

		public static int Except(this Random random, int lessThan, int except1, int except2)
		{
			return Random(random, lessThan, except1, except2);
		}

		public static T Random<T>(this IEnumerable<T> list, Random random)
		{
			Debug.Assert(list.Any());
			var count = list.Count();
			int index = random.Next(count);
			return list.ElementAt(index);
		}

		public static T Random<T>(this List<T> list, Random random)
		{
			Debug.Assert(list.Count > 0);
			var count = list.Count;
			int index = random.Next(count);
			return list[index];
		}

	
		private static CultureInfo culture;

		static Helper()
		{
			culture = CultureInfo.CreateSpecificCulture("en-GB");
			culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
			Console.OutputEncoding = Encoding.UTF8;

			try
			{
				Console.SetBufferSize(210, 10000);
			}
			catch
			{
				return;
			}

			for (int height = 70; height >= 30; height--)
			{
				try
				{
					Console.SetWindowSize(210, height);
					break;
				}
				catch
				{
				}
			}
		}
		
		public static readonly char[] SpaceAndTab = new[] { ' ', '\t' };
		
		public static int[] DistinctUnordered(this Random random, int lessThan, int numberOfElements)
		{
			Debug.Assert(numberOfElements <= lessThan);
			switch (numberOfElements)
			{
				case 0:
					return new int[0];

				case 1:
					return new[] { random.Next(lessThan) };

				case 2:
				{
					var result = new int[2];
					result[0] = random.Next(lessThan);
					result[1] = random.Except(lessThan, result[0]);
					return result;
				}

				case 3:
				{
					var result = new int[3];
					result[0] = random.Next(lessThan);
					result[1] = random.Except(lessThan, result[0]);
					result[2] = random.Except(lessThan, result[0], result[1]);
					return result;
				}

				default:
				{
					if (2 * numberOfElements >= lessThan)
					{ // O(numberOfElements * lessThan) iterations; O(numberOfElements + lessThan) memory
						var available = new Range(lessThan).ToList();
						var result = new int[numberOfElements];
						for (int i = 0; i < numberOfElements; i++)
						{
							int index = random.Next(available.Count);
							result[i] = available[index];
							available.RemoveAt(index);
						}

						return result;
					}
					else
					{ // O(numberOfElements^2) iterations if (numberOfElements / lessThan < const < 1); O(numberOfElements) memory
						var result = new int[numberOfElements];
						for (int i = 0; i < numberOfElements; i++)
						{
							int value;
							do
							{
								value = random.Next(lessThan);
							} while (Array.IndexOf(result, value, 0, i) >= 0);

							result[i] = value;
						}

						return result.ToArray();
					}
				}
			}
		}


		public static T[] FillArray<T>(this T fillWith, int length)
		{
			var result = new T[length];
			for (int i = result.Length - 1; i >= 0; i--)
				result[i] = fillWith;

			return result;
		}
	}
}
