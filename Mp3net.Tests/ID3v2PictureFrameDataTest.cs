using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2PictureFrameDataTest
	{
		private const byte BYTE_FF = unchecked((byte)(-unchecked((int)(0x01))));

		private const byte BYTE_FB = unchecked((byte)(-unchecked((int)(0x05))));

		private static readonly string TEST_MIME_TYPE = "mime/type";

		private static readonly string TEST_DESCRIPTION = "DESCRIPTION";

		private static readonly string TEST_DESCRIPTION_UNICODE = "\u03B3\u03B5\u03B9\u03AC";

		private static readonly byte[] DUMMY_IMAGE_DATA = new byte[] { 1, 2, 3, 4, 5 };

        [TestCase]
		public virtual void TestShouldConsiderTwoEquivalentObjectsEqual()
		{
			ID3v2PictureFrameData frameData1 = new ID3v2PictureFrameData(false, TEST_MIME_TYPE, unchecked((byte)3), new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), DUMMY_IMAGE_DATA);
			ID3v2PictureFrameData frameData2 = new ID3v2PictureFrameData(false, TEST_MIME_TYPE, unchecked((byte)3), new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), DUMMY_IMAGE_DATA);
			Assert.AreEqual(frameData1, frameData2);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataToBytesAndBackToEquivalentObject()
		{
			ID3v2PictureFrameData frameData = new ID3v2PictureFrameData(false, TEST_MIME_TYPE, unchecked((byte)3), new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), DUMMY_IMAGE_DATA);
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { unchecked((int)(0x00)), (byte)('m'), (byte)('i'
				), (byte)('m'), (byte)('e'), (byte)('/'), (byte)('t'), (byte)('y'), (byte)('p'), 
				(byte)('e'), 0, unchecked((int)(0x03)), (byte)('D'), (byte)('E'), (byte)('S'), (
				byte)('C'), (byte)('R'), (byte)('I'), (byte)('P'), (byte)('T'), (byte)('I'), (byte
				)('O'), (byte)('N'), 0, 1, 2, 3, 4, 5 };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2PictureFrameData frameDataCopy = new ID3v2PictureFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataWithUnicodeDescriptionToBytesAndBackToEquivalentObject()
		{
			ID3v2PictureFrameData frameData = new ID3v2PictureFrameData(false, TEST_MIME_TYPE, unchecked((byte)3), new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, TEST_DESCRIPTION_UNICODE), DUMMY_IMAGE_DATA);
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { unchecked((int)(0x01)), (byte)('m'), (byte)('i'
				), (byte)('m'), (byte)('e'), (byte)('/'), (byte)('t'), (byte)('y'), (byte)('p'), 
				(byte)('e'), 0, unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xff))), 
				unchecked((byte)unchecked((int)(0xfe))), unchecked((byte)unchecked((int)(0xb3)))
				, unchecked((int)(0x03)), unchecked((byte)unchecked((int)(0xb5))), unchecked((int
				)(0x03)), unchecked((byte)unchecked((int)(0xb9))), unchecked((int)(0x03)), unchecked(
				(byte)unchecked((int)(0xac))), unchecked((int)(0x03)), 0, 0, 1, 2, 3, 4, 5 };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2PictureFrameData frameDataCopy = new ID3v2PictureFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}

        [TestCase]
		public virtual void TestShouldUnsynchroniseAndSynchroniseDataWhenPackingAndUnpacking()
		{
			byte[] data = new byte[] { unchecked((int)(0x00)), (byte)('m'), (byte)('i'), (byte
				)('m'), (byte)('e'), (byte)('/'), (byte)('t'), (byte)('y'), (byte)('p'), (byte)(
				'e'), 0, unchecked((int)(0x03)), (byte)('D'), (byte)('E'), (byte)('S'), (byte)('C'
				), (byte)('R'), (byte)('I'), (byte)('P'), (byte)('T'), (byte)('I'), (byte)('O'), 
				(byte)('N'), 0, 1, 2, 3, BYTE_FF, unchecked((int)(0x00)), BYTE_FB, BYTE_FF, unchecked(
				(int)(0x00)), BYTE_FB, BYTE_FF, 0, 0, 4, 5 };
			ID3v2PictureFrameData frameData = new ID3v2PictureFrameData(true, data);
			byte[] expectedImageData = new byte[] { 1, 2, 3, BYTE_FF, BYTE_FB, BYTE_FF, BYTE_FB, BYTE_FF, 0, 4, 5 };
			Assert.IsTrue(Arrays.Equals(expectedImageData, frameData.GetImageData()));
			byte[] bytes = frameData.ToBytes();
			Assert.IsTrue(Arrays.Equals(data, bytes));
		}
	}
}
