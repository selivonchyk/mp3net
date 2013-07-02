namespace Mp3net
{
	public class ID3v22Tag : AbstractID3v2Tag
	{
		public static readonly string VERSION = "2.0";

		public ID3v22Tag() : base()
		{
			version = VERSION;
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v22Tag(byte[] buffer) : base(buffer)
		{
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v22Tag(byte[] buffer, bool obseleteFormat) : base(buffer, obseleteFormat
			)
		{
		}

		protected internal override void UnpackFlags(byte[] bytes)
		{
			unsynchronisation = BufferTools.CheckBit(bytes[FLAGS_OFFSET], UNSYNCHRONISATION_BIT
				);
			compression = BufferTools.CheckBit(bytes[FLAGS_OFFSET], COMPRESSION_BIT);
		}

		protected internal override void PackFlags(byte[] bytes, int offset)
		{
			bytes[offset + FLAGS_OFFSET] = BufferTools.SetBit(bytes[offset + FLAGS_OFFSET], UNSYNCHRONISATION_BIT
				, unsynchronisation);
			bytes[offset + FLAGS_OFFSET] = BufferTools.SetBit(bytes[offset + FLAGS_OFFSET], COMPRESSION_BIT
				, compression);
		}
	}
}
