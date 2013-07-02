using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2UrlFrameDataTest
	{
		private static readonly string TEST_DESCRIPTION = "DESCRIPTION";

		private static readonly string TEST_DESCRIPTION_UNICODE = "\u03B3\u03B5\u03B9\u03AC";

		private static readonly string TEST_URL = "http://ABCDEFGHIJKLMNOPQ";

        [TestCase]
		public virtual void TestShouldConsiderTwoEquivalentObjectsEqual()
		{
			ID3v2UrlFrameData frameData1 = new ID3v2UrlFrameData(false, new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), TEST_URL);
			ID3v2UrlFrameData frameData2 = new ID3v2UrlFrameData(false, new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), TEST_URL);
			Assert.AreEqual(frameData1, frameData2);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataToBytesAndBackToEquivalentObject()
		{
			ID3v2UrlFrameData frameData = new ID3v2UrlFrameData(false, new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), TEST_URL);
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { 0, (byte)('D'), (byte)('E'), (byte)('S'), (byte
				)('C'), (byte)('R'), (byte)('I'), (byte)('P'), (byte)('T'), (byte)('I'), (byte)(
				'O'), (byte)('N'), 0, (byte)('h'), (byte)('t'), (byte)('t'), (byte)('p'), (byte)
				(':'), (byte)('/'), (byte)('/'), (byte)('A'), (byte)('B'), (byte)('C'), (byte)('D'
				), (byte)('E'), (byte)('F'), (byte)('G'), (byte)('H'), (byte)('I'), (byte)('J'), 
				(byte)('K'), (byte)('L'), (byte)('M'), (byte)('N'), (byte)('O'), (byte)('P'), (byte
				)('Q') };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2UrlFrameData frameDataCopy = new ID3v2UrlFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataWithNoDescriptionToBytesAndBackToEquivalentObject()
		{
			ID3v2UrlFrameData frameData = new ID3v2UrlFrameData(false, new EncodedText(string.Empty), TEST_URL);
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { 0, 0, (byte)('h'), (byte)('t'), (byte)('t'), 
				(byte)('p'), (byte)(':'), (byte)('/'), (byte)('/'), (byte)('A'), (byte)('B'), (byte
				)('C'), (byte)('D'), (byte)('E'), (byte)('F'), (byte)('G'), (byte)('H'), (byte)(
				'I'), (byte)('J'), (byte)('K'), (byte)('L'), (byte)('M'), (byte)('N'), (byte)('O'
				), (byte)('P'), (byte)('Q') };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2UrlFrameData frameDataCopy = new ID3v2UrlFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataWithUnicodeDescriptionToBytesAndBackToEquivalentObject()
		{
			ID3v2UrlFrameData frameData = new ID3v2UrlFrameData(false, new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, TEST_DESCRIPTION_UNICODE), TEST_URL);
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { 1, unchecked((byte)unchecked((int)(0xff))), unchecked(
				(byte)unchecked((int)(0xfe))), unchecked((byte)unchecked((int)(0xb3))), unchecked(
				(int)(0x03)), unchecked((byte)unchecked((int)(0xb5))), unchecked((int)(0x03)), unchecked(
				(byte)unchecked((int)(0xb9))), unchecked((int)(0x03)), unchecked((byte)unchecked(
				(int)(0xac))), unchecked((int)(0x03)), 0, 0, (byte)('h'), (byte)('t'), (byte)('t'
				), (byte)('p'), (byte)(':'), (byte)('/'), (byte)('/'), (byte)('A'), (byte)('B'), 
				(byte)('C'), (byte)('D'), (byte)('E'), (byte)('F'), (byte)('G'), (byte)('H'), (byte
				)('I'), (byte)('J'), (byte)('K'), (byte)('L'), (byte)('M'), (byte)('N'), (byte)(
				'O'), (byte)('P'), (byte)('Q') };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2UrlFrameData frameDataCopy = new ID3v2UrlFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}
	}
}
