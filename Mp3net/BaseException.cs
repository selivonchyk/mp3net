using System;
using System.Text;

namespace Mp3net
{
	[System.Serializable]
	public class BaseException : Exception
	{
		private const long serialVersionUID = 1L;

		public BaseException() : base()
		{
		}

		public BaseException(string message) : base(message)
		{
		}

		public BaseException(string message, Exception cause) : base(message, cause)
		{
		}

		public virtual string GetDetailedMessage()
		{
			Exception t = this;
			StringBuilder s = new StringBuilder();
			while (true)
			{
				s.Append('[');
				s.Append(t.GetType().FullName);
				if (t.Message != null && t.Message.Length > 0)
				{
					s.Append(": ");
					s.Append(t.Message);
				}
				s.Append(']');
				t = t.InnerException;
				if (t != null)
				{
					s.Append(" caused by ");
				}
				else
				{
					break;
				}
			}
			return s.ToString();
		}
	}
}
