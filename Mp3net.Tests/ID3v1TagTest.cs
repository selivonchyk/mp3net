using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v1TagTest
	{
		private string VALID_TAG = "TAGTITLE1234567890123456789012345ARTIST123456789012345678901234ALBUM12345678901234567890123452001COMMENT123456789012345678901234";

		private string VALID_TAG_WITH_WHITESPACE = "TAGTITLE                         ARTIST                        ALBUM                         2001COMMENT                        ";

        [TestCase]
		public virtual void TestShouldThrowExceptionForTagBufferTooShort()
		{
			byte[] buffer = new byte[ID3v1Tag.TAG_LENGTH - 1];
			try
			{
				new ID3v1Tag(buffer);
				Assert.Fail("NoSuchTagException expected but not thrown");
			}
			catch (NoSuchTagException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForTagBufferTooLong()
		{
			byte[] buffer = new byte[ID3v1Tag.TAG_LENGTH + 1];
			try
			{
				new ID3v1Tag(buffer);
				Assert.Fail("NoSuchTagException expected but not thrown");
			}
			catch (NoSuchTagException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldExtractMaximumLengthFieldsFromValid10Tag()
		{
			byte[] buffer = BufferTools.StringToByteBuffer(VALID_TAG, 0, VALID_TAG.Length);
			buffer[buffer.Length - 1] = unchecked((byte)(-unchecked((int)(0x6D))));
			// 0x93 as a signed byte
			ID3v1Tag id3v1tag = new ID3v1Tag(buffer);
			Assert.AreEqual("TITLE1234567890123456789012345", id3v1tag.GetTitle());
			Assert.AreEqual("ARTIST123456789012345678901234", id3v1tag.GetArtist());
			Assert.AreEqual("ALBUM1234567890123456789012345", id3v1tag.GetAlbum());
			Assert.AreEqual("2001", id3v1tag.GetYear());
            Assert.AreEqual("COMMENT12345678901234567890123", id3v1tag.GetComment());
			Assert.AreEqual(null, id3v1tag.GetTrack());
			Assert.AreEqual(unchecked((int)(0x93)), id3v1tag.GetGenre());
			Assert.AreEqual("Synthpop", id3v1tag.GetGenreDescription());
		}

        [TestCase]
		public virtual void TestShouldExtractMaximumLengthFieldsFromValid11Tag()
		{
			byte[] buffer = BufferTools.StringToByteBuffer(VALID_TAG, 0, VALID_TAG.Length);
			buffer[buffer.Length - 3] = unchecked((int)(0x00));
			buffer[buffer.Length - 2] = unchecked((int)(0x01));
			buffer[buffer.Length - 1] = unchecked((int)(0x0D));
			ID3v1Tag id3v1tag = new ID3v1Tag(buffer);
			Assert.AreEqual("TITLE1234567890123456789012345", id3v1tag.GetTitle());
			Assert.AreEqual("ARTIST123456789012345678901234", id3v1tag.GetArtist());
			Assert.AreEqual("ALBUM1234567890123456789012345", id3v1tag.GetAlbum());
			Assert.AreEqual("2001", id3v1tag.GetYear());
			Assert.AreEqual("COMMENT123456789012345678901", id3v1tag.GetComment());
			Assert.AreEqual("1", id3v1tag.GetTrack());
			Assert.AreEqual(unchecked((int)(0x0d)), id3v1tag.GetGenre());
			Assert.AreEqual("Pop", id3v1tag.GetGenreDescription());
		}

        [TestCase]
		public virtual void TestShouldExtractTrimmedFieldsFromValid11TagWithWhitespace()
		{
			byte[] buffer = BufferTools.StringToByteBuffer(VALID_TAG_WITH_WHITESPACE, 0, VALID_TAG_WITH_WHITESPACE.Length);
			buffer[buffer.Length - 3] = unchecked((int)(0x00));
			buffer[buffer.Length - 2] = unchecked((int)(0x01));
			buffer[buffer.Length - 1] = unchecked((int)(0x0D));
			ID3v1Tag id3v1tag = new ID3v1Tag(buffer);
			Assert.AreEqual("TITLE", id3v1tag.GetTitle());
			Assert.AreEqual("ARTIST", id3v1tag.GetArtist());
			Assert.AreEqual("ALBUM", id3v1tag.GetAlbum());
			Assert.AreEqual("2001", id3v1tag.GetYear());
			Assert.AreEqual("COMMENT", id3v1tag.GetComment());
			Assert.AreEqual("1", id3v1tag.GetTrack());
			Assert.AreEqual(unchecked((int)(0x0d)), id3v1tag.GetGenre());
			Assert.AreEqual("Pop", id3v1tag.GetGenreDescription());
		}

        [TestCase]
		public virtual void TestShouldExtractTrimmedFieldsFromValid11TagWithNullspace()
		{
			byte[] buffer = BufferTools.StringToByteBuffer(VALID_TAG_WITH_WHITESPACE, 0, VALID_TAG_WITH_WHITESPACE.Length);
			TestHelper.ReplaceSpacesWithNulls(buffer);
			buffer[buffer.Length - 3] = unchecked((int)(0x00));
			buffer[buffer.Length - 2] = unchecked((int)(0x01));
			buffer[buffer.Length - 1] = unchecked((int)(0x0D));
			ID3v1Tag id3v1tag = new ID3v1Tag(buffer);
			Assert.AreEqual("TITLE", id3v1tag.GetTitle());
			Assert.AreEqual("ARTIST", id3v1tag.GetArtist());
			Assert.AreEqual("ALBUM", id3v1tag.GetAlbum());
			Assert.AreEqual("2001", id3v1tag.GetYear());
			Assert.AreEqual("COMMENT", id3v1tag.GetComment());
			Assert.AreEqual("1", id3v1tag.GetTrack());
			Assert.AreEqual(unchecked((int)(0x0d)), id3v1tag.GetGenre());
			Assert.AreEqual("Pop", id3v1tag.GetGenreDescription());
		}

        [TestCase]
		public virtual void TestShouldGenerateValidTagBuffer()
		{
			ID3v1Tag id3v1tag = new ID3v1Tag();
			id3v1tag.SetTitle("TITLE");
			id3v1tag.SetArtist("ARTIST");
			id3v1tag.SetAlbum("ALBUM");
			id3v1tag.SetYear("2001");
			id3v1tag.SetComment("COMMENT");
			id3v1tag.SetTrack("1");
			id3v1tag.SetGenre(unchecked((int)(0x0d)));
			byte[] expectedBuffer = BufferTools.StringToByteBuffer(VALID_TAG_WITH_WHITESPACE, 0, VALID_TAG_WITH_WHITESPACE.Length);
			TestHelper.ReplaceSpacesWithNulls(expectedBuffer);
			expectedBuffer[expectedBuffer.Length - 3] = unchecked((int)(0x00));
			expectedBuffer[expectedBuffer.Length - 2] = unchecked((int)(0x01));
			expectedBuffer[expectedBuffer.Length - 1] = unchecked((int)(0x0D));
			Assert.IsTrue(Arrays.Equals(expectedBuffer, id3v1tag.ToBytes()));
		}

        [TestCase]
		public virtual void TestShouldGenerateValidTagBufferWithHighGenreAndTrackNumber()
		{
			ID3v1Tag id3v1tag = new ID3v1Tag();
			id3v1tag.SetTitle("TITLE");
			id3v1tag.SetArtist("ARTIST");
			id3v1tag.SetAlbum("ALBUM");
			id3v1tag.SetYear("2001");
			id3v1tag.SetComment("COMMENT");
			id3v1tag.SetTrack("254");
			id3v1tag.SetGenre(unchecked((int)(0x8d)));
			byte[] expectedBuffer = BufferTools.StringToByteBuffer(VALID_TAG_WITH_WHITESPACE, 0, VALID_TAG_WITH_WHITESPACE.Length);
			TestHelper.ReplaceSpacesWithNulls(expectedBuffer);
			expectedBuffer[expectedBuffer.Length - 3] = unchecked((int)(0x00));
			expectedBuffer[expectedBuffer.Length - 2] = unchecked((byte)(-unchecked((int)(0x02))));
			// 254 as a signed byte
			expectedBuffer[expectedBuffer.Length - 1] = unchecked((byte)(-unchecked((int)(0x73))));
			// 0x8D as a signed byte
			Assert.IsTrue(Arrays.Equals(expectedBuffer, id3v1tag.ToBytes()));
		}

        [TestCase]
		public virtual void TestShouldReadTagFieldsFromMp3()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v1andv23tags.mp3");
			byte[] tagBuffer = BufferTools.CopyBuffer(buffer, buffer.Length - ID3v1Tag.TAG_LENGTH, ID3v1Tag.TAG_LENGTH);
			ID3v1 id3tag = new ID3v1Tag(tagBuffer);
			Assert.AreEqual("1", id3tag.GetTrack());
			Assert.AreEqual("ARTIST123456789012345678901234", id3tag.GetArtist());
			Assert.AreEqual("TITLE1234567890123456789012345", id3tag.GetTitle());
			Assert.AreEqual("ALBUM1234567890123456789012345", id3tag.GetAlbum());
			Assert.AreEqual("2001", id3tag.GetYear());
			Assert.AreEqual(unchecked((int)(0x0d)), id3tag.GetGenre());
			Assert.AreEqual("Pop", id3tag.GetGenreDescription());
			Assert.AreEqual("COMMENT123456789012345678901", id3tag.GetComment());
		}

        [TestCase]
		public virtual void TestShouldConvertTagToBytesAndBackToEquivalentTag()
		{
			ID3v1 id3tag = new ID3v1Tag();
			id3tag.SetTrack("5");
			id3tag.SetArtist("ARTIST");
			id3tag.SetTitle("TITLE");
			id3tag.SetAlbum("ALBUM");
			id3tag.SetYear("1997");
			id3tag.SetGenre(13);
			id3tag.SetComment("COMMENT");
			byte[] data = id3tag.ToBytes();
			ID3v1 id3tagCopy = new ID3v1Tag(data);
			Assert.AreEqual(id3tag, id3tagCopy);
		}

        [TestCase]
		public virtual void TestShouldReturnEmptyTrackIfNotSetOn11Tag()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v1tagwithnotrack.mp3");
			byte[] tagBuffer = BufferTools.CopyBuffer(buffer, buffer.Length - ID3v1Tag.TAG_LENGTH, ID3v1Tag.TAG_LENGTH);
			ID3v1 id3tag = new ID3v1Tag(tagBuffer);
			Assert.AreEqual(string.Empty, id3tag.GetTrack());
		}
	}
}
