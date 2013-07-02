using System;
using System.IO;
using Mp3net.Helpers;

namespace Mp3net
{
	public class FileWrapper
	{
		protected internal FilePath file;

		protected internal string filename;

		protected internal long length;

		protected internal long lastModified;

		public FileWrapper()
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		public FileWrapper(string filename)
		{
		    if (String.IsNullOrEmpty(filename))
		    {
		        throw new ArgumentNullException("File name can not be null.");
		    }
			this.filename = filename;
			Init();
			length = file.Length();
			lastModified = file.LastModified();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Init()
		{
			file = new FilePath(filename);
			if (!file.Exists())
			{
				throw new FileNotFoundException("File not found " + filename);
			}
            /*
			if (!file.CanRead())
			{
				throw new IOException("File not readable");
			}
             */
		}

		public virtual string GetFilename()
		{
			return filename;
		}

		public virtual long GetLength()
		{
			return length;
		}

		public virtual long GetLastModified()
		{
			return lastModified;
		}
	}
}
