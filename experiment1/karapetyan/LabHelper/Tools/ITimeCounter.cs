using LabHelper.DataStructures;

namespace LabHelper.Tools
{
	public interface ITimeCounter
	{
		void Start();
		void Stop();
		void Resume();

		TimeInterval Elapsed { get; }
		TimeInterval CurrentElapsed { get; }
	}
}
