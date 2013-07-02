using System;

namespace Mp3net
{
	[System.Serializable]
	public class InvalidDataException : BaseException
	{
		private const long serialVersionUID = 1L;

		public InvalidDataException() : base()
		{
		}

		public InvalidDataException(string message) : base(message)
		{
		}

		public InvalidDataException(string message, Exception cause) : base(message, cause
			)
		{
		}
	}
}
