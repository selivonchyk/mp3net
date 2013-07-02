using System;

namespace Mp3net.Helpers
{
	internal class BufferUnderflowException : Exception
	{
	}

	public class CharacterCodingException : Exception
	{
	}

	public class EOFException : Exception
	{
		public EOFException ()
		{
		}

		public EOFException (string msg) : base(msg)
		{
		}
	}

    public class NoSuchElementException : Exception
	{
	}

	internal class UnsupportedEncodingException : Exception
	{
	}
	
	class UnsupportedCharsetException: Exception
	{
		public UnsupportedCharsetException (string msg): base (msg)
		{
		}
	}
}

