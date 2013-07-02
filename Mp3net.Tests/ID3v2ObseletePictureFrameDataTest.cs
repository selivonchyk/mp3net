using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2ObseletePictureFrameDataTest
	{
		private static readonly string TEST_MIME_TYPE = "image/png";

		private static readonly string TEST_DESCRIPTION = "DESCRIPTION";

		private static readonly string TEST_DESCRIPTION_UNICODE = "\u03B3\u03B5\u03B9\u03AC";

		private static readonly byte[] DUMMY_IMAGE_DATA = new byte[] { 1, 2, 3, 4, 5 };

        [TestCase]
		public virtual void TestShouldConsiderTwoEquivalentObjectsEqual()
		{
			ID3v2ObseletePictureFrameData frameData1 = new ID3v2ObseletePictureFrameData(false
				, TEST_MIME_TYPE, unchecked((byte)0), new EncodedText(unchecked((byte)1), TEST_DESCRIPTION
				), DUMMY_IMAGE_DATA);
			ID3v2ObseletePictureFrameData frameData2 = new ID3v2ObseletePictureFrameData(false
				, TEST_MIME_TYPE, unchecked((byte)0), new EncodedText(unchecked((byte)1), TEST_DESCRIPTION
				), DUMMY_IMAGE_DATA);
			Assert.AreEqual(frameData1, frameData2);
		}

        [TestCase]
		public virtual void TestShouldReadFrameData()
		{
			byte[] bytes = new byte[] { unchecked((int)(0x00)), (byte)('P'), (byte)('N'), (byte
				)('G'), unchecked((int)(0x01)), (byte)('D'), (byte)('E'), (byte)('S'), (byte)('C'
				), (byte)('R'), (byte)('I'), (byte)('P'), (byte)('T'), (byte)('I'), (byte)('O'), 
				(byte)('N'), unchecked((int)(0x00)), 1, 2, 3, 4, 5 };
			ID3v2ObseletePictureFrameData frameData = new ID3v2ObseletePictureFrameData(false, bytes);
			Assert.AreEqual(TEST_MIME_TYPE, frameData.GetMimeType());
			Assert.AreEqual(unchecked((byte)1), frameData.GetPictureType());
			Assert.AreEqual(new EncodedText(unchecked((byte)0), TEST_DESCRIPTION), frameData.GetDescription());
			Assert.IsTrue(Arrays.Equals(DUMMY_IMAGE_DATA, frameData.GetImageData()));
		}

        [TestCase]
		public virtual void TestShouldReadFrameDataWithUnicodeDescription()
		{
			byte[] bytes = new byte[] { unchecked((int)(0x01)), (byte)('P'), (byte)('N'), (byte
				)('G'), unchecked((int)(0x01)), unchecked((byte)unchecked((int)(0xff))), unchecked(
				(byte)unchecked((int)(0xfe))), unchecked((byte)unchecked((int)(0xb3))), unchecked(
				(int)(0x03)), unchecked((byte)unchecked((int)(0xb5))), unchecked((int)(0x03)), unchecked(
				(byte)unchecked((int)(0xb9))), unchecked((int)(0x03)), unchecked((byte)unchecked(
				(int)(0xac))), unchecked((int)(0x03)), 0, 0, 1, 2, 3, 4, 5 };
			ID3v2ObseletePictureFrameData frameData = new ID3v2ObseletePictureFrameData(false, bytes);
			Assert.AreEqual(TEST_MIME_TYPE, frameData.GetMimeType());
			Assert.AreEqual(unchecked((byte)1), frameData.GetPictureType());
			Assert.AreEqual(new EncodedText(EncodedText.TEXT_ENCODING_UTF_16, TEST_DESCRIPTION_UNICODE), frameData.GetDescription());
			Assert.IsTrue(Arrays.Equals(DUMMY_IMAGE_DATA, frameData.GetImageData()));
		}
	}
}
