namespace Mp3net.Helpers
{
	using System;

	internal class FileReader : InputStreamReader
	{
		public FileReader (FilePath f) : base(f.GetPath ())
		{
		}
	}
}
