using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class MpegFrameTest
	{
		private const byte BYTE_FF = unchecked((byte)(-unchecked((int)(0x01))));

		private const byte BYTE_FB = unchecked((byte)(-unchecked((int)(0x05))));

		private const byte BYTE_F9 = unchecked((byte)(-unchecked((int)(0x07))));

		private const byte BYTE_F3 = unchecked((byte)(-unchecked((int)(0x0D))));

		private const byte BYTE_F2 = unchecked((byte)(-unchecked((int)(0x0E))));

		private const byte BYTE_A2 = unchecked((byte)(-unchecked((int)(0x5E))));

		private const byte BYTE_AE = unchecked((byte)(-unchecked((int)(0x52))));

		private const byte BYTE_DB = unchecked((byte)(-unchecked((int)(0x25))));

		private const byte BYTE_EB = unchecked((byte)(-unchecked((int)(0x15))));

		private const byte BYTE_40 = unchecked((int)(0x40));

		private const byte BYTE_02 = unchecked((int)(0x02));

        [TestCase]
		public virtual void TestBitwiseLeftShiftOperationsOnLong()
		{
			long original = unchecked((int)(0xFFFFFFFE));
			// 1111 1111 1111 1111 1111 1111 1111 1110
			long expectedShl1 = unchecked((int)(0xFFFFFFFC));
			// 1111 1111 1111 1111 1111 1111 1111 1100
			long expectedShl28 = unchecked((int)(0xE0000000));
			// 1110 0000 0000 0000 0000 0000 0000 0000
			long expectedShl30 = unchecked((int)(0x80000000));
			// 1000 0000 0000 0000 0000 0000 0000 0000
			Assert.AreEqual(expectedShl1, original << 1);
			Assert.AreEqual(expectedShl28, original << 28);
			Assert.AreEqual(expectedShl30, original << 30);
		}

        [TestCase]
		public virtual void TestBitwiseRightShiftOperationsOnLong()
		{
			long original = unchecked((int)(0x80000000));
			// 1000 0000 0000 0000 0000 0000 0000 0000
			long expectedShr1 = unchecked((int)(0xC0000000));
			// 1100 0000 0000 0000 0000 0000 0000 0000
			long expectedShr28 = unchecked((int)(0xFFFFFFF8));
			// 1111 1111 1111 1111 1111 1111 1111 1000
			long expectedShr30 = unchecked((int)(0xFFFFFFFE));
			// 1111 1111 1111 1111 1111 1111 1111 1110
			Assert.AreEqual(expectedShr1, original >> 1);
			Assert.AreEqual(expectedShr28, original >> 28);
			Assert.AreEqual(expectedShr30, original >> 30);
		}

        [TestCase]
		public virtual void TestShiftingByteIntoBiggerNumber()
		{
			byte original = unchecked((byte)(-unchecked((int)(0x02))));
			// 1111 1110
			long originalAsLong = (original & unchecked((int)(0xFF)));
			byte expectedShl1 = unchecked((byte)(-unchecked((int)(0x04))));
			// 1111 1100
			long expectedShl8 = unchecked((int)(0x0000FE00));
			// 0000 0000 0000 0000 1111 1110 0000 0000
			long expectedShl16 = unchecked((int)(0x00FE0000));
			// 0000 0000 1111 1110 0000 0000 0000 0000
			long expectedShl23 = unchecked((int)(0x7F000000));
			// 0111 1111 00000 0000 0000 0000 0000 0000
			Assert.AreEqual(expectedShl1, unchecked((byte)(original << 1)));
			Assert.AreEqual(254, originalAsLong);
			Assert.AreEqual(expectedShl8, originalAsLong << 8);
			Assert.AreEqual(expectedShl16, originalAsLong << 16);
			Assert.AreEqual(expectedShl23, originalAsLong << 23);
		}

        [TestCase]
		public virtual void TestShouldExtractValidFields()
		{
			MpegFrameTest.MpegFrameForTesting mpegFrame = new MpegFrameTest.MpegFrameForTesting
				(this);
			Assert.AreEqual(unchecked((int)(0x000007FF)), mpegFrame.ExtractField
				(unchecked((int)(0xFFE00000)), unchecked((long)(0xFFE00000L))));
			Assert.AreEqual(unchecked((int)(0x000007FF)), mpegFrame.ExtractField
				(unchecked((int)(0xFFEFFFFF)), unchecked((long)(0xFFE00000L))));
			Assert.AreEqual(unchecked((int)(0x00000055)), mpegFrame.ExtractField
				(unchecked((int)(0x11111155)), unchecked((long)(0x000000FFL))));
			Assert.AreEqual(unchecked((int)(0x00000055)), mpegFrame.ExtractField
				(unchecked((int)(0xFFEFFF55)), unchecked((long)(0x000000FFL))));
		}

        [TestCase]
		public virtual void TestShouldExtractValidMpegVersion1Header()
		{
			byte[] frameData = new byte[] { BYTE_FF, BYTE_FB, BYTE_A2, BYTE_40 };
			MpegFrameTest.MpegFrameForTesting mpegFrame = new MpegFrameTest.MpegFrameForTesting(this, frameData);
			Assert.AreEqual(MpegFrame.MPEG_VERSION_1_0, mpegFrame.GetVersion());
			Assert.AreEqual(MpegFrame.MPEG_LAYER_3, mpegFrame.GetLayer());
			Assert.AreEqual(160, mpegFrame.GetBitrate());
			Assert.AreEqual(44100, mpegFrame.GetSampleRate());
			Assert.AreEqual(MpegFrame.CHANNEL_MODE_JOINT_STEREO, mpegFrame.GetChannelMode());
			Assert.AreEqual("None", mpegFrame.GetModeExtension());
			Assert.AreEqual("None", mpegFrame.GetEmphasis());
			Assert.AreEqual(true, mpegFrame.IsProtection());
			Assert.AreEqual(true, mpegFrame.HasPadding());
			Assert.AreEqual(false, mpegFrame.IsPrivate());
			Assert.AreEqual(false, mpegFrame.IsCopyright());
			Assert.AreEqual(false, mpegFrame.IsOriginal());
			Assert.AreEqual(523, mpegFrame.GetLengthInBytes());
		}

        [TestCase]
		public virtual void TestShouldProcessValidMpegVersion2Header()
		{
			byte[] frameData = new byte[] { BYTE_FF, BYTE_F3, BYTE_A2, BYTE_40 };
			MpegFrameTest.MpegFrameForTesting mpegFrame = new MpegFrameTest.MpegFrameForTesting(this, frameData);
			Assert.AreEqual(MpegFrame.MPEG_VERSION_2_0, mpegFrame.GetVersion());
			Assert.AreEqual(MpegFrame.MPEG_LAYER_3, mpegFrame.GetLayer());
			Assert.AreEqual(96, mpegFrame.GetBitrate());
			Assert.AreEqual(22050, mpegFrame.GetSampleRate());
			Assert.AreEqual(MpegFrame.CHANNEL_MODE_JOINT_STEREO, mpegFrame.GetChannelMode());
			Assert.AreEqual("None", mpegFrame.GetModeExtension());
			Assert.AreEqual("None", mpegFrame.GetEmphasis());
			Assert.AreEqual(true, mpegFrame.IsProtection());
			Assert.AreEqual(true, mpegFrame.HasPadding());
			Assert.AreEqual(false, mpegFrame.IsPrivate());
			Assert.AreEqual(false, mpegFrame.IsCopyright());
			Assert.AreEqual(false, mpegFrame.IsOriginal());
			Assert.AreEqual(627, mpegFrame.GetLengthInBytes());
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForInvalidFrameSync()
		{
			byte[] frameData = new byte[] { BYTE_FF, BYTE_DB, BYTE_A2, BYTE_40 };
			try
			{
				new MpegFrameTest.MpegFrameForTesting(this, frameData);
				Assert.Fail("InvalidDataException expected but not thrown");
			}
			catch (InvalidDataException e)
			{
				Assert.AreEqual("Frame sync missing", e.Message);
			}
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForInvalidMpegVersion()
		{
			byte[] frameData = new byte[] { BYTE_FF, BYTE_EB, BYTE_A2, BYTE_40 };
			try
			{
				new MpegFrameTest.MpegFrameForTesting(this, frameData);
				Assert.Fail("InvalidDataException expected but not thrown");
			}
			catch (InvalidDataException e)
			{
				Assert.AreEqual("Invalid mpeg audio version in frame header", e.Message
					);
			}
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForInvalidMpegLayer()
		{
			byte[] frameData = new byte[] { BYTE_FF, BYTE_F9, BYTE_A2, BYTE_40 };
			try
			{
				new MpegFrameTest.MpegFrameForTesting(this, frameData);
				Assert.Fail("InvalidDataException expected but not thrown");
			}
			catch (InvalidDataException e)
			{
				Assert.AreEqual("Invalid mpeg layer description in frame header", e.Message);
			}
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForFreeBitrate()
		{
			byte[] frameData = new byte[] { BYTE_FF, BYTE_FB, BYTE_02, BYTE_40 };
			try
			{
				new MpegFrameTest.MpegFrameForTesting(this, frameData);
				Assert.Fail("InvalidDataException expected but not thrown");
			}
			catch (InvalidDataException e)
			{
				Assert.AreEqual("Invalid bitrate in frame header", e.Message);
			}
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForInvalidBitrate()
		{
			byte[] frameData = new byte[] { BYTE_FF, BYTE_FB, BYTE_F2, BYTE_40 };
			try
			{
				new MpegFrameTest.MpegFrameForTesting(this, frameData);
				Assert.Fail("InvalidDataException expected but not thrown");
			}
			catch (InvalidDataException e)
			{
				Assert.AreEqual("Invalid bitrate in frame header", e.Message);
			}
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForInvalidSampleRate()
		{
			byte[] frameData = new byte[] { BYTE_FF, BYTE_FB, BYTE_AE, BYTE_40 };
			try
			{
				new MpegFrameTest.MpegFrameForTesting(this, frameData);
				Assert.Fail("InvalidDataException expected but not thrown");
			}
			catch (InvalidDataException e)
			{
				Assert.AreEqual("Invalid sample rate in frame header", e.Message);
			}
		}

		internal class MpegFrameForTesting : MpegFrame
		{
			public MpegFrameForTesting(MpegFrameTest _enclosing) : base()
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Mp3net.InvalidDataException"></exception>
			public MpegFrameForTesting(MpegFrameTest _enclosing, byte[] frameData) : base(frameData)
			{
				this._enclosing = _enclosing;
			}

			public virtual int ExtractField(int frameHeader, int bitMask)
			{
				return base.ExtractField(frameHeader, bitMask);
			}

			private readonly MpegFrameTest _enclosing;
		}
	}
}
