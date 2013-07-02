namespace Mp3net
{
	public class ID3v23Tag : AbstractID3v2Tag
	{
		public static readonly string VERSION = "3.0";

		public ID3v23Tag() : base()
		{
			version = VERSION;
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v23Tag(byte[] buffer) : base(buffer)
		{
		}

		protected internal override void UnpackFlags(byte[] buffer)
		{
			unsynchronisation = BufferTools.CheckBit(buffer[FLAGS_OFFSET], UNSYNCHRONISATION_BIT
				);
			extendedHeader = BufferTools.CheckBit(buffer[FLAGS_OFFSET], EXTENDED_HEADER_BIT);
			experimental = BufferTools.CheckBit(buffer[FLAGS_OFFSET], EXPERIMENTAL_BIT);
		}

		protected internal override void PackFlags(byte[] bytes, int offset)
		{
			bytes[offset + FLAGS_OFFSET] = BufferTools.SetBit(bytes[offset + FLAGS_OFFSET], UNSYNCHRONISATION_BIT
				, unsynchronisation);
			bytes[offset + FLAGS_OFFSET] = BufferTools.SetBit(bytes[offset + FLAGS_OFFSET], EXTENDED_HEADER_BIT
				, extendedHeader);
			bytes[offset + FLAGS_OFFSET] = BufferTools.SetBit(bytes[offset + FLAGS_OFFSET], EXPERIMENTAL_BIT
				, experimental);
		}
	}
}
