namespace Mp3net.Helpers
{
	using System;

	public interface FilenameFilter
	{
		bool Accept (FilePath dir, string name);
	}
}
