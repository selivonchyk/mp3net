using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2ObseleteFrameTest
	{
		private static readonly string T_FRAME = "TP100\"0ARTISTABCDEFGHIJKLMNOPQRSTUVWXYZ0";

		private static readonly string LONG_T_FRAME = "TP10110Metamorphosis A a very long album B a very long album C a very long album D a very long album E a very long album F a very long album G a very long album H a very long album I a very long album J a very long album K a very long album L a very long album M0";

        [TestCase]
		public virtual void TestShouldReadValidLong32ObseleteTFrame()
		{
			byte[] bytes = BufferTools.StringToByteBuffer(LONG_T_FRAME, 0, LONG_T_FRAME.Length);
			TestHelper.ReplaceNumbersWithBytes(bytes, 3);
			ID3v2ObseleteFrame frame = new ID3v2ObseleteFrame(bytes, 0);
			Assert.AreEqual(263, frame.GetLength());
			Assert.AreEqual("TP1", frame.GetId());
			string s = "0Metamorphosis A a very long album B a very long album C a very long album D a very long album E a very long album F a very long album G a very long album H a very long album I a very long album J a very long album K a very long album L a very long album M0";
			byte[] expectedBytes = BufferTools.StringToByteBuffer(s, 0, s.Length);
			TestHelper.ReplaceNumbersWithBytes(expectedBytes, 0);
			Assert.IsTrue(Arrays.Equals(expectedBytes, frame.GetData()));
		}

        [TestCase]
		public virtual void TestShouldReadValid32ObseleteTFrame()
		{
			byte[] bytes = BufferTools.StringToByteBuffer("xxxxx" + T_FRAME, 0, 5 + T_FRAME.Length);
			TestHelper.ReplaceNumbersWithBytes(bytes, 8);
			ID3v2ObseleteFrame frame = new ID3v2ObseleteFrame(bytes, 5);
			Assert.AreEqual(40, frame.GetLength());
			Assert.AreEqual("TP1", frame.GetId());
			string s = "0ARTISTABCDEFGHIJKLMNOPQRSTUVWXYZ0";
			byte[] expectedBytes = BufferTools.StringToByteBuffer(s, 0, s.Length);
			TestHelper.ReplaceNumbersWithBytes(expectedBytes, 0);
			Assert.IsTrue(Arrays.Equals(expectedBytes, frame.GetData()));
		}
	}
}
