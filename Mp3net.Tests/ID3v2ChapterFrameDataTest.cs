using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2ChapterFrameDataTest
	{
        [TestCase]
		public virtual void TestShouldConsiderTwoEquivalentObjectsEqual()
		{
			ID3v2ChapterFrameData frameData1 = new ID3v2ChapterFrameData(false, "ch1", 1, 380, 3, 400);
			ID3v2TextFrameData subFrameData1 = new ID3v2TextFrameData(false, new EncodedText("Hello there"));
			frameData1.AddSubframe("TIT2", subFrameData1);
			ID3v2ChapterFrameData frameData2 = new ID3v2ChapterFrameData(false, "ch1", 1, 380, 3, 400);
			ID3v2TextFrameData subFrameData2 = new ID3v2TextFrameData(false, new EncodedText("Hello there"));
			frameData2.AddSubframe("TIT2", subFrameData2);
			Assert.AreEqual(frameData1, frameData2);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataToBytesAndBackToEquivalentObject()
		{
			ID3v2ChapterFrameData frameData = new ID3v2ChapterFrameData(false, "ch1", 1, 380, 3, 400);
			ID3v2TextFrameData subFrameData = new ID3v2TextFrameData(false, new EncodedText("Hello there"));
			frameData.AddSubframe("TIT2", subFrameData);
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { (byte)('c'), (byte)('h'), (byte)('1'), 0, 0, 
				0, 0, 1, 0, 0, 1, unchecked((byte)unchecked((int)(0x7c))), 0, 0, 0, 3, 0, 0, 1, 
				unchecked((byte)unchecked((int)(0x90))), (byte)('T'), (byte)('I'), (byte)('T'), 
				(byte)('2'), 0, 0, 0, unchecked((byte)unchecked((int)(0xc))), 0, 0, 0, (byte)('H'
				), (byte)('e'), (byte)('l'), (byte)('l'), (byte)('o'), (byte)(' '), (byte)('t'), 
				(byte)('h'), (byte)('e'), (byte)('r'), (byte)('e') };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2ChapterFrameData frameDataCopy = new ID3v2ChapterFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}
	}
}
