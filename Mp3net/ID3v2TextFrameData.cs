namespace Mp3net
{
	public class ID3v2TextFrameData : AbstractID3v2FrameData
	{
		protected internal EncodedText text;

		public ID3v2TextFrameData(bool unsynchronisation) : base(unsynchronisation)
		{
		}

		public ID3v2TextFrameData(bool unsynchronisation, EncodedText text) : base(unsynchronisation)
		{
			this.text = text;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2TextFrameData(bool unsynchronisation, byte[] bytes) : base(unsynchronisation)
		{
			SynchroniseAndUnpackFrameData(bytes);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal override void UnpackFrameData(byte[] bytes)
		{
			text = new EncodedText(bytes[0], BufferTools.CopyBuffer(bytes, 1, bytes.Length - 1));
		}

		protected internal override byte[] PackFrameData()
		{
			byte[] bytes = new byte[GetLength()];
			if (text != null)
			{
				bytes[0] = text.GetTextEncoding();
				byte[] textBytes = text.ToBytes(true, false);
				if (textBytes.Length > 0)
				{
					BufferTools.CopyIntoByteBuffer(textBytes, 0, textBytes.Length, bytes, 1);
				}
			}
			return bytes;
		}

		protected internal override int GetLength()
		{
			int length = 1;
			if (text != null)
			{
				length += text.ToBytes(true, false).Length;
			}
			return length;
		}

		public virtual EncodedText GetText()
		{
			return text;
		}

		public virtual void SetText(EncodedText text)
		{
			this.text = text;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.ID3v2TextFrameData))
			{
				return false;
			}
			if (!base.Equals(obj))
			{
				return false;
			}
			Mp3net.ID3v2TextFrameData other = (Mp3net.ID3v2TextFrameData)obj;
			if (text == null)
			{
				if (other.text != null)
				{
					return false;
				}
			}
			else
			{
				if (other.text == null)
				{
					return false;
				}
				else
				{
					if (!text.Equals(other.text))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
