using System.Text;
using Mp3net.Helpers;

namespace Mp3net
{
	public class BufferTools
	{
		protected internal static readonly string defaultCharsetName = "ISO-8859-1";

		public static string ByteBufferToStringIgnoringEncodingIssues(byte[] bytes, int offset, int length)
		{
			try
			{
				return ByteBufferToString(bytes, offset, length, defaultCharsetName);
			}
			catch (UnsupportedEncodingException)
			{
				return null;
			}
		}

		/// <exception cref="System.IO.UnsupportedEncodingException"></exception>
		public static string ByteBufferToString(byte[] bytes, int offset, int length)
		{
			return ByteBufferToString(bytes, offset, length, defaultCharsetName);
		}

		/// <exception cref="System.IO.UnsupportedEncodingException"></exception>
		public static string ByteBufferToString(byte[] bytes, int offset, int length, string charsetName)
		{
			if (length < 1)
			{
				return string.Empty;
			}
			return Runtime.GetStringForBytes(bytes, offset, length, charsetName);
		}

		public static byte[] StringToByteBufferIgnoringEncodingIssues(string s, int offset, int length)
		{
			try
			{
				return StringToByteBuffer(s, offset, length);
			}
			catch (UnsupportedEncodingException)
			{
				return null;
			}
		}

		/// <exception cref="System.IO.UnsupportedEncodingException"></exception>
		public static byte[] StringToByteBuffer(string s, int offset, int length)
		{
			return StringToByteBuffer(s, offset, length, defaultCharsetName);
		}

		/// <exception cref="UnsupportedEncodingException"></exception>
		public static byte[] StringToByteBuffer(string s, int offset, int length, string charsetName)
		{
			string stringToCopy = s.Substring(offset, length);
			byte[] bytes = Runtime.GetBytesForString(stringToCopy, charsetName);
			return bytes;
		}

		/// <exception cref="System.IO.UnsupportedEncodingException"></exception>
		public static void StringIntoByteBuffer(string s, int offset, int length, byte[] bytes, int destOffset)
		{
			StringIntoByteBuffer(s, offset, length, bytes, destOffset, defaultCharsetName);
		}

		/// <exception cref="System.IO.UnsupportedEncodingException"></exception>
		public static void StringIntoByteBuffer(string s, int offset, int length, byte[] bytes, int destOffset, string charsetName)
		{
			string stringToCopy = s.Substring(offset, length);
			byte[] srcBytes = Runtime.GetBytesForString(stringToCopy, charsetName);
			if (srcBytes.Length > 0)
			{
				System.Array.Copy(srcBytes, 0, bytes, destOffset, srcBytes.Length);
			}
		}

		public static string TrimStringRight(string s)
		{
			int endPosition = s.Length - 1;
			char endChar;
			while (endPosition >= 0)
			{
				endChar = s[endPosition];
				if (endChar > 32)
				{
					break;
				}
				endPosition--;
			}
			if (endPosition == s.Length - 1)
			{
				return s;
			}
			else
			{
				if (endPosition < 0)
				{
					return string.Empty;
				}
			}
			return s.Substring(0, endPosition + 1);
		}

		public static string PadStringRight(string s, int length, char padWith)
		{
			if (s.Length >= length)
			{
				return s;
			}
			StringBuilder stringBuffer = new StringBuilder(s);
			while (stringBuffer.Length < length)
			{
				stringBuffer.Append(padWith);
			}
			return stringBuffer.ToString();
		}

		public static bool CheckBit(byte b, int bitPosition)
		{
			return ((b & (unchecked((int)(0x01)) << bitPosition)) != 0);
		}

		public static byte SetBit(byte b, int bitPosition, bool value)
		{
			byte newByte;
			if (value)
			{
				newByte = unchecked((byte)(b | (unchecked((byte)unchecked((int)(0x01))) << bitPosition
					)));
			}
			else
			{
				newByte = unchecked((byte)(b & (~(unchecked((byte)unchecked((int)(0x01))) << bitPosition
					))));
			}
			return newByte;
		}

		public static int ShiftByte(byte c, int places)
		{
			int i = c & unchecked((int)(0xff));
			if (places < 0)
			{
				return i << -places;
			}
			else
			{
				if (places > 0)
				{
					return i >> places;
				}
			}
			return i;
		}

		public static int UnpackInteger(byte b1, byte b2, byte b3, byte b4)
		{
			int value = b4 & unchecked((int)(0xff));
			value += BufferTools.ShiftByte(b3, -8);
			value += BufferTools.ShiftByte(b2, -16);
			value += BufferTools.ShiftByte(b1, -24);
			return value;
		}

		public static byte[] PackInteger(int i)
		{
			byte[] bytes = new byte[4];
			bytes[3] = unchecked((byte)(i & unchecked((int)(0xff))));
			bytes[2] = unchecked((byte)((i >> 8) & unchecked((int)(0xff))));
			bytes[1] = unchecked((byte)((i >> 16) & unchecked((int)(0xff))));
			bytes[0] = unchecked((byte)((i >> 24) & unchecked((int)(0xff))));
			return bytes;
		}

		public static int UnpackSynchsafeInteger(byte b1, byte b2, byte b3, byte b4)
		{
			int value = (unchecked((byte)(b4 & unchecked((int)(0x7f)))));
			value += ShiftByte(unchecked((byte)(b3 & unchecked((int)(0x7f)))), -7);
			value += ShiftByte(unchecked((byte)(b2 & unchecked((int)(0x7f)))), -14);
			value += ShiftByte(unchecked((byte)(b1 & unchecked((int)(0x7f)))), -21);
			return value;
		}

		public static byte[] PackSynchsafeInteger(int i)
		{
			byte[] bytes = new byte[4];
			PackSynchsafeInteger(i, bytes, 0);
			return bytes;
		}

		public static void PackSynchsafeInteger(int i, byte[] bytes, int offset)
		{
			bytes[offset + 3] = unchecked((byte)(i & unchecked((int)(0x7f))));
			bytes[offset + 2] = unchecked((byte)((i >> 7) & unchecked((int)(0x7f))));
			bytes[offset + 1] = unchecked((byte)((i >> 14) & unchecked((int)(0x7f))));
			bytes[offset + 0] = unchecked((byte)((i >> 21) & unchecked((int)(0x7f))));
		}

		public static byte[] CopyBuffer(byte[] bytes, int offset, int length)
		{
			byte[] copy = new byte[length];
			if (length > 0)
			{
				System.Array.Copy(bytes, offset, copy, 0, length);
			}
			return copy;
		}

		public static void CopyIntoByteBuffer(byte[] bytes, int offset, int length, byte[]
			 destBuffer, int destOffset)
		{
			if (length > 0)
			{
				System.Array.Copy(bytes, offset, destBuffer, destOffset, length);
			}
		}

		public static int SizeUnsynchronisationWouldAdd(byte[] bytes)
		{
			int count = 0;
			for (int i = 0; i < bytes.Length - 1; i++)
			{
				if (bytes[i] == unchecked((byte)unchecked((int)(0xff))) && ((bytes[i + 1] & unchecked(
					(byte)unchecked((int)(0xe0)))) == unchecked((byte)unchecked((int)(0xe0))) || bytes
					[i + 1] == 0))
				{
					count++;
				}
			}
			if (bytes.Length > 0 && bytes[bytes.Length - 1] == unchecked((byte)unchecked((int
				)(0xff))))
			{
				count++;
			}
			return count;
		}

		public static byte[] UnsynchroniseBuffer(byte[] bytes)
		{
			// unsynchronisation is replacing instances of:
			// 11111111 111xxxxx with 11111111 00000000 111xxxxx and
			// 11111111 00000000 with 11111111 00000000 00000000 
			int count = SizeUnsynchronisationWouldAdd(bytes);
			if (count == 0)
			{
				return bytes;
			}
			byte[] newBuffer = new byte[bytes.Length + count];
			int j = 0;
			for (int i = 0; i < bytes.Length - 1; i++)
			{
				newBuffer[j++] = bytes[i];
				if (bytes[i] == unchecked((byte)unchecked((int)(0xff))) && ((bytes[i + 1] & unchecked(
					(byte)unchecked((int)(0xe0)))) == unchecked((byte)unchecked((int)(0xe0))) || bytes
					[i + 1] == 0))
				{
					newBuffer[j++] = 0;
				}
			}
			newBuffer[j++] = bytes[bytes.Length - 1];
			if (bytes[bytes.Length - 1] == unchecked((byte)unchecked((int)(0xff))))
			{
				newBuffer[j++] = 0;
			}
			return newBuffer;
		}

		public static int SizeSynchronisationWouldSubtract(byte[] bytes)
		{
			int count = 0;
			for (int i = 0; i < bytes.Length - 2; i++)
			{
				if (bytes[i] == unchecked((byte)unchecked((int)(0xff))) && bytes[i + 1] == 0 && (
					(bytes[i + 2] & unchecked((byte)unchecked((int)(0xe0)))) == unchecked((byte)unchecked(
					(int)(0xe0))) || bytes[i + 2] == 0))
				{
					count++;
				}
			}
			if (bytes.Length > 1 && bytes[bytes.Length - 2] == unchecked((byte)unchecked((int
				)(0xff))) && bytes[bytes.Length - 1] == 0)
			{
				count++;
			}
			return count;
		}

		public static byte[] SynchroniseBuffer(byte[] bytes)
		{
			// synchronisation is replacing instances of:
			// 11111111 00000000 111xxxxx with 11111111 111xxxxx and
			// 11111111 00000000 00000000 with 11111111 00000000
			int count = SizeSynchronisationWouldSubtract(bytes);
			if (count == 0)
			{
				return bytes;
			}
			byte[] newBuffer = new byte[bytes.Length - count];
			int i = 0;
			for (int j = 0; j < newBuffer.Length - 1; j++)
			{
				newBuffer[j] = bytes[i];
				if (bytes[i] == unchecked((byte)unchecked((int)(0xff))) && bytes[i + 1] == 0 && (
					(bytes[i + 2] & unchecked((byte)unchecked((int)(0xe0)))) == unchecked((byte)unchecked(
					(int)(0xe0))) || bytes[i + 2] == 0))
				{
					i++;
				}
				i++;
			}
			newBuffer[newBuffer.Length - 1] = bytes[i];
			return newBuffer;
		}

		public static string Substitute(string s, string replaceThis, string withThis)
		{
			if (replaceThis.Length < 1 || s.IndexOf(replaceThis) < 0)
			{
				return s;
			}
			StringBuilder newString = new StringBuilder();
			int lastPosition = 0;
			int position = 0;
			while ((position = s.IndexOf(replaceThis, position)) >= 0)
			{
				if (position > lastPosition)
				{
					newString.Append(s.Substring(lastPosition, position - lastPosition));
				}
				if (withThis != null)
				{
					newString.Append(withThis);
				}
				lastPosition = position + replaceThis.Length;
				position++;
			}
			if (lastPosition < s.Length)
			{
				newString.Append(s.Substring(lastPosition));
			}
			return newString.ToString();
		}

		public static string AsciiOnly(string s)
		{
			StringBuilder newString = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				char ch = s[i];
				if (ch < 32 || ch > 126)
				{
					newString.Append('?');
				}
				else
				{
					newString.Append(ch);
				}
			}
			return newString.ToString();
		}

		public static int IndexOfTerminator(byte[] bytes)
		{
			return IndexOfTerminator(bytes, 0);
		}

		public static int IndexOfTerminator(byte[] bytes, int fromIndex)
		{
			return IndexOfTerminator(bytes, 0, 1);
		}

		public static int IndexOfTerminator(byte[] bytes, int fromIndex, int terminatorLength
			)
		{
			int marker = -1;
			for (int i = fromIndex; i <= bytes.Length - terminatorLength; i++)
			{
				if ((i - fromIndex) % terminatorLength == 0)
				{
					int matched;
					for (matched = 0; matched < terminatorLength; matched++)
					{
						if (bytes[i + matched] != 0)
						{
							break;
						}
					}
					if (matched == terminatorLength)
					{
						marker = i;
						break;
					}
				}
			}
			return marker;
		}

		public static int IndexOfTerminatorForEncoding(byte[] bytes, int fromIndex, int encoding
			)
		{
			int terminatorLength = (encoding == EncodedText.TEXT_ENCODING_UTF_16 || encoding 
				== EncodedText.TEXT_ENCODING_UTF_16BE) ? 2 : 1;
			return IndexOfTerminator(bytes, fromIndex, terminatorLength);
		}
	}
}
