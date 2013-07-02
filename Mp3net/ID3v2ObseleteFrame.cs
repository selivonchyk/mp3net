namespace Mp3net
{
	public class ID3v2ObseleteFrame : ID3v2Frame
	{
		private const int HEADER_LENGTH = 6;

		private const int ID_OFFSET = 0;

		private const int ID_LENGTH = 3;

		protected internal const int DATA_LENGTH_OFFSET = 3;

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2ObseleteFrame(byte[] buffer, int offset) : base(buffer, offset)
		{
		}

		public ID3v2ObseleteFrame(string id, byte[] data) : base(id, data)
		{
		}

		protected internal override int UnpackHeader(byte[] buffer, int offset)
		{
			id = BufferTools.ByteBufferToStringIgnoringEncodingIssues(buffer, offset + ID_OFFSET
				, ID_LENGTH);
			UnpackDataLength(buffer, offset);
			return offset + HEADER_LENGTH;
		}

		protected internal override void UnpackDataLength(byte[] buffer, int offset)
		{
			dataLength = BufferTools.UnpackInteger(unchecked((byte)0), buffer[offset + DATA_LENGTH_OFFSET
				], buffer[offset + DATA_LENGTH_OFFSET + 1], buffer[offset + DATA_LENGTH_OFFSET +
				 2]);
		}

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		public override void PackFrame(byte[] bytes, int offset)
		{
			throw (new NotSupportedException("Packing Obselete frames is not supported"));
		}

		public override int GetLength()
		{
			return dataLength + HEADER_LENGTH;
		}
	}
}
