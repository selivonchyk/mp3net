using System;

namespace Mp3net
{
	[System.Serializable]
	public class NoSuchTagException : BaseException
	{
		private const long serialVersionUID = 1L;

		public NoSuchTagException() : base()
		{
		}

		public NoSuchTagException(string message) : base(message)
		{
		}

		public NoSuchTagException(string message, Exception cause) : base(message, cause)
		{
		}
	}
}
