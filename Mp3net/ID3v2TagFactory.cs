namespace Mp3net
{
	public class ID3v2TagFactory
	{
		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public static AbstractID3v2Tag CreateTag(byte[] bytes)
		{
			SanityCheckTag(bytes);
			int majorVersion = bytes[AbstractID3v2Tag.MAJOR_VERSION_OFFSET];
			switch (majorVersion)
			{
				case 2:
				{
					return CreateID3v22Tag(bytes);
				}

				case 3:
				{
					return new ID3v23Tag(bytes);
				}

				case 4:
				{
					return new ID3v24Tag(bytes);
				}
			}
			throw new UnsupportedTagException("Tag version not supported");
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private static AbstractID3v2Tag CreateID3v22Tag(byte[] bytes)
		{
			ID3v22Tag tag = new ID3v22Tag(bytes);
			if (tag.GetFrameSets().Count == 0)
			{
				tag = new ID3v22Tag(bytes, true);
			}
			return tag;
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		public static void SanityCheckTag(byte[] bytes)
		{
			if (bytes.Length < AbstractID3v2Tag.HEADER_LENGTH)
			{
				throw new NoSuchTagException("Buffer too short");
			}
			if (!AbstractID3v2Tag.TAG.Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues
				(bytes, 0, AbstractID3v2Tag.TAG.Length)))
			{
				throw new NoSuchTagException();
			}
			int majorVersion = bytes[AbstractID3v2Tag.MAJOR_VERSION_OFFSET];
			if (majorVersion != 2 && majorVersion != 3 && majorVersion != 4)
			{
				int minorVersion = bytes[AbstractID3v2Tag.MINOR_VERSION_OFFSET];
				throw new UnsupportedTagException("Unsupported version 2." + majorVersion + "." +
					 minorVersion);
			}
		}
	}
}
