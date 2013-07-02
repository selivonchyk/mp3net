using System;
using System.Collections.Generic;
using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2TagTest
	{
		private const byte BYTE_I = unchecked((int)(0x49));

		private const byte BYTE_D = unchecked((int)(0x44));

		private const byte BYTE_3 = unchecked((int)(0x33));

		private static readonly byte[] ID3V2_HEADER = new byte[] { BYTE_I, BYTE_D, BYTE_3, 4, 0, 0, 0, 0, 2, 1 };

        [TestCase]
		public virtual void TestShouldInitialiseFromHeaderBlockWithValidHeaders()
		{
			byte[] header = BufferTools.CopyBuffer(ID3V2_HEADER, 0, ID3V2_HEADER.Length);
			header[3] = 2;
			header[4] = 0;
			ID3v2 id3v2tag = null;
			id3v2tag = CreateTag(header);
			Assert.AreEqual("2.0", id3v2tag.GetVersion());
			header[3] = 3;
			id3v2tag = CreateTag(header);
			Assert.AreEqual("3.0", id3v2tag.GetVersion());
			header[3] = 4;
			id3v2tag = CreateTag(header);
			Assert.AreEqual("4.0", id3v2tag.GetVersion());
		}

        [TestCase]
		public virtual void TestShouldCalculateCorrectDataLengthsFromHeaderBlock()
		{
			byte[] header = BufferTools.CopyBuffer(ID3V2_HEADER, 0, ID3V2_HEADER.Length);
			ID3v2 id3v2tag = CreateTag(header);
			Assert.AreEqual(257, id3v2tag.GetDataLength());
			header[8] = unchecked((int)(0x09));
			header[9] = unchecked((int)(0x41));
			id3v2tag = CreateTag(header);
			Assert.AreEqual(1217, id3v2tag.GetDataLength());
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForNonSupportedVersionInId3v2HeaderBlock()
		{
			byte[] header = BufferTools.CopyBuffer(ID3V2_HEADER, 0, ID3V2_HEADER.Length);
			header[3] = 5;
			header[4] = 0;
			try
			{
				ID3v2TagFactory.CreateTag(header);
				Assert.Fail("UnsupportedTagException expected but not thrown");
			}
			catch (UnsupportedTagException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldSortId3TagsAlphabetically()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v1andv23tags.mp3");
			ID3v2 id3v2tag = ID3v2TagFactory.CreateTag(buffer);
			IDictionary<string, ID3v2FrameSet> frameSets = id3v2tag.GetFrameSets();
			Iterator<ID3v2FrameSet> frameSetIterator = frameSets.Values.Iterator();
			string lastKey = string.Empty;
			while (frameSetIterator.HasNext())
			{
				ID3v2FrameSet frameSet = (ID3v2FrameSet)frameSetIterator.Next();
                Assert.IsTrue(String.CompareOrdinal(frameSet.GetId(), lastKey) > 0);
				lastKey = frameSet.GetId();
			}
		}

        [TestCase]
		public virtual void TestShouldReadFramesFromMp3With32Tag()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v1andv23tags.mp3");
			ID3v2 id3v2tag = ID3v2TagFactory.CreateTag(buffer);
			Assert.AreEqual("3.0", id3v2tag.GetVersion());
			Assert.AreEqual(unchecked((int)(0x44B)), id3v2tag.GetLength());
			Assert.AreEqual(12, id3v2tag.GetFrameSets().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TENC")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("WXXX")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TCOP")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TOPE")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TCOM")).GetFrames().Count);
			Assert.AreEqual(2, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("COMM")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TPE1")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TALB")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TRCK")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TYER")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TCON")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TIT2")).GetFrames().Count);
		}

        [TestCase]
		public virtual void TestShouldReadId3v2WithFooter()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v1andv24tags.mp3");
			ID3v2 id3v2tag = ID3v2TagFactory.CreateTag(buffer);
			Assert.AreEqual("4.0", id3v2tag.GetVersion());
			Assert.AreEqual(unchecked((int)(0x44B)), id3v2tag.GetLength());
		}

        [TestCase]
		public virtual void TestShouldReadTagFieldsFromMp3With32tag()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v1andv23tagswithalbumimage.mp3");
			ID3v2 id3tag = ID3v2TagFactory.CreateTag(buffer);
			Assert.AreEqual("1", id3tag.GetTrack());
			Assert.AreEqual("ARTIST123456789012345678901234", id3tag.GetArtist());
			Assert.AreEqual("TITLE1234567890123456789012345", id3tag.GetTitle());
			Assert.AreEqual("ALBUM1234567890123456789012345", id3tag.GetAlbum());
			Assert.AreEqual("2001", id3tag.GetYear());
			Assert.AreEqual(unchecked((int)(0x0d)), id3tag.GetGenre());
			Assert.AreEqual("Pop", id3tag.GetGenreDescription());
			Assert.AreEqual("COMMENT123456789012345678901", id3tag.GetComment());
			Assert.AreEqual("COMPOSER23456789012345678901234", id3tag.GetComposer());
			Assert.AreEqual("ORIGARTIST234567890123456789012", id3tag.GetOriginalArtist());
			Assert.AreEqual("COPYRIGHT2345678901234567890123", id3tag.GetCopyright());
			Assert.AreEqual("URL2345678901234567890123456789", id3tag.GetUrl());
			Assert.AreEqual("ENCODER234567890123456789012345", id3tag.GetEncoder());
			Assert.AreEqual(1885, id3tag.GetAlbumImage().Length);
			Assert.AreEqual("image/png", id3tag.GetAlbumImageMimeType());
		}

        [TestCase]
		public virtual void TestShouldConvert23TagToBytesAndBackToEquivalentTag()
		{
			ID3v2 id3tag = new ID3v23Tag();
			SetTagFields(id3tag);
			byte[] data = id3tag.ToBytes();
			ID3v2 id3tagCopy = new ID3v23Tag(data);
			Assert.AreEqual(2131, data.Length);
			Assert.AreEqual(id3tag, id3tagCopy);
		}

        [TestCase]
		public virtual void TestShouldConvert24TagWithFooterToBytesAndBackToEquivalentTag()
		{
			ID3v2 id3tag = new ID3v24Tag();
			SetTagFields(id3tag);
			id3tag.SetFooter(true);
			byte[] data = id3tag.ToBytes();
			ID3v2 id3tagCopy = new ID3v24Tag(data);
			Assert.AreEqual(2141, data.Length);
			Assert.AreEqual(id3tag, id3tagCopy);
		}

        [TestCase]
		public virtual void TestShouldConvert24TagWithPaddingToBytesAndBackToEquivalentTag()
		{
			ID3v2 id3tag = new ID3v24Tag();
			SetTagFields(id3tag);
			id3tag.SetPadding(true);
			byte[] data = id3tag.ToBytes();
			ID3v2 id3tagCopy = new ID3v24Tag(data);
			Assert.AreEqual(2131 + AbstractID3v2Tag.PADDING_LENGTH, data.Length);
			Assert.AreEqual(id3tag, id3tagCopy);
		}

        [TestCase]
		public virtual void TestShouldNotUsePaddingOnA24TagIfItHasAFooter()
		{
			ID3v2 id3tag = new ID3v24Tag();
			SetTagFields(id3tag);
			id3tag.SetFooter(true);
			id3tag.SetPadding(true);
			byte[] data = id3tag.ToBytes();
			Assert.AreEqual(2141, data.Length);
		}

        [TestCase]
		public virtual void TestShouldExtractGenreNumberFromCombinedGenreStringsCorrectly()
		{
			ID3v2TagTest.ID3v23TagForTesting id3tag = new ID3v2TagTest.ID3v23TagForTesting(this);
			try
			{
				id3tag.ExtractGenreNumber(string.Empty);
				Assert.Fail("NumberFormatException expected but not thrown");
			}
			catch (FormatException)
			{
			}
			// expected
			Assert.AreEqual(13, id3tag.ExtractGenreNumber("13"));
			Assert.AreEqual(13, id3tag.ExtractGenreNumber("(13)"));
			Assert.AreEqual(13, id3tag.ExtractGenreNumber("(13)Pop"));
		}

        [TestCase]
		public virtual void TestShouldExtractGenreDescriptionFromCombinedGenreStringsCorrectly()
		{
			ID3v2TagTest.ID3v23TagForTesting id3tag = new ID3v2TagTest.ID3v23TagForTesting(this);
			Assert.IsNull(id3tag.ExtractGenreDescription(string.Empty));
			Assert.AreEqual(string.Empty, id3tag.ExtractGenreDescription("(13)"));
			Assert.AreEqual("Pop", id3tag.ExtractGenreDescription("(13)Pop"));
		}

        [TestCase]
		public virtual void TestShouldSetCombinedGenreOnTag()
		{
			ID3v2 id3tag = new ID3v23Tag();
			SetTagFields(id3tag);
			IDictionary<string, ID3v2FrameSet> frameSets = id3tag.GetFrameSets();
			ID3v2FrameSet frameSet = (ID3v2FrameSet)frameSets.Get("TCON");
			IList<ID3v2Frame> frames = frameSet.GetFrames();
			ID3v2Frame frame = (ID3v2Frame)frames[0];
			byte[] bytes = frame.GetData();
			string genre = BufferTools.ByteBufferToString(bytes, 1, bytes.Length - 1);
			Assert.AreEqual("(13)Pop", genre);
		}

        [TestCase]
		public virtual void TestShouldReadCombinedGenreInTag()
		{
			ID3v2 id3tag = new ID3v23Tag();
			SetTagFields(id3tag);
			byte[] bytes = id3tag.ToBytes();
			ID3v2 id3tagFromData = new ID3v23Tag(bytes);
			Assert.AreEqual(13, id3tagFromData.GetGenre());
			Assert.AreEqual("Pop", id3tagFromData.GetGenreDescription());
		}

        [TestCase]
		public virtual void TestShouldGetCommentAndItunesComment()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/withitunescomment.mp3");
			ID3v2 id3tag = ID3v2TagFactory.CreateTag(buffer);
			Assert.AreEqual("COMMENT123456789012345678901", id3tag.GetComment());
			Assert.AreEqual(" 00000A78 00000A74 00000C7C 00000C6C 00000000 00000000 000051F7 00005634 00000000 00000000", id3tag.GetItunesComment());
		}

        [TestCase]
		public virtual void TestShouldReadFramesFromMp3WithObselete32Tag()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/obsolete.mp3");
			ID3v2 id3v2tag = ID3v2TagFactory.CreateTag(buffer);
			Assert.AreEqual("2.0", id3v2tag.GetVersion());
			Assert.AreEqual(unchecked((int)(0x3c5a2)), id3v2tag.GetLength());
			Assert.AreEqual(10, id3v2tag.GetFrameSets().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TCM")).GetFrames().Count);
			Assert.AreEqual(2, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("COM")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TP1")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TAL")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TRK")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TPA")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TYE")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("PIC")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TCO")).GetFrames().Count);
			Assert.AreEqual(1, ((ID3v2FrameSet)id3v2tag.GetFrameSets().Get("TT2")).GetFrames().Count);
		}

        [TestCase]
		public virtual void TestShouldReadTagFieldsFromMp3WithObselete32tag()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/obsolete.mp3");
			ID3v2 id3tag = ID3v2TagFactory.CreateTag(buffer);
			Assert.AreEqual("2009", id3tag.GetYear());
			Assert.AreEqual("4/15", id3tag.GetTrack());
			Assert.AreEqual("image/png", id3tag.GetAlbumImageMimeType());
			Assert.AreEqual(40, id3tag.GetGenre());
			Assert.AreEqual("Alt Rock", id3tag.GetGenreDescription());
			Assert.AreEqual("NAME1234567890123456789012345678901234567890", id3tag.GetTitle());
			Assert.AreEqual("ARTIST1234567890123456789012345678901234567890", id3tag.GetArtist());
			Assert.AreEqual("COMPOSER1234567890123456789012345678901234567890", id3tag.GetComposer());
			Assert.AreEqual("ALBUM1234567890123456789012345678901234567890", id3tag.GetAlbum());
			Assert.AreEqual("COMMENTS1234567890123456789012345678901234567890", id3tag.GetComment());
		}

        [TestCase]
		public virtual void TestShouldReadTagFieldsWithUnicodeDataFromMp3()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v23unicodetags.mp3");
			ID3v2 id3tag = ID3v2TagFactory.CreateTag(buffer);
			Assert.AreEqual("\u03B3\u03B5\u03B9\u03AC \u03C3\u03BF\u03C5", id3tag.GetArtist());
			// greek
			Assert.AreEqual("\u4E2D\u6587", id3tag.GetTitle());
			// chinese
			Assert.AreEqual("\u3053\u3093\u306B\u3061\u306F", id3tag.GetAlbum());
			// japanese
			Assert.AreEqual("\u0AB9\u0AC7\u0AB2\u0ACD\u0AB2\u0ACB", id3tag.GetComposer());
		}

        [TestCase]
		public virtual void TestShouldSetTagFieldsWithUnicodeDataAndSpecifiedEncodingCorrectly()
		{
			ID3v2 id3tag = new ID3v23Tag();
			id3tag.SetArtist("\u03B3\u03B5\u03B9\u03AC \u03C3\u03BF\u03C5");
			id3tag.SetTitle("\u4E2D\u6587");
			id3tag.SetAlbum("\u3053\u3093\u306B\u3061\u306F");
			id3tag.SetComment("\u03C3\u03BF\u03C5");
			id3tag.SetComposer("\u0AB9\u0AC7\u0AB2\u0ACD\u0AB2\u0ACB");
			id3tag.SetOriginalArtist("\u03B3\u03B5\u03B9\u03AC");
			id3tag.SetCopyright("\u03B3\u03B5");
			id3tag.SetUrl("URL");
			id3tag.SetEncoder("\u03B9\u03AC");
			byte[] albumImage = TestHelper.LoadFile("Resources/image.png");
			id3tag.SetAlbumImage(albumImage, "image/png");
		}

        [TestCase]
		public virtual void TestShouldExtractChapterTOCFramesFromMp3()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v23tagwithchapters.mp3");
			ID3v2 id3tag = ID3v2TagFactory.CreateTag(buffer);
			IList<ID3v2ChapterTOCFrameData> chapterTOCs = id3tag.GetChapterTOC();
			Assert.AreEqual(1, chapterTOCs.Count);
			ID3v2ChapterTOCFrameData tocFrameData = chapterTOCs[0];
			Assert.AreEqual("toc1", tocFrameData.GetId());
			string[] expectedChildren = new string[] { "ch1", "ch2", "ch3" };
			Assert.IsTrue(Arrays.Equals(expectedChildren, tocFrameData.GetChilds()));
			IList<ID3v2Frame> subFrames = tocFrameData.GetSubframes();
			Assert.AreEqual(0, subFrames.Count);
		}

        [TestCase]
		public virtual void TestShouldExtractChapterTOCAndChapterFramesFromMp3()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v23tagwithchapters.mp3");
			ID3v2 id3tag = ID3v2TagFactory.CreateTag(buffer);
			IList<ID3v2ChapterFrameData> chapters = id3tag.GetChapters();
			Assert.AreEqual(3, chapters.Count);
			ID3v2ChapterFrameData chapter1 = chapters[0];
			Assert.AreEqual("ch1", chapter1.GetId());
			Assert.AreEqual(0, chapter1.GetStartTime());
			Assert.AreEqual(5000, chapter1.GetEndTime());
			Assert.AreEqual(-1, chapter1.GetStartOffset());
			Assert.AreEqual(-1, chapter1.GetEndOffset());
			IList<ID3v2Frame> subFrames1 = chapter1.GetSubframes();
			Assert.AreEqual(1, subFrames1.Count);
			ID3v2Frame subFrame1 = subFrames1[0];
			Assert.AreEqual("TIT2", subFrame1.GetId());
			ID3v2TextFrameData frameData1 = new ID3v2TextFrameData(false, subFrame1.GetData());
			Assert.AreEqual("start", frameData1.GetText().ToString());
			ID3v2ChapterFrameData chapter2 = chapters[1];
			Assert.AreEqual("ch2", chapter2.GetId());
			Assert.AreEqual(5000, chapter2.GetStartTime());
			Assert.AreEqual(10000, chapter2.GetEndTime());
			Assert.AreEqual(-1, chapter2.GetStartOffset());
			Assert.AreEqual(-1, chapter2.GetEndOffset());
			IList<ID3v2Frame> subFrames2 = chapter2.GetSubframes();
			Assert.AreEqual(1, subFrames2.Count);
			ID3v2Frame subFrame2 = subFrames2[0];
			Assert.AreEqual("TIT2", subFrame2.GetId());
			ID3v2TextFrameData frameData2 = new ID3v2TextFrameData(false, subFrame2.GetData());
			Assert.AreEqual("5 seconds", frameData2.GetText().ToString());
			ID3v2ChapterFrameData chapter3 = chapters[2];
			Assert.AreEqual("ch3", chapter3.GetId());
			Assert.AreEqual(10000, chapter3.GetStartTime());
			Assert.AreEqual(15000, chapter3.GetEndTime());
			Assert.AreEqual(-1, chapter3.GetStartOffset());
			Assert.AreEqual(-1, chapter3.GetEndOffset());
			IList<ID3v2Frame> subFrames3 = chapter3.GetSubframes();
			Assert.AreEqual(1, subFrames3.Count);
			ID3v2Frame subFrame3 = subFrames3[0];
			Assert.AreEqual("TIT2", subFrame3.GetId());
			ID3v2TextFrameData frameData3 = new ID3v2TextFrameData(false, subFrame3.GetData());
			Assert.AreEqual("10 seconds", frameData3.GetText().ToString());
		}

        [TestCase]
		public virtual void TestShouldReadTagFieldsFromMp3With32tagResavedByMp3tagWithUTF16Encoding()
		{
			byte[] buffer = TestHelper.LoadFile("Resources/v1andv23tagswithalbumimage-utf16le.mp3");
			ID3v2 id3tag = ID3v2TagFactory.CreateTag(buffer);
			Assert.AreEqual("1", id3tag.GetTrack());
			Assert.AreEqual("ARTIST123456789012345678901234", id3tag.GetArtist());
			Assert.AreEqual("TITLE1234567890123456789012345", id3tag.GetTitle());
			Assert.AreEqual("ALBUM1234567890123456789012345", id3tag.GetAlbum());
			Assert.AreEqual("2001", id3tag.GetYear());
			Assert.AreEqual(unchecked((int)(0x01)), id3tag.GetGenre());
			Assert.AreEqual("Classic Rock", id3tag.GetGenreDescription());
			Assert.AreEqual("COMMENT123456789012345678901", id3tag.GetComment());
			Assert.AreEqual("COMPOSER23456789012345678901234", id3tag.GetComposer());
			Assert.AreEqual("ORIGARTIST234567890123456789012", id3tag.GetOriginalArtist());
			Assert.AreEqual("COPYRIGHT2345678901234567890123", id3tag.GetCopyright());
			Assert.AreEqual("URL2345678901234567890123456789", id3tag.GetUrl());
			Assert.AreEqual("ENCODER234567890123456789012345", id3tag.GetEncoder());
			Assert.AreEqual(1885, id3tag.GetAlbumImage().Length);
			Assert.AreEqual("image/png", id3tag.GetAlbumImageMimeType());
		}

		private void SetTagFields(ID3v2 id3tag)
		{
			id3tag.SetTrack("1");
			id3tag.SetArtist("ARTIST");
			id3tag.SetTitle("TITLE");
			id3tag.SetAlbum("ALBUM");
			id3tag.SetYear("1954");
			id3tag.SetGenre(unchecked((int)(0x0d)));
			id3tag.SetComment("COMMENT");
			id3tag.SetComposer("COMPOSER");
			id3tag.SetOriginalArtist("ORIGINALARTIST");
			id3tag.SetCopyright("COPYRIGHT");
			id3tag.SetUrl("URL");
			id3tag.SetEncoder("ENCODER");
			byte[] albumImage = TestHelper.LoadFile("Resources/image.png");
			id3tag.SetAlbumImage(albumImage, "image/png");
		}

		private ID3v2 CreateTag(byte[] buffer)
		{
			ID3v2TagTest.ID3v2TagFactoryForTesting factory = new ID3v2TagTest.ID3v2TagFactoryForTesting(this);
			return factory.CreateTag(buffer);
		}

		internal class ID3v22TagForTesting : ID3v22Tag
		{
			public ID3v22TagForTesting(ID3v2TagTest _enclosing, byte[] buffer) : base(buffer)
			{
				this._enclosing = _enclosing;
			}

			protected internal override int UnpackFrames(byte[] buffer, int offset, int framesLength)
			{
				return offset;
			}

			private readonly ID3v2TagTest _enclosing;
		}

		internal class ID3v23TagForTesting : ID3v23Tag
		{
			public ID3v23TagForTesting(ID3v2TagTest _enclosing) : base()
			{
				this._enclosing = _enclosing;
			}

			public ID3v23TagForTesting(ID3v2TagTest _enclosing, byte[] buffer) : base(buffer)
			{
				this._enclosing = _enclosing;
			}

			protected internal override int UnpackFrames(byte[] buffer, int offset, int framesLength)
			{
				return offset;
			}

			private readonly ID3v2TagTest _enclosing;
		}

		internal class ID3v24TagForTesting : ID3v24Tag
		{
			public ID3v24TagForTesting(ID3v2TagTest _enclosing, byte[] buffer) : base(buffer)
			{
				this._enclosing = _enclosing;
			}

			protected internal override int UnpackFrames(byte[] buffer, int offset, int framesLength)
			{
				return offset;
			}

			private readonly ID3v2TagTest _enclosing;
		}

		internal class ID3v2TagFactoryForTesting
		{
			protected internal const int HEADER_LENGTH = 10;

			protected internal static readonly string TAG = "ID3";

			protected internal const int MAJOR_VERSION_OFFSET = 3;

			public virtual ID3v2 CreateTag(byte[] buffer)
			{
				int majorVersion = buffer[ID3v2TagTest.ID3v2TagFactoryForTesting.MAJOR_VERSION_OFFSET];
				switch (majorVersion)
				{
					case 2:
					{
						return new ID3v2TagTest.ID3v22TagForTesting(this._enclosing, buffer);
					}

					case 3:
					{
						return new ID3v2TagTest.ID3v23TagForTesting(this._enclosing, buffer);
					}

					case 4:
					{
						return new ID3v2TagTest.ID3v24TagForTesting(this._enclosing, buffer);
					}
				}
				throw new UnsupportedTagException("Tag version not supported");
			}

			internal ID3v2TagFactoryForTesting(ID3v2TagTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly ID3v2TagTest _enclosing;
		}
	}
}
