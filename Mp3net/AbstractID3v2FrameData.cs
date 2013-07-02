namespace Mp3net
{
	public abstract class AbstractID3v2FrameData
	{
		internal bool unsynchronisation;

		public AbstractID3v2FrameData(bool unsynchronisation)
		{
			this.unsynchronisation = unsynchronisation;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal virtual void SynchroniseAndUnpackFrameData(byte[] bytes)
		{
			if (unsynchronisation && BufferTools.SizeSynchronisationWouldSubtract(bytes) > 0)
			{
				byte[] synchronisedBytes = BufferTools.SynchroniseBuffer(bytes);
				UnpackFrameData(synchronisedBytes);
			}
			else
			{
				UnpackFrameData(bytes);
			}
		}

		protected internal virtual byte[] PackAndUnsynchroniseFrameData()
		{
			byte[] bytes = PackFrameData();
			if (unsynchronisation && BufferTools.SizeUnsynchronisationWouldAdd(bytes) > 0)
			{
				return BufferTools.UnsynchroniseBuffer(bytes);
			}
			return bytes;
		}

		protected internal virtual byte[] ToBytes()
		{
			return PackAndUnsynchroniseFrameData();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.AbstractID3v2FrameData))
			{
				return false;
			}
			Mp3net.AbstractID3v2FrameData other = (Mp3net.AbstractID3v2FrameData)obj;
			if (unsynchronisation != other.unsynchronisation)
			{
				return false;
			}
			return true;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal abstract void UnpackFrameData(byte[] bytes);

		protected internal abstract byte[] PackFrameData();

		protected internal abstract int GetLength();
	}
}
