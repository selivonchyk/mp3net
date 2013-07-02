namespace Mp3net
{
	public class ID3v24Frame : ID3v2Frame
	{
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v24Frame(byte[] buffer, int offset) : base(buffer, offset)
		{
		}

		public ID3v24Frame(string id, byte[] data) : base(id, data)
		{
		}

		protected internal override void UnpackDataLength(byte[] buffer, int offset)
		{
			dataLength = BufferTools.UnpackSynchsafeInteger(buffer[offset + DATA_LENGTH_OFFSET
				], buffer[offset + DATA_LENGTH_OFFSET + 1], buffer[offset + DATA_LENGTH_OFFSET +
				 2], buffer[offset + DATA_LENGTH_OFFSET + 3]);
		}

		protected internal override byte[] PackDataLength()
		{
			return BufferTools.PackSynchsafeInteger(dataLength);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.ID3v24Frame))
			{
				return false;
			}
			return base.Equals(obj);
		}
	}
}
