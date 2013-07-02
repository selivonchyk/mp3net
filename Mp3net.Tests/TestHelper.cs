using System;
using System.Text;
using Mp3net.Helpers;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class TestHelper
	{
		public static string BytesToHexString(byte[] bytes)
		{
			StringBuilder hexString = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				if (i > 0)
				{
					hexString.Append(' ');
				}
				string hex = Mp3net.Helpers.Extensions.ToHexString(unchecked((int)(0xff)) & bytes
					[i]);
				if (hex.Length == 1)
				{
					hexString.Append('0');
				}
				hexString.Append(hex);
			}
			return hexString.ToString();
		}

		public static byte[] HexStringToBytes(string hex)
		{
			int len = hex.Length;
			byte[] bytes = new byte[(len + 1) / 3];
			for (int i = 0; i < len; i += 3)
			{
                bytes[i / 3] = unchecked((byte)((Convert.ToInt32(hex.Substring(i, 1), 16) << 4) + Convert.ToInt32(hex.Substring(i+1, 1), 16)));
			}
			return bytes;
		}

		public static byte[] LoadFile(string filename)
		{
			RandomAccessFile file = new RandomAccessFile(filename, "r");
			byte[] buffer = new byte[(int)file.Length()];
			file.Read(buffer);
			file.Close();
			return buffer;
		}

		public static void DeleteFile(string filename)
		{
			FilePath file = new FilePath(filename);
			file.Delete();
		}

		public static void ReplaceSpacesWithNulls(byte[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				if (buffer[i] == unchecked((int)(0x20)))
				{
					buffer[i] = unchecked((int)(0x00));
				}
			}
		}

		public static void ReplaceNumbersWithBytes(byte[] bytes, int offset)
		{
			for (int i = offset; i < bytes.Length; i++)
			{
				if (bytes[i] >= '0' && ((sbyte)bytes[i]) <= '9')
				{
					bytes[i] -= unchecked((byte)48);
				}
			}
		}

        [TestCase]
		public virtual void TestShouldConvertBytesToHexAndBack()
		{
			byte[] bytes = new byte[] { unchecked((byte)unchecked((int)(0x48))), unchecked((byte
				)unchecked((int)(0x45))), unchecked((byte)unchecked((int)(0x4C))), unchecked((byte
				)unchecked((int)(0x4C))), unchecked((byte)unchecked((int)(0x4F))), unchecked((byte
				)unchecked((int)(0x20))), unchecked((byte)unchecked((int)(0x74))), unchecked((byte
				)unchecked((int)(0x68))), unchecked((byte)unchecked((int)(0x65))), unchecked((byte
				)unchecked((int)(0x72))), unchecked((byte)unchecked((int)(0x65))), unchecked((byte
				)unchecked((int)(0x21))) };
			string hexString = TestHelper.BytesToHexString(bytes);
			Assert.AreEqual("48 45 4c 4c 4f 20 74 68 65 72 65 21", hexString);
			Assert.IsTrue(Arrays.Equals(bytes, TestHelper.HexStringToBytes(hexString)));
		}
	}
}
