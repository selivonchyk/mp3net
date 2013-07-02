using System;

namespace Mp3net
{
	[System.Serializable]
	public class NotSupportedException : BaseException
	{
		private const long serialVersionUID = 1L;

		public NotSupportedException() : base()
		{
		}

		public NotSupportedException(string message) : base(message)
		{
		}

		public NotSupportedException(string message, Exception cause) : base(message, cause
			)
		{
		}
	}
}
