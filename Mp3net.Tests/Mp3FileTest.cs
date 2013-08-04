using System;
using System.IO;
using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class Mp3FileTest
	{
		private static readonly string MP3_WITH_NO_TAGS = "Resources/notags.mp3";

		private static readonly string MP3_WITH_ID3V1_AND_ID3V23_TAGS = "Resources/v1andv23tags.mp3";

		private static readonly string MP3_WITH_DUMMY_START_AND_END_FRAMES = "Resources/dummyframes.mp3";

		private static readonly string MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS = "Resources/v1andv23andcustomtags.mp3";

		private static readonly string MP3_WITH_ID3V23_UNICODE_TAGS = "Resources/v23unicodetags.mp3";

		private static readonly string NOT_AN_MP3 = "Resources/notanmp3.mp3";

		private static readonly string MP3_WITH_INCOMPLETE_MPEG_FRAME = "Resources/incompletempegframe.mp3";

        private static readonly string VALID_FILENAME = "Resources/notags.mp3";

        private const long VALID_FILE_LENGTH = 2869;

        private static readonly string NON_EXISTANT_FILENAME = "just-not.there";

        private static readonly string MALFORMED_FILENAME = "malformed.?";

        [TestCase]
        public virtual void TestShouldReadValidFile()
        {
            Mp3File mp3File = new Mp3File(VALID_FILENAME);
            Assert.AreEqual(mp3File.GetFilename(), VALID_FILENAME);
            Assert.IsTrue(mp3File.GetLastModified().ToFileTime() > 0);
            Assert.AreEqual(mp3File.GetLength(), VALID_FILE_LENGTH);
        }

        [TestCase]
        public virtual void TestShouldFailForNonExistentFile()
        {
            try
            {
                new Mp3File(NON_EXISTANT_FILENAME);
                Assert.Fail("FileNotFoundException expected but not thrown");
            }
            catch (FileNotFoundException)
            {
            }
        }

        [TestCase]
        public virtual void TestShouldFailForMalformedFilename()
        {
            try
            {
                new Mp3File(MALFORMED_FILENAME);
                Assert.Fail("ArgumentException expected but not thrown");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestCase]
        public virtual void TestShouldFailForNullFilename()
        {
            try
            {
                new Mp3File((String)null);
                Assert.Fail("NullPointerException expected but not thrown");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestCase]
        public virtual void TestShouldFailForNullStream()
        {
            try
            {
                new Mp3File((Stream)null);
                Assert.Fail("NullPointerException expected but not thrown");
            }
            catch (ArgumentNullException)
            {
            }
        }	

        [TestCase]
		public virtual void TestShouldLoadMp3WithNoTags()
		{
			LoadAndCheckTestMp3WithNoTags(MP3_WITH_NO_TAGS, 41);
			LoadAndCheckTestMp3WithNoTags(MP3_WITH_NO_TAGS, 256);
			LoadAndCheckTestMp3WithNoTags(MP3_WITH_NO_TAGS, 1024);
			LoadAndCheckTestMp3WithNoTags(MP3_WITH_NO_TAGS, 5000);
		}

        [TestCase]
		public virtual void TestShouldLoadMp3WithId3Tags()
		{
			LoadAndCheckTestMp3WithTags(MP3_WITH_ID3V1_AND_ID3V23_TAGS, 41);
			LoadAndCheckTestMp3WithTags(MP3_WITH_ID3V1_AND_ID3V23_TAGS, 256);
			LoadAndCheckTestMp3WithTags(MP3_WITH_ID3V1_AND_ID3V23_TAGS, 1024);
			LoadAndCheckTestMp3WithTags(MP3_WITH_ID3V1_AND_ID3V23_TAGS, 5000);
		}

        [TestCase]
		public virtual void TestShouldLoadMp3WithFakeStartAndEndFrames()
		{
			LoadAndCheckTestMp3WithTags(MP3_WITH_DUMMY_START_AND_END_FRAMES, 41);
			LoadAndCheckTestMp3WithTags(MP3_WITH_DUMMY_START_AND_END_FRAMES, 256);
			LoadAndCheckTestMp3WithTags(MP3_WITH_DUMMY_START_AND_END_FRAMES, 1024);
			LoadAndCheckTestMp3WithTags(MP3_WITH_DUMMY_START_AND_END_FRAMES, 5000);
		}

        [TestCase]
		public virtual void TestShouldLoadMp3WithCustomTag()
		{
			LoadAndCheckTestMp3WithCustomTag(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS, 41);
			LoadAndCheckTestMp3WithCustomTag(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS, 256);
			LoadAndCheckTestMp3WithCustomTag(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS, 1024);
			LoadAndCheckTestMp3WithCustomTag(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS, 5000);
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForFileThatIsNotAnMp3()
		{
			try
			{
				new Mp3File(NOT_AN_MP3);
				Assert.Fail("InvalidDataException expected but not thrown");
			}
			catch (InvalidDataException e)
			{
				Assert.AreEqual("No mpegs frames found", e.Message);
			}
		}

        [TestCase]
		public virtual void TestShouldFindProbableStartOfMpegFramesWithPrescan()
		{
			Mp3FileTest.Mp3FileForTesting mp3file = new Mp3FileTest.Mp3FileForTesting(this, MP3_WITH_ID3V1_AND_ID3V23_TAGS);
			Assert.AreEqual(unchecked((int)(0x44B)), mp3file.preScanResult);
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionIfSavingMp3WithSameNameAsSourceFile()
		{
			Mp3File mp3file = new Mp3File(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS);
			try
			{
				mp3file.Save(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS);
				Assert.Fail("IllegalArgumentException expected but not thrown");
			}
			catch (ArgumentException e)
			{
				Assert.AreEqual("Save filename same as source filename", e.Message);
			}
		}

        [TestCase]
		public virtual void TestShouldSaveLoadedMp3WhichIsEquivalentToOriginal()
		{
			CopyAndCheckTestMp3WithCustomTag(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS, 41);
			CopyAndCheckTestMp3WithCustomTag(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS, 256);
			CopyAndCheckTestMp3WithCustomTag(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS, 1024);
			CopyAndCheckTestMp3WithCustomTag(MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS, 5000);
		}

        [TestCase]
		public virtual void TestShouldLoadAndCheckMp3ContainingUnicodeFields()
		{
			LoadAndCheckTestMp3WithUnicodeFields(MP3_WITH_ID3V23_UNICODE_TAGS, 41);
			LoadAndCheckTestMp3WithUnicodeFields(MP3_WITH_ID3V23_UNICODE_TAGS, 256);
			LoadAndCheckTestMp3WithUnicodeFields(MP3_WITH_ID3V23_UNICODE_TAGS, 1024);
			LoadAndCheckTestMp3WithUnicodeFields(MP3_WITH_ID3V23_UNICODE_TAGS, 5000);
		}

        [TestCase]
		public virtual void TestShouldSaveLoadedMp3WithUnicodeFieldsWhichIsEquivalentToOriginal()
		{
			CopyAndCheckTestMp3WithUnicodeFields(MP3_WITH_ID3V23_UNICODE_TAGS, 41);
			CopyAndCheckTestMp3WithUnicodeFields(MP3_WITH_ID3V23_UNICODE_TAGS, 256);
			CopyAndCheckTestMp3WithUnicodeFields(MP3_WITH_ID3V23_UNICODE_TAGS, 1024);
			CopyAndCheckTestMp3WithUnicodeFields(MP3_WITH_ID3V23_UNICODE_TAGS, 5000);
		}

        [TestCase]
		public virtual void TestShouldIgnoreIncompleteMpegFrame()
		{
			Mp3File mp3File = new Mp3File(MP3_WITH_INCOMPLETE_MPEG_FRAME, 256);
			Assert.AreEqual(unchecked((int)(0x44B)), mp3File.GetXingOffset());
			Assert.AreEqual(unchecked((int)(0x5EC)), mp3File.GetStartOffset());
			Assert.AreEqual(unchecked((int)(0xF17)), mp3File.GetEndOffset());
			Assert.IsTrue(mp3File.HasId3v1Tag());
			Assert.IsTrue(mp3File.HasId3v2Tag());
			Assert.AreEqual(5, mp3File.GetFrameCount());
		}

        [TestCase]
		public virtual void TestShouldInitialiseProperlyWhenNotScanningFile()
		{
			Mp3File mp3File = new Mp3File(MP3_WITH_INCOMPLETE_MPEG_FRAME, 256, false);
			Assert.IsTrue(mp3File.HasId3v1Tag());
			Assert.IsTrue(mp3File.HasId3v2Tag());
		}

        [TestCase]
		public virtual void TestShouldRemoveId3v1Tag()
		{
			string filename = MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS;
			string saveFilename = filename + ".copy";
			try
			{
				Mp3File mp3File = new Mp3File(filename);
				mp3File.RemoveId3v1Tag();
				mp3File.Save(saveFilename);
				Mp3File newMp3File = new Mp3File(saveFilename);
				Assert.IsFalse(newMp3File.HasId3v1Tag());
				Assert.IsTrue(newMp3File.HasId3v2Tag());
				Assert.IsTrue(newMp3File.HasCustomTag());
			}
			finally
			{
				TestHelper.DeleteFile(saveFilename);
			}
		}

        [TestCase]
		public virtual void TestShouldRemoveId3v2Tag()
		{
			string filename = MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS;
			string saveFilename = filename + ".copy";
			try
			{
				Mp3File mp3File = new Mp3File(filename);
				mp3File.RemoveId3v2Tag();
				mp3File.Save(saveFilename);
				Mp3File newMp3File = new Mp3File(saveFilename);
				Assert.IsTrue(newMp3File.HasId3v1Tag());
				Assert.IsFalse(newMp3File.HasId3v2Tag());
				Assert.IsTrue(newMp3File.HasCustomTag());
			}
			finally
			{
				TestHelper.DeleteFile(saveFilename);
			}
		}

        [TestCase]
		public virtual void TestShouldRemoveCustomTag()
		{
			string filename = MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS;
			string saveFilename = filename + ".copy";
			try
			{
				Mp3File mp3File = new Mp3File(filename);
				mp3File.RemoveCustomTag();
				mp3File.Save(saveFilename);
				Mp3File newMp3File = new Mp3File(saveFilename);
				Assert.IsTrue(newMp3File.HasId3v1Tag());
				Assert.IsTrue(newMp3File.HasId3v2Tag());
				Assert.IsFalse(newMp3File.HasCustomTag());
			}
			finally
			{
				TestHelper.DeleteFile(saveFilename);
			}
		}


        [TestCase]
		public virtual void TestShouldRemoveId3v1AndId3v2AndCustomTags()
		{
			string filename = MP3_WITH_ID3V1_AND_ID3V23_AND_CUSTOM_TAGS;
			string saveFilename = filename + ".copy";
			try
			{
				Mp3File mp3File = new Mp3File(filename);
				mp3File.RemoveId3v1Tag();
				mp3File.RemoveId3v2Tag();
				mp3File.RemoveCustomTag();
				mp3File.Save(saveFilename);
				Mp3File newMp3File = new Mp3File(saveFilename);
				Assert.IsFalse(newMp3File.HasId3v1Tag());
				Assert.IsFalse(newMp3File.HasId3v2Tag());
				Assert.IsFalse(newMp3File.HasCustomTag());
			}
			finally
			{
				TestHelper.DeleteFile(saveFilename);
			}
		}

		private Mp3File CopyAndCheckTestMp3WithCustomTag(string filename, int bufferLength)
		{
			string saveFilename = null;
			try
			{
				Mp3File mp3file = LoadAndCheckTestMp3WithCustomTag(filename, bufferLength);
				saveFilename = filename + ".copy";
				mp3file.Save(saveFilename);
				Mp3File copyMp3file = LoadAndCheckTestMp3WithCustomTag(saveFilename, 5000);
				Assert.AreEqual(mp3file.GetId3v1Tag(), copyMp3file.GetId3v1Tag());
				Assert.AreEqual(mp3file.GetId3v2Tag(), copyMp3file.GetId3v2Tag());
				Assert.IsTrue(Arrays.Equals(mp3file.GetCustomTag(), copyMp3file.GetCustomTag()));
				return copyMp3file;
			}
			finally
			{
				TestHelper.DeleteFile(saveFilename);
			}
		}

		private Mp3File CopyAndCheckTestMp3WithUnicodeFields(string filename, int bufferLength)
		{
			string saveFilename = null;
			try
			{
				Mp3File mp3file = LoadAndCheckTestMp3WithUnicodeFields(filename, bufferLength);
				saveFilename = filename + ".copy";
				mp3file.Save(saveFilename);
				Mp3File copyMp3file = LoadAndCheckTestMp3WithUnicodeFields(saveFilename, 5000);
				Assert.AreEqual(mp3file.GetId3v2Tag(), copyMp3file.GetId3v2Tag());
				return copyMp3file;
			}
			finally
			{
				TestHelper.DeleteFile(saveFilename);
			}
		}

		private Mp3File LoadAndCheckTestMp3WithNoTags(string filename, int bufferLength)
		{
			Mp3File mp3File = LoadAndCheckTestMp3(filename, bufferLength);
			Assert.AreEqual(unchecked((int)(0x000)), mp3File.GetXingOffset());
			Assert.AreEqual(unchecked((int)(0x1A1)), mp3File.GetStartOffset());
			Assert.AreEqual(unchecked((int)(0xB34)), mp3File.GetEndOffset());
			Assert.IsFalse(mp3File.HasId3v1Tag());
			Assert.IsFalse(mp3File.HasId3v2Tag());
			Assert.IsFalse(mp3File.HasCustomTag());
			return mp3File;
		}

		private Mp3File LoadAndCheckTestMp3WithTags(string filename, int bufferLength)
		{
			Mp3File mp3File = LoadAndCheckTestMp3(filename, bufferLength);
			Assert.AreEqual(unchecked((int)(0x44B)), mp3File.GetXingOffset());
			Assert.AreEqual(unchecked((int)(0x5EC)), mp3File.GetStartOffset());
			Assert.AreEqual(unchecked((int)(0xF7F)), mp3File.GetEndOffset());
			Assert.IsTrue(mp3File.HasId3v1Tag());
			Assert.IsTrue(mp3File.HasId3v2Tag());
			Assert.IsFalse(mp3File.HasCustomTag());
			return mp3File;
		}

		private Mp3File LoadAndCheckTestMp3WithUnicodeFields(string filename, int bufferLength)
		{
			Mp3File mp3File = LoadAndCheckTestMp3(filename, bufferLength);
			Assert.AreEqual(unchecked((int)(0x0CA)), mp3File.GetXingOffset());
			Assert.AreEqual(unchecked((int)(0x26B)), mp3File.GetStartOffset());
			Assert.AreEqual(unchecked((int)(0xBFE)), mp3File.GetEndOffset());
			Assert.IsFalse(mp3File.HasId3v1Tag());
			Assert.IsTrue(mp3File.HasId3v2Tag());
			Assert.IsFalse(mp3File.HasCustomTag());
			return mp3File;
		}

		private Mp3File LoadAndCheckTestMp3WithCustomTag(string filename, int bufferLength)
		{
			Mp3File mp3File = LoadAndCheckTestMp3(filename, bufferLength);
			Assert.AreEqual(unchecked((int)(0x44B)), mp3File.GetXingOffset());
			Assert.AreEqual(unchecked((int)(0x5EC)), mp3File.GetStartOffset());
			Assert.AreEqual(unchecked((int)(0xF7F)), mp3File.GetEndOffset());
			Assert.IsTrue(mp3File.HasId3v1Tag());
			Assert.IsTrue(mp3File.HasId3v2Tag());
			Assert.IsTrue(mp3File.HasCustomTag());
			return mp3File;
		}

		private Mp3File LoadAndCheckTestMp3(string filename, int bufferLength)
		{
			Mp3File mp3File = new Mp3File(filename, bufferLength);
			Assert.IsTrue(mp3File.HasXingFrame());
			Assert.AreEqual(6, mp3File.GetFrameCount());
			Assert.AreEqual(MpegFrame.MPEG_VERSION_1_0, mp3File.GetVersion());
			Assert.AreEqual(MpegFrame.MPEG_LAYER_3, mp3File.GetLayer());
			Assert.AreEqual(44100, mp3File.GetSampleRate());
			Assert.AreEqual(MpegFrame.CHANNEL_MODE_JOINT_STEREO, mp3File.GetChannelMode());
			Assert.AreEqual(MpegFrame.EMPHASIS_NONE, mp3File.GetEmphasis());
			Assert.IsTrue(mp3File.IsOriginal());
			Assert.IsFalse(mp3File.IsCopyright());
			Assert.AreEqual(128, mp3File.GetXingBitrate());
			Assert.AreEqual(125, mp3File.GetBitrate());
            Assert.AreEqual(1, ((MutableInteger)mp3File.GetBitrates()[224]).GetValue());
			Assert.AreEqual(1, ((MutableInteger)mp3File.GetBitrates()[112]).GetValue());
			Assert.AreEqual(2, ((MutableInteger)mp3File.GetBitrates()[96]).GetValue());
			Assert.AreEqual(1, ((MutableInteger)mp3File.GetBitrates()[192]).GetValue());
			Assert.AreEqual(1, ((MutableInteger)mp3File.GetBitrates()[32]).GetValue());
			Assert.AreEqual(156, mp3File.GetLengthInMilliseconds());
			return mp3File;
		}

		private class Mp3FileForTesting : Mp3File
		{
			internal int preScanResult;

			/// <exception cref="System.IO.IOException"></exception>
			public Mp3FileForTesting(Mp3FileTest _enclosing, string filename)
			{
				this._enclosing = _enclosing;
				Stream stream = new FileStream (filename, System.IO.FileMode.Open, FileAccess.Read);
				this.preScanResult = this.PreScanFile(stream);
			}

			private readonly Mp3FileTest _enclosing;
		}















        [TestCase]
        public virtual void TestMp3ID3v2FrameDetectionAlgorithm()
        {
            string filename = "Resources/test.mp3";
            Mp3File mp3File = new Mp3File(filename);
            Assert.Fail();
        }
	}
}
