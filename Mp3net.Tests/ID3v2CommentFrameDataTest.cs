using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2CommentFrameDataTest
	{
		private static readonly string TEST_LANGUAGE = "eng";

		private static readonly string TEST_DESCRIPTION = "DESCRIPTION";

		private static readonly string TEST_VALUE = "ABCDEFGHIJKLMNOPQ";

		private static readonly string TEST_DESCRIPTION_UNICODE = "\u03B3\u03B5\u03B9\u03AC";

		private static readonly string TEST_VALUE_UNICODE = "\u03C3\u03BF\u03C5";

        [TestCase]
		public virtual void TestShouldConsiderTwoEquivalentObjectsEqual()
		{
			ID3v2CommentFrameData frameData1 = new ID3v2CommentFrameData(false, TEST_LANGUAGE, new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), new EncodedText(unchecked((byte)0), TEST_VALUE));
			ID3v2CommentFrameData frameData2 = new ID3v2CommentFrameData(false, TEST_LANGUAGE, new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), new EncodedText(unchecked((byte)0), TEST_VALUE));
			Assert.AreEqual(frameData1, frameData2);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataToBytesAndBackToEquivalentObject()
		{
			ID3v2CommentFrameData frameData = new ID3v2CommentFrameData(false, TEST_LANGUAGE, new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), new EncodedText(unchecked((byte)0), TEST_VALUE));
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { 0, (byte)('e'), (byte)('n'), (byte)('g'), (byte
				)('D'), (byte)('E'), (byte)('S'), (byte)('C'), (byte)('R'), (byte)('I'), (byte)(
				'P'), (byte)('T'), (byte)('I'), (byte)('O'), (byte)('N'), 0, (byte)('A'), (byte)
				('B'), (byte)('C'), (byte)('D'), (byte)('E'), (byte)('F'), (byte)('G'), (byte)('H'
				), (byte)('I'), (byte)('J'), (byte)('K'), (byte)('L'), (byte)('M'), (byte)('N'), 
				(byte)('O'), (byte)('P'), (byte)('Q') };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2CommentFrameData frameDataCopy = new ID3v2CommentFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataWithBlankDescriptionAndLanguageToBytesAndBackToEquivalentObject()
		{
			byte[] bytes = new byte[] { 0, 0, 0, 0, 0, (byte)('A'), (byte)('B'), (byte)('C'), 
				(byte)('D'), (byte)('E'), (byte)('F'), (byte)('G'), (byte)('H'), (byte)('I'), (byte
				)('J'), (byte)('K'), (byte)('L'), (byte)('M'), (byte)('N'), (byte)('O'), (byte)(
				'P'), (byte)('Q') };
			ID3v2CommentFrameData frameData = new ID3v2CommentFrameData(false, bytes);
			Assert.AreEqual("\x0\x0\x0", frameData.GetLanguage());
			Assert.AreEqual(new EncodedText(string.Empty), frameData.GetDescription());
			Assert.AreEqual(new EncodedText(TEST_VALUE), frameData.GetComment());
			Assert.IsTrue(Arrays.Equals(bytes, frameData.ToBytes()));
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataWithUnicodeToBytesAndBackToEquivalentObject()
		{
			ID3v2CommentFrameData frameData = new ID3v2CommentFrameData(false, TEST_LANGUAGE, 
				new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, TEST_DESCRIPTION_UNICODE), new 
				EncodedText(EncodedText.TEXT_ENCODING_UTF_16, TEST_VALUE_UNICODE));
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { 1, (byte)('e'), (byte)('n'), (byte)('g'), unchecked(
				(byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xfe))), unchecked(
				(byte)unchecked((int)(0xb3))), unchecked((int)(0x03)), unchecked((byte)unchecked(
				(int)(0xb5))), unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xb9))), 
				unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xac))), unchecked((int)
				(0x03)), 0, 0, unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked(
				(int)(0xfe))), unchecked((byte)unchecked((int)(0xc3))), unchecked((int)(0x03)), 
				unchecked((byte)unchecked((int)(0xbf))), unchecked((int)(0x03)), unchecked((byte
				)unchecked((int)(0xc5))), unchecked((int)(0x03)) };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2CommentFrameData frameDataCopy = new ID3v2CommentFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}
	}
}
