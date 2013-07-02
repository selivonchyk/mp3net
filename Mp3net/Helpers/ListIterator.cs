namespace Mp3net.Helpers
{
	using System;
	using System.Collections.Generic;

	internal class ListIterator<T>
	{
		private IList<T> list;
		private int pos;

		public ListIterator (IList<T> list, int n)
		{
			this.list = list;
			this.pos = n;
		}

		public bool HasPrevious ()
		{
			return (this.pos > 0);
		}

		public T Previous ()
		{
			pos--;
			return list[pos];
		}
	}
}
