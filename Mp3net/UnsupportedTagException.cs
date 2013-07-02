using System;

namespace Mp3net
{
	[System.Serializable]
	public class UnsupportedTagException : BaseException
	{
		private const long serialVersionUID = 1L;

		public UnsupportedTagException() : base()
		{
		}

		public UnsupportedTagException(string message) : base(message)
		{
		}

		public UnsupportedTagException(string message, Exception cause) : base(message, cause
			)
		{
		}
	}
}
