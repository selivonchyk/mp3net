namespace Mp3net.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public abstract class Iterable<T> : IEnumerable, IEnumerable<T>
	{
		protected Iterable ()
		{
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return this.Iterator ();
		}

		public abstract Iterator<T> Iterator ();
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.Iterator ();
		}
	}
}
