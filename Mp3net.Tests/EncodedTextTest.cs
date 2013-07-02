using System;
using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class EncodedTextTest
	{
		private static readonly string TEST_STRING = "This is a string!";

		private static readonly string TEST_STRING_HEX_ISO8859_1 = "54 68 69 73 20 69 73 20 61 20 73 74 72 69 6e 67 21";

		private static readonly string UNICODE_TEST_STRING = "\u03B3\u03B5\u03B9\u03AC \u03C3\u03BF\u03C5";

		private static readonly string UNICODE_TEST_STRING_HEX_UTF8 = "ce b3 ce b5 ce b9 ce ac 20 cf 83 ce bf cf 85";

		private static readonly string UNICODE_TEST_STRING_HEX_UTF16LE = "b3 03 b5 03 b9 03 ac 03 20 00 c3 03 bf 03 c5 03";

		private static readonly string UNICODE_TEST_STRING_HEX_UTF16BE = "03 b3 03 b5 03 b9 03 ac 00 20 03 c3 03 bf 03 c5";

		private static readonly byte[] BUFFER_WITH_A_BACKTICK = new byte[] { unchecked((byte
			)unchecked((int)(0x49))), unchecked((byte)unchecked((int)(0x60))), unchecked((byte
			)unchecked((int)(0x6D))) };

        [TestCase]
		public virtual void TestShouldConstructFromStringOrBytes()
		{
			EncodedText encodedText;
			EncodedText encodedText2;
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, TEST_STRING);
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, TestHelper.HexStringToBytes(TEST_STRING_HEX_ISO8859_1));
			Assert.AreEqual(encodedText, encodedText2);
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, UNICODE_TEST_STRING);
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, TestHelper.HexStringToBytes(UNICODE_TEST_STRING_HEX_UTF8));
			Assert.AreEqual(encodedText, encodedText2);
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, UNICODE_TEST_STRING);
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, TestHelper.HexStringToBytes(UNICODE_TEST_STRING_HEX_UTF16LE));
			Assert.AreEqual(encodedText, encodedText2);
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16BE, UNICODE_TEST_STRING);
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16BE, TestHelper.HexStringToBytes(UNICODE_TEST_STRING_HEX_UTF16BE));
			Assert.AreEqual(encodedText, encodedText2);
		}

        [TestCase]
		public virtual void TestShouldUseAppropriateEncodingWhenConstructingFromStringOnly()
		{
			EncodedText encodedText;
			string s;
			encodedText = new EncodedText(TEST_STRING);
			s = encodedText.ToString();
			Assert.IsNotNull(s);
			encodedText = new EncodedText(UNICODE_TEST_STRING);
			s = encodedText.ToString();
			Assert.IsNotNull(s);
		}

        [TestCase]
		public virtual void TestShouldEncodeAndDecodeISO8859_1Text()
		{
			EncodedText encodedText = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, TEST_STRING);
			Assert.AreEqual(EncodedText.CHARSET_ISO_8859_1, encodedText.GetCharacterSet());
			Assert.AreEqual(TEST_STRING, encodedText.ToString());
			EncodedText encodedText2;
			byte[] bytes;
			// no bom & no terminator
			bytes = encodedText.ToBytes();
			Assert.AreEqual(TEST_STRING_HEX_ISO8859_1, TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// bom & no terminator
			bytes = encodedText.ToBytes(true);
			Assert.AreEqual(TEST_STRING_HEX_ISO8859_1, TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// no bom & terminator
			bytes = encodedText.ToBytes(false, true);
			Assert.AreEqual(TEST_STRING_HEX_ISO8859_1 + " 00", TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// bom & terminator
			bytes = encodedText.ToBytes(true, true);
			Assert.AreEqual(TEST_STRING_HEX_ISO8859_1 + " 00", TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, bytes);
			Assert.AreEqual(encodedText, encodedText2);
		}

        [TestCase]
		public virtual void TestShouldEncodeAndDecodeUTF8Text()
		{
			EncodedText encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, UNICODE_TEST_STRING);
			Assert.AreEqual(EncodedText.CHARSET_UTF_8, encodedText.GetCharacterSet());
			Assert.AreEqual(UNICODE_TEST_STRING, encodedText.ToString());
			EncodedText encodedText2;
			byte[] bytes;
			// no bom & no terminator
			bytes = encodedText.ToBytes();
			string c = TestHelper.BytesToHexString(bytes);
			Assert.AreEqual(UNICODE_TEST_STRING_HEX_UTF8, c);
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// bom & no terminator
			bytes = encodedText.ToBytes(true);
			Assert.AreEqual(UNICODE_TEST_STRING_HEX_UTF8, TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// no bom & terminator
			bytes = encodedText.ToBytes(false, true);
			Assert.AreEqual(UNICODE_TEST_STRING_HEX_UTF8 + " 00", TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// bom & terminator
			bytes = encodedText.ToBytes(true, true);
			Assert.AreEqual(UNICODE_TEST_STRING_HEX_UTF8 + " 00", TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, bytes);
			Assert.AreEqual(encodedText, encodedText2);
		}

        [TestCase]
		public virtual void TestShouldEncodeAndDecodeUTF16Text()
		{
			EncodedText encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, UNICODE_TEST_STRING);
			Assert.AreEqual(EncodedText.CHARSET_UTF_16, encodedText.GetCharacterSet());
			Assert.AreEqual(UNICODE_TEST_STRING, encodedText.ToString());
			byte[] bytes;
			EncodedText encodedText2;
			// no bom & no terminator
			bytes = encodedText.ToBytes();
			Assert.AreEqual(UNICODE_TEST_STRING_HEX_UTF16LE, TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// bom & no terminator
			bytes = encodedText.ToBytes(true);
			Assert.AreEqual("ff fe " + UNICODE_TEST_STRING_HEX_UTF16LE, TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// no bom & terminator
			bytes = encodedText.ToBytes(false, true);
			Assert.AreEqual(UNICODE_TEST_STRING_HEX_UTF16LE + " 00 00", TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// bom & terminator
			bytes = encodedText.ToBytes(true, true);
			Assert.AreEqual("ff fe " + UNICODE_TEST_STRING_HEX_UTF16LE + " 00 00", TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, bytes);
			Assert.AreEqual(encodedText, encodedText2);
		}

        [TestCase]
		public virtual void TestShouldEncodeAndDecodeUTF16BEText()
		{
			EncodedText encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16BE, UNICODE_TEST_STRING);
			Assert.AreEqual(EncodedText.CHARSET_UTF_16BE, encodedText.GetCharacterSet());
			Assert.AreEqual(UNICODE_TEST_STRING, encodedText.ToString());
			byte[] bytes;
			EncodedText encodedText2;
			// no bom & no terminator
			bytes = encodedText.ToBytes();
			Assert.AreEqual(UNICODE_TEST_STRING_HEX_UTF16BE, TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16BE, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// bom & no terminator
			bytes = encodedText.ToBytes(true);
			Assert.AreEqual("fe ff " + UNICODE_TEST_STRING_HEX_UTF16BE, TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16BE, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// no bom & terminator
			bytes = encodedText.ToBytes(false, true);
			Assert.AreEqual(UNICODE_TEST_STRING_HEX_UTF16BE + " 00 00", TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16BE, bytes);
			Assert.AreEqual(encodedText, encodedText2);
			// bom & terminator
			bytes = encodedText.ToBytes(true, true);
			Assert.AreEqual("fe ff " + UNICODE_TEST_STRING_HEX_UTF16BE + " 00 00", TestHelper.BytesToHexString(bytes));
			encodedText2 = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16BE, bytes);
			Assert.AreEqual(encodedText, encodedText2);
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionWhenEncodingWithInvalidCharacterSet()
		{
			try
			{
				new EncodedText(unchecked((byte)4), TEST_STRING);
				Assert.Fail("IllegalArgumentException expected but not thrown");
			}
			catch (ArgumentException e)
			{
				Assert.AreEqual("Invalid text encoding 4", e.Message);
			}
		}

        [TestCase]
		public virtual void TestShouldInferISO8859_1EncodingFromBytesWithNoBOM()
		{
			EncodedText encodedText = new EncodedText(TestHelper.HexStringToBytes(TEST_STRING_HEX_ISO8859_1));
			Assert.AreEqual(EncodedText.TEXT_ENCODING_ISO_8859_1, encodedText
				.GetTextEncoding());
		}

        [TestCase]
		public virtual void TestShouldDetectUTF8EncodingFromBytesWithBOM()
		{
			EncodedText encodedText = new EncodedText(TestHelper.HexStringToBytes("ef bb bf " + UNICODE_TEST_STRING_HEX_UTF8));
			Assert.AreEqual(EncodedText.TEXT_ENCODING_UTF_8, encodedText.GetTextEncoding());
		}

        [TestCase]
		public virtual void TestShouldDetectUTF16EncodingFromBytesWithBOM()
		{
			EncodedText encodedText = new EncodedText(TestHelper.HexStringToBytes("ff fe " + UNICODE_TEST_STRING_HEX_UTF16LE));
			Assert.AreEqual(EncodedText.TEXT_ENCODING_UTF_16, encodedText.GetTextEncoding());
		}

        [TestCase]
		public virtual void TestShouldDetectUTF16BEEncodingFromBytesWithBOM()
		{
			EncodedText encodedText = new EncodedText(TestHelper.HexStringToBytes("fe ff " + UNICODE_TEST_STRING_HEX_UTF16BE));
			Assert.AreEqual(EncodedText.TEXT_ENCODING_UTF_16BE, encodedText.GetTextEncoding());
		}

        [TestCase]
		public virtual void TestShouldTranscodeFromOneEncodingToAnother()
		{
			EncodedText encodedText;
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, TestHelper.HexStringToBytes("43 61 66 c3 a9 20 50 61 72 61 64 69 73 6f"));
			encodedText.SetTextEncoding(EncodedText.TEXT_ENCODING_ISO_8859_1, true);
			Assert.AreEqual("43 61 66 e9 20 50 61 72 61 64 69 73 6f", TestHelper.BytesToHexString(encodedText.ToBytes()));
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, TestHelper.HexStringToBytes("43 61 66 c3 a9 20 50 61 72 61 64 69 73 6f"));
			encodedText.SetTextEncoding(EncodedText.TEXT_ENCODING_UTF_8, true);
			Assert.AreEqual("43 61 66 c3 a9 20 50 61 72 61 64 69 73 6f", TestHelper.BytesToHexString(encodedText.ToBytes()));
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, TestHelper.HexStringToBytes("43 61 66 c3 a9 20 50 61 72 61 64 69 73 6f"));
			encodedText.SetTextEncoding(EncodedText.TEXT_ENCODING_UTF_16, true);
			Assert.AreEqual("43 00 61 00 66 00 e9 00 20 00 50 00 61 00 72 00 61 00 64 00 69 00 73 00 6f 00", TestHelper.BytesToHexString(encodedText.ToBytes()));
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, TestHelper.HexStringToBytes("43 61 66 c3 a9 20 50 61 72 61 64 69 73 6f"));
			encodedText.SetTextEncoding(EncodedText.TEXT_ENCODING_UTF_16BE, true);
			Assert.AreEqual("00 43 00 61 00 66 00 e9 00 20 00 50 00 61 00 72 00 61 00 64 00 69 00 73 00 6f", TestHelper.BytesToHexString(encodedText.ToBytes()));
		}

        [TestCase]
		public virtual void TestShouldThrowAnExceptionWhenAttemptingToTranscodeToACharacterSetWithUnmappableCharacters()
		{
			EncodedText encodedText;
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, UNICODE_TEST_STRING);
			try
			{
				encodedText.SetTextEncoding(EncodedText.TEXT_ENCODING_ISO_8859_1, true);
				Assert.Fail("CharacterCodingException expected but not thrown");
			}
			catch (CharacterCodingException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldThrowExceptionWhenTranscodingWithInvalidCharacterSet()
		{
			EncodedText encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_8, TestHelper.HexStringToBytes("43 61 66 c3 a9 20 50 61 72 61 64 69 73 6f"));
			try
			{
				encodedText.SetTextEncoding(unchecked((byte)4), true);
				Assert.Fail("IllegalArgumentException expected but not thrown");
			}
			catch (ArgumentException e)
			{
				Assert.AreEqual("Invalid text encoding 4", e.Message);
			}
		}

        [TestCase]
		public virtual void TestShouldReturnNullWhenDecodingInvalidString()
		{
			string s = "Not unicode";
			byte[] notUnicode = BufferTools.StringToByteBuffer(s, 0, s.Length);
			EncodedText encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, notUnicode);
			Assert.IsNull(encodedText.ToString());
		}

        [TestCase]
		public virtual void TestShouldHandleBacktickCharacterInString()
		{
			EncodedText encodedText = new EncodedText(unchecked((byte)0), BUFFER_WITH_A_BACKTICK);
			Assert.AreEqual("I" + (char)(96) + "m", encodedText.ToString());
		}

        [TestCase]
		public virtual void TestShouldStillReturnBytesWhenStringIsEmpty()
		{
			EncodedText encodedText = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, string.Empty);
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(false, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(true, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0 }, encodedText.ToBytes(false, true)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0 }, encodedText.ToBytes(true, true)));
		}

        [TestCase]
		public virtual void TestShouldStillReturnBytesWhenUnicodeStringIsEmpty()
		{
			EncodedText encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, string.Empty);
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(false, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))) }, encodedText.ToBytes(true, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0, 0 }, encodedText.ToBytes(false, true)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))), 0, 0 }, encodedText.ToBytes(true, true)));
		}

        [TestCase]
		public virtual void TestShouldStillReturnBytesWhenDataIsEmpty()
		{
			EncodedText encodedText;
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, new byte[] { });
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(false, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(true, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0 }, encodedText.ToBytes(false, true)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0 }, encodedText.ToBytes(true, true)));
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, new byte[] { 0 });
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(false, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(true, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0 }, encodedText.ToBytes(false, true)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0 }, encodedText.ToBytes(true, true)));
		}

        [TestCase]
		public virtual void TestShouldStillReturnBytesWhenUnicodeDataIsEmpty()
		{
			EncodedText encodedText;
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, new byte[] {  });
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(false, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))) }, encodedText.ToBytes(true, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0, 0 }, encodedText.ToBytes(false, true)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))), 0, 0 }, encodedText.ToBytes(true, true)));
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, new byte[] { 0, 0});
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(false, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))) }, encodedText.ToBytes(true, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0, 0 }, encodedText.ToBytes(false, true)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))), 0, 0 }, encodedText.ToBytes(true, true)));
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))) });
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(false, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))) }, encodedText.ToBytes(true, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0, 0 }, encodedText.ToBytes(false, true)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))), 0, 0 }, encodedText.ToBytes(true, true)));
			encodedText = new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))), 0, 0 });
			Assert.IsTrue(Arrays.Equals(new byte[] {  }, encodedText.ToBytes(false, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))) }, encodedText.ToBytes(true, false)));
			Assert.IsTrue(Arrays.Equals(new byte[] { 0, 0 }, encodedText.ToBytes(false, true)));
			Assert.IsTrue(Arrays.Equals(new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))), 0, 0 }, encodedText.ToBytes(true, true)));
		}
	}
}
