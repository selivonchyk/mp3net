using System;
using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class BufferToolsTest
	{
		private const byte BYTE_T = unchecked((int)(0x54));

		private const byte BYTE_A = unchecked((int)(0x41));

		private const byte BYTE_G = unchecked((int)(0x47));

		private const byte BYTE_DASH = unchecked((int)(0x2D));

		private const byte BYTE_FF = unchecked((byte)(-unchecked((int)(0x01))));

		private const byte BYTE_FB = unchecked((byte)(-unchecked((int)(0x05))));

		private const byte BYTE_90 = unchecked((byte)(-unchecked((int)(0x70))));

		private const byte BYTE_44 = unchecked((int)(0x44));

		private const byte BYTE_E0 = unchecked((byte)(-unchecked((int)(0x20))));

		private const byte BYTE_F0 = unchecked((byte)(-unchecked((int)(0x10))));

		private const byte BYTE_81 = unchecked((byte)(-unchecked((int)(0x7F))));

		private const byte BYTE_ESZETT = unchecked((byte)(-unchecked((int)(0x21))));

		// byte buffer to string
        [TestCase]
		public virtual void TestShouldExtractStringFromStartOfBuffer()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_A, BYTE_G, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH };
			Assert.AreEqual("TAG", BufferTools.ByteBufferToString(buffer, 0, 3));
		}

        [TestCase]
		public virtual void TestShouldExtractStringFromEndOfBuffer()
		{
			byte[] buffer = new byte[] { BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_T, BYTE_A, BYTE_G };
			Assert.AreEqual("TAG", BufferTools.ByteBufferToString(buffer, 5, 3));
		}

        [TestCase]
		public virtual void TestShouldExtractStringFromMiddleOfBuffer()
		{
			byte[] buffer = new byte[] { BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_T, BYTE_A, BYTE_G };
			Assert.AreEqual("TAG", BufferTools.ByteBufferToString(buffer, 5, 3));
		}

        [TestCase]
		public virtual void TestShouldExtractUnicodeStringFromMiddleOfBuffer()
		{
			byte[] buffer = new byte[] { BYTE_DASH, BYTE_DASH, unchecked((int)(0x03)), unchecked(
				(byte)unchecked((int)(0xb3))), unchecked((int)(0x03)), unchecked((byte)unchecked(
				(int)(0xb5))), unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xb9))), 
				unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xac))), BYTE_DASH, BYTE_DASH};
			Assert.AreEqual("\u03B3\u03B5\u03B9\u03AC", BufferTools.ByteBufferToString(buffer, 2, 8, "UTF-16BE"));
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForOffsetBeforeStartOfArray()
		{
			byte[] buffer = new byte[] { BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_T, BYTE_A, BYTE_G };
            try
            {
                BufferTools.ByteBufferToString(buffer, -1, 4);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionForOffsetAfterEndOfArray()
		{
			byte[] buffer = new byte[] { BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_T, BYTE_A, BYTE_G };
			try
			{
				BufferTools.ByteBufferToString(buffer, buffer.Length, 1);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
		}

        [TestCase]
        public virtual void TestShouldThrowExceptionForLengthExtendingBeyondEndOfArray()
		{
			byte[] buffer = new byte[] { BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_DASH, BYTE_T, BYTE_A, BYTE_G };
			try
			{
				BufferTools.ByteBufferToString(buffer, buffer.Length - 2, 3);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
		}

        [TestCase]
        public virtual void TestShouldConvertStringToBufferAndBack()
		{
			string original = "1234567890QWERTYUIOP";
			byte[] buffer = BufferTools.StringToByteBuffer(original, 0, original.Length);
			string converted = BufferTools.ByteBufferToString(buffer, 0, buffer.Length);
			Assert.AreEqual(original, converted);
		}

        [TestCase]
		public virtual void TestShouldConvertSubstringToBufferAndBack()
		{
			string original = "1234567890QWERTYUIOP";
			byte[] buffer = BufferTools.StringToByteBuffer(original, 2, original.Length - 5);
			string converted = BufferTools.ByteBufferToString(buffer, 0, buffer.Length);
			Assert.AreEqual("34567890QWERTYU", converted);
		}

        [TestCase]
		public virtual void TestShouldConvertUnicodeStringToBufferAndBack()
		{
			string original = "\u03B3\u03B5\u03B9\u03AC \u03C3\u03BF\u03C5";
			byte[] buffer = BufferTools.StringToByteBuffer(original, 0, original.Length, "UTF-16LE");
			string converted = BufferTools.ByteBufferToString(buffer, 0, buffer.Length, "UTF-16LE");
			Assert.AreEqual(original, converted);
		}

        [TestCase]
		public virtual void TestShouldConvertUnicodeSubstringToBufferAndBack()
		{
			string original = "\u03B3\u03B5\u03B9\u03AC \u03C3\u03BF\u03C5";
			byte[] buffer = BufferTools.StringToByteBuffer(original, 2, original.Length - 5, "UTF-16LE");
			string converted = BufferTools.ByteBufferToString(buffer, 0, buffer.Length, "UTF-16LE");
			Assert.AreEqual("\u03B9\u03AC ", converted);
		}

        [TestCase]
		public virtual void TestShouldThrowAnExceptionWhenConvertingStringToBytesWithOffsetOutOfRange()
		{
			string original = "1234567890QWERTYUIOP";
			try
			{
				BufferTools.StringToByteBuffer(original, -1, 1);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
			try
			{
				BufferTools.StringToByteBuffer(original, original.Length, 1);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldThrowAnExceptionWhenConvertingStringToBytesWithLengthOutOfRange()
		{
			string original = "1234567890QWERTYUIOP";
			try
			{
				BufferTools.StringToByteBuffer(original, 0, -1);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
			try
			{
				BufferTools.StringToByteBuffer(original, 0, original.Length + 1);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
			try
			{
				BufferTools.StringToByteBuffer(original, 3, original.Length - 2);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldCopyStringToStartOfByteBuffer()
		{
			byte[] buffer = new byte[10];
			Arrays.Fill(buffer, unchecked((byte)0));
			string s = "TAG-";
			BufferTools.StringIntoByteBuffer(s, 0, s.Length, buffer, 0);
			byte[] expectedBuffer = new byte[] { BYTE_T, BYTE_A, BYTE_G, BYTE_DASH, 0, 0, 0, 0, 0, 0 };
			Assert.IsTrue(Arrays.Equals(expectedBuffer, buffer));
		}

        [TestCase]
		public virtual void TestShouldCopyUnicodeStringToStartOfByteBuffer()
		{
			byte[] buffer = new byte[10];
			Arrays.Fill(buffer, unchecked((byte)0));
			string s = "\u03B3\u03B5\u03B9\u03AC";
			BufferTools.StringIntoByteBuffer(s, 0, s.Length, buffer, 0, "UTF-16BE");
			byte[] expectedBuffer = new byte[] { unchecked((int)(0x03)), unchecked((byte)unchecked(
				(int)(0xb3))), unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xb5))), 
				unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xb9))), unchecked((int)
				(0x03)), unchecked((byte)unchecked((int)(0xac))), 0, 0 };
			Assert.IsTrue(Arrays.Equals(expectedBuffer, buffer));
		}

        [TestCase]
		public virtual void TestShouldCopyStringToEndOfByteBuffer()
		{
			byte[] buffer = new byte[10];
			Arrays.Fill(buffer, unchecked((byte)0));
			string s = "TAG-";
			BufferTools.StringIntoByteBuffer(s, 0, s.Length, buffer, 6);
			byte[] expectedBuffer = new byte[] { 0, 0, 0, 0, 0, 0, BYTE_T, BYTE_A, BYTE_G, BYTE_DASH};
			Assert.IsTrue(Arrays.Equals(expectedBuffer, buffer));
		}

        [TestCase]
		public virtual void TestShouldCopyUnicodeStringToEndOfByteBuffer()
		{
			byte[] buffer = new byte[10];
			Arrays.Fill(buffer, unchecked((byte)0));
			string s = "\u03B3\u03B5\u03B9\u03AC";
			BufferTools.StringIntoByteBuffer(s, 0, s.Length, buffer, 2, "UTF-16BE");
			byte[] expectedBuffer = new byte[] { 0, 0, unchecked((int)(0x03)), unchecked((byte
				)unchecked((int)(0xb3))), unchecked((int)(0x03)), unchecked((byte)unchecked((int
				)(0xb5))), unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xb9))), unchecked(
				(int)(0x03)), unchecked((byte)unchecked((int)(0xac))) };
			Assert.IsTrue(Arrays.Equals(expectedBuffer, buffer));
		}

        [TestCase]
		public virtual void TestShouldCopySubstringToStartOfByteBuffer()
		{
			byte[] buffer = new byte[10];
			Arrays.Fill(buffer, unchecked((byte)0));
			string s = "TAG-";
			BufferTools.StringIntoByteBuffer(s, 1, 2, buffer, 0);
			byte[] expectedBuffer = new byte[] { BYTE_A, BYTE_G, 0, 0, 0, 0, 0, 0, 0, 0 };
			Assert.IsTrue(Arrays.Equals(expectedBuffer, buffer));
		}

        [TestCase]
		public virtual void TestShouldCopyUnicodeSubstringToStartOfByteBuffer()
		{
			byte[] buffer = new byte[10];
			Arrays.Fill(buffer, unchecked((byte)0));
			string s = "\u03B3\u03B5\u03B9\u03AC";
			BufferTools.StringIntoByteBuffer(s, 1, 2, buffer, 0, "UTF-16BE");
			byte[] expectedBuffer = new byte[] { unchecked((int)(0x03)), unchecked((byte)unchecked(
				(int)(0xb5))), unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xb9))), 
				0, 0, 0, 0, 0, 0 };
			Assert.IsTrue(Arrays.Equals(expectedBuffer, buffer));
		}

        [TestCase]
		public virtual void TestShouldCopySubstringToMiddleOfByteBuffer()
		{
			byte[] buffer = new byte[10];
			Arrays.Fill(buffer, unchecked((byte)0));
			string s = "TAG-";
			BufferTools.StringIntoByteBuffer(s, 1, 2, buffer, 4);
			byte[] expectedBuffer = new byte[] { 0, 0, 0, 0, BYTE_A, BYTE_G, 0, 0, 0, 0 };
			Assert.IsTrue(Arrays.Equals(expectedBuffer, buffer));
		}

        [TestCase]
		public virtual void TestShouldRaiseExceptionWhenCopyingStringIntoByteBufferWithOffsetOutOfRange()
		{
			byte[] buffer = new byte[10];
			string s = "TAG-";
			try
			{
				BufferTools.StringIntoByteBuffer(s, -1, 1, buffer, 0);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
			try
			{
				BufferTools.StringIntoByteBuffer(s, s.Length, 1, buffer, 0);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldRaiseExceptionWhenCopyingStringIntoByteBufferWithLengthOutOfRange()
		{
			byte[] buffer = new byte[10];
			string s = "TAG-";
			try
			{
				BufferTools.StringIntoByteBuffer(s, 0, -1, buffer, 0);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
			try
			{
				BufferTools.StringIntoByteBuffer(s, 0, s.Length + 1, buffer, 0);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
			try
			{
				BufferTools.StringIntoByteBuffer(s, 3, s.Length - 2, buffer, 0);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
			}
            catch (ArgumentOutOfRangeException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldRaiseExceptionWhenCopyingStringIntoByteBufferWithDestinationOffsetOutOfRange()
		{
			byte[] buffer = new byte[10];
			string s = "TAG-";
			try
			{
				BufferTools.StringIntoByteBuffer(s, 0, 1, buffer, 10);
                Assert.Fail("ArgumentException expected but not thrown");
			}
			catch (ArgumentException)
			{
			}
			try
			{
				BufferTools.StringIntoByteBuffer(s, 0, s.Length, buffer, buffer.Length - s.Length + 1);
                Assert.Fail("ArgumentException expected but not thrown");
			}
            catch (ArgumentException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldRightTrimStringsCorrectly()
		{
			Assert.AreEqual(string.Empty, BufferTools.TrimStringRight(string.Empty));
			Assert.AreEqual(string.Empty, BufferTools.TrimStringRight(" "));
			Assert.AreEqual("TEST", BufferTools.TrimStringRight("TEST"));
			Assert.AreEqual("TEST", BufferTools.TrimStringRight("TEST   "));
			Assert.AreEqual("   TEST", BufferTools.TrimStringRight("   TEST"));
			Assert.AreEqual("   TEST", BufferTools.TrimStringRight("   TEST   "));
			Assert.AreEqual("TEST", BufferTools.TrimStringRight("TEST\t\r\n"));
			Assert.AreEqual("TEST", BufferTools.TrimStringRight("TEST" + BufferTools.ByteBufferToString(new byte[] { 0, 0 }, 0, 2)));
		}

        [TestCase]
		public virtual void TestShouldRightTrimUnicodeStringsCorrectly()
		{
			Assert.AreEqual("\u03B3\u03B5\u03B9\u03AC", BufferTools.TrimStringRight("\u03B3\u03B5\u03B9\u03AC"));
			Assert.AreEqual("\u03B3\u03B5\u03B9\u03AC", BufferTools.TrimStringRight("\u03B3\u03B5\u03B9\u03AC   "));
			Assert.AreEqual("   \u03B3\u03B5\u03B9\u03AC", BufferTools.TrimStringRight("   \u03B3\u03B5\u03B9\u03AC"));
			Assert.AreEqual("   \u03B3\u03B5\u03B9\u03AC", BufferTools.TrimStringRight("   \u03B3\u03B5\u03B9\u03AC   "));
			Assert.AreEqual("\u03B3\u03B5\u03B9\u03AC", BufferTools.TrimStringRight("\u03B3\u03B5\u03B9\u03AC\t\r\n"));
			Assert.AreEqual("\u03B3\u03B5\u03B9\u03AC", BufferTools.TrimStringRight("\u03B3\u03B5\u03B9\u03AC" + BufferTools.ByteBufferToString(new byte[] { 0, 0 }, 0, 2)));
		}

        [TestCase]
		public virtual void TestShouldRightPadStringsCorrectly()
		{
			Assert.AreEqual("1234", BufferTools.PadStringRight("1234", 3, ' '));
			Assert.AreEqual("123", BufferTools.PadStringRight("123", 3, ' '));
			Assert.AreEqual("12 ", BufferTools.PadStringRight("12", 3, ' '));
			Assert.AreEqual("1  ", BufferTools.PadStringRight("1", 3, ' '));
			Assert.AreEqual("   ", BufferTools.PadStringRight(string.Empty, 3, ' '));
		}

        [TestCase]
		public virtual void TestShouldRightPadUnicodeStringsCorrectly()
		{
			Assert.AreEqual("\u03B3\u03B5\u03B9\u03AC", BufferTools.PadStringRight("\u03B3\u03B5\u03B9\u03AC", 3, ' '));
			Assert.AreEqual("\u03B3\u03B5\u03B9", BufferTools.PadStringRight("\u03B3\u03B5\u03B9", 3, ' '));
			Assert.AreEqual("\u03B3\u03B5 ", BufferTools.PadStringRight("\u03B3\u03B5", 3, ' '));
			Assert.AreEqual("\u03B3  ", BufferTools.PadStringRight("\u03B3", 3, ' '));
		}

        [TestCase]
		public virtual void TestShouldPadRightWithNullCharacters()
		{
			Assert.AreEqual("123", BufferTools.PadStringRight("123", 3, '\0'));
			Assert.AreEqual("12\x0", BufferTools.PadStringRight("12", 3, '\0'));
			Assert.AreEqual("1\x0\x0", BufferTools.PadStringRight("1", 3, '\0'));
			Assert.AreEqual("\x0\x0\x0", BufferTools.PadStringRight(string.Empty, 3, '\0'));
		}

        [TestCase]
		public virtual void TestShouldExtractBitsCorrectly()
		{
			byte b = unchecked((byte)(-unchecked((int)(0x36))));
			// 11001010
			Assert.IsFalse(BufferTools.CheckBit(b, 0));
			Assert.IsTrue(BufferTools.CheckBit(b, 1));
			Assert.IsFalse(BufferTools.CheckBit(b, 2));
			Assert.IsTrue(BufferTools.CheckBit(b, 3));
			Assert.IsFalse(BufferTools.CheckBit(b, 4));
			Assert.IsFalse(BufferTools.CheckBit(b, 5));
			Assert.IsTrue(BufferTools.CheckBit(b, 6));
			Assert.IsTrue(BufferTools.CheckBit(b, 7));
		}

        [TestCase]
		public virtual void TestShouldSetBitsInBytesCorrectly()
		{
			byte b = unchecked((byte)(-unchecked((int)(0x36))));
			// 11001010
			Assert.AreEqual(unchecked((byte)-unchecked((int)(0x36))), BufferTools.SetBit(b, 7, true));
			// 11010010
			Assert.AreEqual(unchecked((byte)-unchecked((int)(0x35))), BufferTools.SetBit(b, 0, true));
			// 11001011
			Assert.AreEqual(unchecked((byte)-unchecked((int)(0x26))), BufferTools.SetBit(b, 4, true));
			// 11011010
			Assert.AreEqual(unchecked((byte)-unchecked((int)(0x36))), BufferTools.SetBit(b, 0, false));
			// 11010010
			Assert.AreEqual(unchecked((byte)unchecked((int)(0x4A))), BufferTools.SetBit(b, 7, false));
			// 01001010
			Assert.AreEqual(unchecked((byte)-unchecked((int)(0x3E))), BufferTools.SetBit(b, 3, false));
		}

        [TestCase]
		// 11000010
		public virtual void TestShouldUnpackIntegerCorrectly()
		{
			Assert.AreEqual(unchecked((int)(0xFFFB9044)), BufferTools.UnpackInteger(BYTE_FF, BYTE_FB, BYTE_90, BYTE_44));
			Assert.AreEqual(unchecked((int)(0x00000081)), BufferTools.UnpackInteger(unchecked((byte)0), unchecked((byte)0), unchecked((byte)0), BYTE_81));
			Assert.AreEqual(unchecked((int)(0x00000101)), BufferTools.UnpackInteger(unchecked((byte)0), unchecked((byte)0), unchecked((byte)1), unchecked((byte)1)));
		}

        [TestCase]
		public virtual void TestShouldUnpackSynchsafeIntegersCorrectly()
		{
			Assert.AreEqual(1217, BufferTools.UnpackSynchsafeInteger(unchecked(
				(byte)0), unchecked((byte)0), unchecked((byte)unchecked((int)(0x09))), unchecked(
				(byte)unchecked((int)(0x41)))));
			Assert.AreEqual(1227, BufferTools.UnpackSynchsafeInteger(unchecked(
				(byte)0), unchecked((byte)0), unchecked((byte)unchecked((int)(0x09))), unchecked(
				(byte)unchecked((int)(0x4B)))));
			Assert.AreEqual(1002, BufferTools.UnpackSynchsafeInteger(unchecked(
				(byte)0), unchecked((byte)0), unchecked((byte)unchecked((int)(0x07))), unchecked(
				(byte)unchecked((int)(0x6A)))));
			Assert.AreEqual(unchecked((int)(0x0101)), BufferTools.UnpackSynchsafeInteger
				(unchecked((byte)0), unchecked((byte)0), unchecked((byte)2), unchecked((byte)1))
				);
			Assert.AreEqual(unchecked((int)(0x01010101)), BufferTools.UnpackSynchsafeInteger
				(unchecked((byte)8), unchecked((byte)4), unchecked((byte)2), unchecked((byte)1))
				);
		}

        [TestCase]
		public virtual void TestShouldPackIntegerCorrectly()
		{
			Assert.IsTrue(Arrays.Equals(new byte[] { BYTE_FF, BYTE_FB, BYTE_90, BYTE_44 }, BufferTools.PackInteger(unchecked((int)(0xFFFB9044)))));
		}

        [TestCase]
		public virtual void TestShouldPackSynchsafeIntegersCorrectly()
		{
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)0), unchecked(
				(byte)0), unchecked((byte)unchecked((int)(0x09))), unchecked((byte)unchecked((int
				)(0x41))) }, BufferTools.PackSynchsafeInteger(1217)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)0), unchecked(
				(byte)0), unchecked((byte)unchecked((int)(0x09))), unchecked((byte)unchecked((int
				)(0x4B))) }, BufferTools.PackSynchsafeInteger(1227)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)0), unchecked(
				(byte)0), unchecked((byte)unchecked((int)(0x07))), unchecked((byte)unchecked((int
				)(0x6A))) }, BufferTools.PackSynchsafeInteger(1002)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)0), unchecked(
				(byte)0), unchecked((byte)2), unchecked((byte)1) }, BufferTools.PackSynchsafeInteger
				(unchecked((int)(0x0101)))));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)8), unchecked(
				(byte)4), unchecked((byte)2), unchecked((byte)1) }, BufferTools.PackSynchsafeInteger
				(unchecked((int)(0x01010101)))));
		}

        [TestCase]
		public virtual void TestShouldPackAndInpackIntegerBackToOriginalValue()
		{
			int original = 12345;
			byte[] bytes = BufferTools.PackInteger(original);
			int unpacked = BufferTools.UnpackInteger(bytes[0], bytes[1], bytes[2], bytes[3]);
			Assert.AreEqual(original, unpacked);
		}

        [TestCase]
		public virtual void TestShouldPackAndInpackSynchsafeIntegerBackToOriginalValue()
		{
			int original = 12345;
			byte[] bytes = BufferTools.PackSynchsafeInteger(original);
			int unpacked = BufferTools.UnpackSynchsafeInteger(bytes[0], bytes[1], bytes[2], bytes[3]);
			Assert.AreEqual(original, unpacked);
		}

        [TestCase]
		public virtual void TestShouldCopyBuffersWithValidOffsetsAndLengths()
		{
			byte[] buffer = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			Assert.IsTrue(Arrays.Equals(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, BufferTools.CopyBuffer(buffer, 0, buffer.Length)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, BufferTools.CopyBuffer(buffer, 1, buffer.Length - 1)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, BufferTools.CopyBuffer(buffer, 0, buffer.Length - 1)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, BufferTools.CopyBuffer(buffer, 1, buffer.Length - 2)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 4 }, BufferTools.CopyBuffer(buffer, 4, 1)));
		}

        [TestCase]
		public virtual void TestThrowExceptionWhenCopyingBufferWithInvalidOffsetAndOrLength()
		{
			byte[] buffer = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			TryCopyBufferWithInvalidOffsetAndOrLength(buffer, -1, buffer.Length);
			TryCopyBufferWithInvalidOffsetAndOrLength(buffer, buffer.Length, 1);
			TryCopyBufferWithInvalidOffsetAndOrLength(buffer, 1, buffer.Length);
		}

        [TestCase]
		public virtual void TestShouldDetermineUnsynchronisationSizesCorrectly()
		{
			Assert.AreEqual(0, BufferTools.SizeUnsynchronisationWouldAdd(new byte[] {  }));
			Assert.AreEqual(0, BufferTools.SizeUnsynchronisationWouldAdd(new byte[] { BYTE_FF, 1, BYTE_FB }));
			Assert.AreEqual(1, BufferTools.SizeUnsynchronisationWouldAdd(new byte[] { BYTE_FF, BYTE_FB }));
			Assert.AreEqual(1, BufferTools.SizeUnsynchronisationWouldAdd(new byte[] { 0, BYTE_FF, BYTE_FB, 0 }));
			Assert.AreEqual(1, BufferTools.SizeUnsynchronisationWouldAdd(new byte[] { 0, BYTE_FF }));
			Assert.AreEqual(2, BufferTools.SizeUnsynchronisationWouldAdd(new byte[] { BYTE_FF, BYTE_FB, 0, BYTE_FF, BYTE_FB }));
			Assert.AreEqual(3, BufferTools.SizeUnsynchronisationWouldAdd(new byte[] { BYTE_FF, BYTE_FF, BYTE_FF }));
		}

        [TestCase]
		public virtual void TestShouldDetermineSynchronisationSizesCorrectly()
		{
			Assert.AreEqual(0, BufferTools.SizeSynchronisationWouldSubtract(new byte[] {  }));
			Assert.AreEqual(0, BufferTools.SizeSynchronisationWouldSubtract(new byte[] { BYTE_FF, 1, BYTE_FB }));
			Assert.AreEqual(1, BufferTools.SizeSynchronisationWouldSubtract(new byte[] { BYTE_FF, 0, BYTE_FB }));
			Assert.AreEqual(1, BufferTools.SizeSynchronisationWouldSubtract(new byte[] { 0, BYTE_FF, 0, BYTE_FB, 0 }));
			Assert.AreEqual(1, BufferTools.SizeSynchronisationWouldSubtract(new byte[] { 0, BYTE_FF, 0 }));
			Assert.AreEqual(2, BufferTools.SizeSynchronisationWouldSubtract(new byte[] { BYTE_FF, 0, BYTE_FB, 0, BYTE_FF, 0, BYTE_FB }));
			Assert.AreEqual(3, BufferTools.SizeSynchronisationWouldSubtract(new byte[] { BYTE_FF, 0, BYTE_FF, 0, BYTE_FF, 0 }));
		}

        [TestCase]
		public virtual void TestShouldUnsynchroniseThenSynchroniseFFExBytesCorrectly()
		{
			byte[] buffer = new byte[] { BYTE_FF, BYTE_FB, 2, 3, 4, BYTE_FF, BYTE_E0, 7, 8, 9, 10, 11, 12, 13, BYTE_FF, BYTE_F0 };
			byte[] expectedBuffer = new byte[] { BYTE_FF, 0, BYTE_FB, 2, 3, 4, BYTE_FF, 0, BYTE_E0, 7, 8, 9, 10, 11, 12, 13, BYTE_FF, 0, BYTE_F0 };
			byte[] unsynchronised = BufferTools.UnsynchroniseBuffer(buffer);
			byte[] synchronised = BufferTools.SynchroniseBuffer(unsynchronised);
			Assert.IsTrue(Arrays.Equals(expectedBuffer, unsynchronised));
			Assert.IsTrue(Arrays.Equals(buffer, synchronised));
		}

        [TestCase]
		public virtual void TestShouldUnsynchroniseThenSynchroniseFF00BytesCorrectly()
		{
			byte[] buffer = new byte[] { BYTE_FF, 0, 2, 3, 4, BYTE_FF, 0, 7, 8, 9, 10, 11, 12, 13, BYTE_FF, 0 };
			byte[] expectedBuffer = new byte[] { BYTE_FF, 0, 0, 2, 3, 4, BYTE_FF, 0, 0, 7, 8, 9, 10, 11, 12, 13, BYTE_FF, 0, 0 };
			byte[] unsynchronised = BufferTools.UnsynchroniseBuffer(buffer);
			byte[] synchronised = BufferTools.SynchroniseBuffer(unsynchronised);
			Assert.IsTrue(Arrays.Equals(expectedBuffer, unsynchronised));
			Assert.IsTrue(Arrays.Equals(buffer, synchronised));
		}

        [TestCase]
		public virtual void TestShouldUnsynchroniseThenSynchroniseBufferFullOfFFsCorrectly()
		{
			byte[] buffer = new byte[] { BYTE_FF, BYTE_FF, BYTE_FF, BYTE_FF };
			byte[] expectedBuffer = new byte[] { BYTE_FF, 0, BYTE_FF, 0, BYTE_FF, 0, BYTE_FF, 0 };
			byte[] unsynchronised = BufferTools.UnsynchroniseBuffer(buffer);
			byte[] synchronised = BufferTools.SynchroniseBuffer(unsynchronised);
			Assert.IsTrue(Arrays.Equals(expectedBuffer, unsynchronised));
			Assert.IsTrue(Arrays.Equals(buffer, synchronised));
		}

        [TestCase]
		public virtual void TestShouldUnsynchroniseThenSynchroniseBufferMinimalBufferCorrectly()
		{
			byte[] buffer = new byte[] { BYTE_FF };
			byte[] expectedBuffer = new byte[] { BYTE_FF, 0 };
			byte[] unsynchronised = BufferTools.UnsynchroniseBuffer(buffer);
			byte[] synchronised = BufferTools.SynchroniseBuffer(unsynchronised);
			Assert.IsTrue(Arrays.Equals(expectedBuffer, unsynchronised));
			Assert.IsTrue(Arrays.Equals(buffer, synchronised));
		}

        [TestCase]
		public virtual void TestShouldReturnOriginalBufferIfNoUnynchronisationOrSynchronisationIsRequired()
		{
			byte[] buffer = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
			byte[] unsynchronised = BufferTools.UnsynchroniseBuffer(buffer);
			byte[] synchronised = BufferTools.SynchroniseBuffer(buffer);
			Assert.AreEqual(buffer, unsynchronised);
			Assert.AreEqual(buffer, synchronised);
		}

        [TestCase]
		public virtual void TestShouldReplaceTokensWithSpecifiedStrings()
		{
			string source = "%1-%2 something %1-%3";
			Assert.AreEqual("ONE-%2 something ONE-%3", BufferTools.Substitute(source, "%1", "ONE"));
			Assert.AreEqual("%1-TWO something %1-%3", BufferTools.Substitute(source, "%2", "TWO"));
			Assert.AreEqual("%1-%2 something %1-THREE", BufferTools.Substitute(source, "%3", "THREE"));
		}

        [TestCase]
		public virtual void TestShouldReturnOriginalStringIfTokenToSubstituteDoesNotExistInString()
		{
			string source = "%1-%2 something %1-%3";
			Assert.AreEqual("%1-%2 something %1-%3", BufferTools.Substitute(source, "%X", "XXXXX"));
		}

        [TestCase]
		public virtual void TestShouldReturnOriginalStringForSubstitutionWithEmptyString()
		{
			string source = "%1-%2 something %1-%3";
			Assert.AreEqual("%1-%2 something %1-%3", BufferTools.Substitute(source, string.Empty, "WHATEVER"));
		}

        [TestCase]
		public virtual void TestShouldSubstituteEmptyStringWhenDestinationStringIsNull()
		{
			string source = "%1-%2 something %1-%3";
			Assert.AreEqual("-%2 something -%3", BufferTools.Substitute(source, "%1", null));
		}

        [TestCase]
		public virtual void TestShouldConvertNonAsciiCharactersToQuestionMarksInString()
		{
			Assert.AreEqual("?12?34?567???89?", BufferTools.AsciiOnly("ь12¬34ь567¬¬¬89ь"));
		}

        private void TryCopyBufferWithInvalidOffsetAndOrLength(byte[] buffer, int offset, int length)
		{
            try
            {
                BufferTools.CopyBuffer(buffer, offset, length);
                Assert.Fail("ArgumentOutOfRangeException expected but not thrown");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (ArgumentException)
            {
            }
		}

        [TestCase]
        public virtual void TestShouldConvertBufferContainingHighAscii()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G };
			Assert.AreEqual("T" + (char)(223) + "G", BufferTools.ByteBufferToString(buffer, 0, 3));
		}

        [TestCase]
		// finding terminators
		public virtual void TestShouldFindSingleTerminator()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T, 0, BYTE_G, BYTE_A};
			Assert.AreEqual(4, BufferTools.IndexOfTerminator(buffer, 0, 1));
		}

        [TestCase]
		public virtual void TestShouldFindFirstSingleTerminator()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T, 0, BYTE_G, BYTE_A, 0, BYTE_G, BYTE_A };
			Assert.AreEqual(4, BufferTools.IndexOfTerminator(buffer, 0, 1));
		}

        [TestCase]
		public virtual void TestShouldFindFirstSingleTerminatorAfterFromIndex()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T, 0, BYTE_G, BYTE_A, 0, BYTE_G, BYTE_A };
			Assert.AreEqual(7, BufferTools.IndexOfTerminator(buffer, 5, 1));
		}

        [TestCase]
		public virtual void TestShouldFindSingleTerminatorWhenFirstElement()
		{
			byte[] buffer = new byte[] { 0, BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T };
			Assert.AreEqual(0, BufferTools.IndexOfTerminator(buffer, 0, 1));
		}

        [TestCase]
		public virtual void TestShouldFindSingleTerminatorWhenLastElement()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T, 0 };
			Assert.AreEqual(4, BufferTools.IndexOfTerminator(buffer, 0, 1));
		}

        [TestCase]
		public virtual void TestShouldReturnMinusOneWhenNoSingleTerminator()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T };
			Assert.AreEqual(-1, BufferTools.IndexOfTerminator(buffer, 0, 1));
		}

        [TestCase]
		public virtual void TestShouldFindDoubleTerminator()
		{
			byte[] buffer = new byte[] { BYTE_T, 0, BYTE_G, BYTE_T, 0, 0, BYTE_G, BYTE_A };
			Assert.AreEqual(4, BufferTools.IndexOfTerminator(buffer, 0, 2));
		}

        [TestCase]
		public virtual void TestShouldFindNotFindDoubleTerminatorIfNotOnEvenByte()
		{
			byte[] buffer = new byte[] { BYTE_T, 0, BYTE_G, BYTE_T, BYTE_T, 0, 0, BYTE_G, BYTE_A};
			Assert.AreEqual(-1, BufferTools.IndexOfTerminator(buffer, 0, 2));
		}

        [TestCase]
		public virtual void TestShouldFindFirstDoubleTerminator()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T, 0, 0, BYTE_G, BYTE_A, 0, 0, BYTE_G, BYTE_A };
			Assert.AreEqual(4, BufferTools.IndexOfTerminator(buffer, 0, 2));
		}

        [TestCase]
		public virtual void TestShouldFindFirstDoubleTerminatorOnAnEvenByte()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, 0, 0, BYTE_T, BYTE_G, BYTE_A, 0, 0, BYTE_G, BYTE_A };
			Assert.AreEqual(8, BufferTools.IndexOfTerminator(buffer, 0, 2));
		}

        [TestCase]
		public virtual void TestShouldFindFirstDoubleTerminatorAfterFromIndex()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T, 0, 0, BYTE_G, BYTE_A, 0, 0, BYTE_G, BYTE_A };
			Assert.AreEqual(8, BufferTools.IndexOfTerminator(buffer, 6, 2));
		}

        [TestCase]
		public virtual void TestShouldFindDoubleTerminatorWhenFirstElement()
		{
			byte[] buffer = new byte[] { 0, 0, BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T };
			Assert.AreEqual(0, BufferTools.IndexOfTerminator(buffer, 0, 2));
		}

        [TestCase]
		public virtual void TestShouldFindDoubleTerminatorWhenLastElement()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T, 0, 0 };
			Assert.AreEqual(4, BufferTools.IndexOfTerminator(buffer, 0, 2));
		}

        [TestCase]
		public virtual void TestShouldReturnMinusOneWhenNoDoubleTerminator()
		{
			byte[] buffer = new byte[] { BYTE_T, BYTE_ESZETT, BYTE_G, BYTE_T };
			Assert.AreEqual(-1, BufferTools.IndexOfTerminator(buffer, 0, 2));
		}
	}
}
