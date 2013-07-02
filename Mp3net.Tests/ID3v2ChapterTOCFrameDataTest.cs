using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2ChapterTOCFrameDataTest
	{
        [TestCase]
		public virtual void TestShouldConsiderTwoEquivalentObjectsEqual()
		{
			string[] children = new string[] { "ch1", "ch2" };
			ID3v2ChapterTOCFrameData frameData1 = new ID3v2ChapterTOCFrameData(false, true, false, "toc1", children);
			ID3v2TextFrameData subFrameData1 = new ID3v2TextFrameData(false, new EncodedText("Hello there"));
			frameData1.AddSubframe("TIT2", subFrameData1);
			ID3v2ChapterTOCFrameData frameData2 = new ID3v2ChapterTOCFrameData(false, true, false, "toc1", children);
			ID3v2TextFrameData subFrameData2 = new ID3v2TextFrameData(false, new EncodedText("Hello there"));
			frameData2.AddSubframe("TIT2", subFrameData2);
			Assert.AreEqual(frameData1, frameData2);
		}

        [TestCase]
		public virtual void TestShouldConvertFrameDataToBytesAndBackToEquivalentObject()
		{
			string[] children = new string[] { "ch1", "ch2" };
			ID3v2ChapterTOCFrameData frameData = new ID3v2ChapterTOCFrameData(false, true, true, "toc1", children);
			ID3v2TextFrameData subFrameData = new ID3v2TextFrameData(false, new EncodedText("Hello there"));
			frameData.AddSubframe("TIT2", subFrameData);
			byte[] bytes = frameData.ToBytes();
			byte[] expectedBytes = new byte[] { (byte)('t'), (byte)('o'), (byte)('c'), (byte)
				('1'), 0, 3, 2, (byte)('c'), (byte)('h'), (byte)('1'), 0, (byte)('c'), (byte)('h'
				), (byte)('2'), 0, (byte)('T'), (byte)('I'), (byte)('T'), (byte)('2'), 0, 0, 0, 
				unchecked((byte)unchecked((int)(0xc))), 0, 0, 0, (byte)('H'), (byte)('e'), (byte
				)('l'), (byte)('l'), (byte)('o'), (byte)(' '), (byte)('t'), (byte)('h'), (byte)(
				'e'), (byte)('r'), (byte)('e') };
			Assert.IsTrue(Arrays.Equals(expectedBytes, bytes));
			ID3v2ChapterTOCFrameData frameDataCopy = new ID3v2ChapterTOCFrameData(false, bytes);
			Assert.AreEqual(frameData, frameDataCopy);
		}
	}
}
