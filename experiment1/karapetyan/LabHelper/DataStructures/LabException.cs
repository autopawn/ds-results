using System;

namespace LabHelper.DataStructures
{
	public class LabException : Exception
    {
        public LabException()
        {
        }

        public LabException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }
    }
}
