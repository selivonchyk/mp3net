using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2TextFrameDataTest
	{
		private static readonly string TEST_TEXT = "ABCDEFGHIJKLMNOPQ";

		private static readonly string TEST_TEXT_UNICODE = "\u03B3\u03B5\u03B9\u03AC";

        [TestCase]
		public virtual void TestShouldConsiderTwoEquivalentObjectsEqual()
		{
			ID3v2TextFrameData frameData1 = new ID3v2TextFrameData(false, new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, TEST_TEXT));
			ID3v2TextFrameData frameData2 = new ID3v2TextFrameData(false, new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, TEST_TEXT));
			Assert.AreEqual(frameData1, frameData2);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataToBytesAndBackToEquivalentObject()
		{
			ID3v2TextFrameData frameData = new ID3v2TextFrameData(false, new EncodedText(EncodedText.TEXT_ENCODING_ISO_8859_1, TEST_TEXT));
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { 0, (byte)('A'), (byte)('B'), (byte)('C'), (byte
				)('D'), (byte)('E'), (byte)('F'), (byte)('G'), (byte)('H'), (byte)('I'), (byte)(
				'J'), (byte)('K'), (byte)('L'), (byte)('M'), (byte)('N'), (byte)('O'), (byte)('P'
				), (byte)('Q') };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2TextFrameData frameDataCopy = new ID3v2TextFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataWithUnicodeToBytesAndBackToEquivalentObject()
		{
			ID3v2TextFrameData frameData = new ID3v2TextFrameData(false, new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, TEST_TEXT_UNICODE));
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { 1, unchecked((byte)unchecked((int)(0xff))), unchecked(
				(byte)unchecked((int)(0xfe))), unchecked((byte)unchecked((int)(0xb3))), unchecked(
				(int)(0x03)), unchecked((byte)unchecked((int)(0xb5))), unchecked((int)(0x03)), unchecked(
				(byte)unchecked((int)(0xb9))), unchecked((int)(0x03)), unchecked((byte)unchecked(
				(int)(0xac))), unchecked((int)(0x03)) };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2TextFrameData frameDataCopy = new ID3v2TextFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}
	}
}
