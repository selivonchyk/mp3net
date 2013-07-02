using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class ID3v2FrameTest
	{
		private static readonly string T_FRAME = "TPE1000 000ABCDEFGHIJKLMNOPQRSTUVWXYZABCDE";

		private static readonly string W_FRAME = "WXXX000!0000ABCDEFGHIJKLMNOPQRSTUVWXYZABCDE";

		private static readonly string C_FRAME = "COMM000$0000000ABCDEFGHIJKLMNOPQRSTUVWXYZABCDE";

        [TestCase]
		public virtual void TestShouldReadValid32TFrame()
		{
			byte[] bytes = BufferTools.StringToByteBuffer("xxxxx" + T_FRAME, 0, 5 + T_FRAME.Length);
			TestHelper.ReplaceNumbersWithBytes(bytes, 9);
			ID3v2Frame frame = new ID3v2Frame(bytes, 5);
			Assert.AreEqual(42, frame.GetLength());
			Assert.AreEqual("TPE1", frame.GetId());
			string s = "0ABCDEFGHIJKLMNOPQRSTUVWXYZABCDE";
			byte[] expectedBytes = BufferTools.StringToByteBuffer(s, 0, s.Length);
			TestHelper.ReplaceNumbersWithBytes(expectedBytes, 0);
			Assert.IsTrue(Arrays.Equals(expectedBytes, frame.GetData()));
		}

        [TestCase]
		public virtual void TestShouldReadValid32WFrame()
		{
			byte[] bytes = BufferTools.StringToByteBuffer(W_FRAME + "xxxxx", 0, W_FRAME.Length);
			TestHelper.ReplaceNumbersWithBytes(bytes, 0);
			ID3v2Frame frame = new ID3v2Frame(bytes, 0);
			Assert.AreEqual(43, frame.GetLength());
			Assert.AreEqual("WXXX", frame.GetId());
			string s = "00ABCDEFGHIJKLMNOPQRSTUVWXYZABCDE";
			byte[] expectedBytes = BufferTools.StringToByteBuffer(s, 0, s.Length);
			TestHelper.ReplaceNumbersWithBytes(expectedBytes, 0);
			Assert.IsTrue(Arrays.Equals(expectedBytes, frame.GetData()));
		}

        [TestCase]
		public virtual void TestShouldReadValid32CFrame()
		{
			byte[] bytes = BufferTools.StringToByteBuffer(C_FRAME, 0, C_FRAME.Length);
			TestHelper.ReplaceNumbersWithBytes(bytes, 0);
			ID3v2Frame frame = new ID3v2Frame(bytes, 0);
			Assert.AreEqual(46, frame.GetLength());
			Assert.AreEqual("COMM", frame.GetId());
			string s = "00000ABCDEFGHIJKLMNOPQRSTUVWXYZABCDE";
			byte[] expectedBytes = BufferTools.StringToByteBuffer(s, 0, s.Length);
			TestHelper.ReplaceNumbersWithBytes(expectedBytes, 0);
			Assert.IsTrue(Arrays.Equals(expectedBytes, frame.GetData()));
		}

        [TestCase]
		public virtual void TestShouldPackAndUnpackHeaderToGiveEquivalentObject()
		{
			byte[] bytes = new byte[26];
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = unchecked((byte)((byte)('A') + i));
			}
			ID3v2Frame frame = new ID3v2Frame("TEST", bytes);
			byte[] newBytes = frame.ToBytes();
			ID3v2Frame frameCopy = new ID3v2Frame(newBytes, 0);
			Assert.AreEqual("TEST", frameCopy.GetId());
			Assert.AreEqual(frame, frameCopy);
		}
	}
}
