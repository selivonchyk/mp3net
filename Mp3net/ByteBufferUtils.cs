using Mp3net.Helpers;

namespace Mp3net
{
	public class ByteBufferUtils
	{
		public static string ExtractNullTerminatedString(ByteBuffer bb)
		{
			int start = bb.Position();
			byte[] buffer = new byte[bb.Remaining()];
			bb.Get(buffer);
			string s = Runtime.GetStringForBytes(buffer);
			int nullPos = s.IndexOf('\0');
			s = s.Substring(0, nullPos);
			bb.Position(start + s.Length + 1);
			return s;
		}
	}
}
