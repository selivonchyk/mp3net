using Mp3net.Helpers;

namespace Mp3net
{
	public class MutableInteger
	{
		private int value;

		public MutableInteger(int value)
		{
			this.value = value;
		}

		public virtual void Increment()
		{
			value++;
		}

		public virtual int GetValue()
		{
			return value;
		}

		public virtual void SetValue(int value)
		{
			this.value = value;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.MutableInteger))
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			Mp3net.MutableInteger other = (Mp3net.MutableInteger)obj;
			if (value != other.value)
			{
				return false;
			}
			return true;
		}
	}
}
