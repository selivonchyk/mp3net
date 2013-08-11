using System;
using System.Collections.Generic;
using System.IO;
using Mp3net.Helpers;

namespace Mp3net
{
	public class Mp3File
	{
		private const int DEFAULT_BUFFER_LENGTH = 65536;

		private const int MINIMUM_BUFFER_LENGTH = 40;

		private const int XING_MARKER_OFFSET_1 = 13;

		private const int XING_MARKER_OFFSET_2 = 21;

		private const int XING_MARKER_OFFSET_3 = 36;

		protected internal int bufferLength;

		private int xingOffset = -1;

		private int startOffset = -1;

		private int endOffset = -1;

		private int frameCount = 0;

		private IDictionary<int, MutableInteger> bitrates = new Dictionary<int, MutableInteger>();

		private int xingBitrate;

		private double bitrate = 0;

		private string channelMode;

		private string emphasis;

		private string layer;

		private string modeExtension;

		private int sampleRate;

		private bool copyright;

		private bool original;

		private string version;

		private ID3v1 id3v1Tag;

		private ID3v2 id3v2Tag;

		private byte[] customTag;

		private bool scanFile;

        private string filename;

        private long length;

        private DateTime lastModified;

        private Stream stream;

		public Mp3File()
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public Mp3File(string filename) : this(filename, DEFAULT_BUFFER_LENGTH, true)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public Mp3File(string filename, int bufferLength) : this(filename, bufferLength, true)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public Mp3File(string filename, bool scanFile) : this(filename, DEFAULT_BUFFER_LENGTH, scanFile)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public Mp3File(string filename, int bufferLength, bool scanFile)
		{
            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("File name can not be null.");
            }
            this.filename = filename;

			if (bufferLength < MINIMUM_BUFFER_LENGTH + 1)
			{
				throw new ArgumentException("Buffer too small");
			}
			this.bufferLength = bufferLength;
			this.scanFile = scanFile;
			InitFile();
		}

        public Mp3File(Stream stream) : this(stream, DEFAULT_BUFFER_LENGTH, true)
        {
        }

        public Mp3File(Stream stream, int bufferLength) : this(stream, bufferLength, true)
        {
        }

        public Mp3File(Stream stream, bool scanFile) : this(stream, DEFAULT_BUFFER_LENGTH, scanFile)
        {
        }

        public Mp3File(Stream stream, int bufferLength, bool scanFile)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("Stream can not be null.");
            }
            this.stream = stream;

            if (bufferLength < MINIMUM_BUFFER_LENGTH + 1)
            {
                throw new ArgumentException("Buffer too small");
            }
            this.bufferLength = bufferLength;
            this.scanFile = scanFile;
            this.length = stream.Length;
            InitStream();
        }

        public virtual string GetFilename()
        {
            return filename;
        }

        public virtual long GetLength()
        {
            return length;
        }

        public virtual DateTime GetLastModified()
        {
            return lastModified;
        }
		
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void InitFile()
		{
            FileInfo file = new FileInfo(filename);
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found " + filename);
            }
            length = file.Length;
            lastModified = file.LastWriteTime;

            /*
            if (!file.CanRead())
            {
                throw new IOException("File not readable");
            }
            */

			stream = new FileStream (filename, System.IO.FileMode.Open, FileAccess.Read);

            InitStream();
		}

        private void InitStream()
        {
            try
            {
                InitId3v1Tag(stream);
                ScanFile(stream);
                if (startOffset < 0)
                {
                    throw new InvalidDataException("No mpegs frames found");
                }
                InitId3v2Tag(stream);
                if (scanFile)
                {
                    InitCustomTag(stream);
                }
            }
            finally
            {
                stream.Close();
            }
        }

		internal virtual int PreScanFile(Stream stream)
		{
			byte[] bytes = new byte[AbstractID3v2Tag.HEADER_LENGTH];
			try
			{
				stream.Position = 0;
				int bytesRead = stream.Read(bytes, 0, AbstractID3v2Tag.HEADER_LENGTH);
				if (bytesRead == AbstractID3v2Tag.HEADER_LENGTH)
				{
					try
					{
						ID3v2TagFactory.SanityCheckTag(bytes);
						return AbstractID3v2Tag.HEADER_LENGTH + BufferTools.UnpackSynchsafeInteger(bytes[
							AbstractID3v2Tag.DATA_LENGTH_OFFSET], bytes[AbstractID3v2Tag.DATA_LENGTH_OFFSET 
							+ 1], bytes[AbstractID3v2Tag.DATA_LENGTH_OFFSET + 2], bytes[AbstractID3v2Tag.DATA_LENGTH_OFFSET
							 + 3]);
					}
					catch (NoSuchTagException)
					{
					}
					catch (UnsupportedTagException)
					{
					}
				}
			}
			catch (IOException)
			{
			}
			// do nothing
			// do nothing
			// do nothing
			return 0;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void ScanFile(Stream stream)
		{
			byte[] bytes = new byte[bufferLength];
			int fileOffset = PreScanFile(stream);
			stream.Position = fileOffset;
			bool lastBlock = false;
			int lastOffset = fileOffset;
			while (!lastBlock)
			{
				int bytesRead = stream.Read(bytes, 0, bufferLength);
				if (bytesRead < bufferLength)
				{
					lastBlock = true;
				}
				if (bytesRead >= MINIMUM_BUFFER_LENGTH)
				{
					while (true)
					{
						try
						{
							int offset = 0;
							if (startOffset < 0)
							{
								offset = ScanBlockForStart(bytes, bytesRead, fileOffset, offset);
								if (startOffset >= 0 && !scanFile)
								{
									return;
								}
								lastOffset = startOffset;
							}
							offset = ScanBlock(bytes, bytesRead, fileOffset, offset);
							fileOffset += offset;
							stream.Position = fileOffset;
							break;
						}
						catch (InvalidDataException e)
						{
							if (frameCount < 2)
							{
								startOffset = -1;
								xingOffset = -1;
								frameCount = 0;
								bitrates.Clear();
								lastBlock = false;
								fileOffset = lastOffset + 1;
								if (fileOffset == 0)
								{
									throw new InvalidDataException("Valid start of mpeg frames not found", e);
								}
								stream.Position = fileOffset;
								break;
							}
							return;
						}
					}
				}
			}
		}

		private int ScanBlockForStart(byte[] bytes, int bytesRead, int absoluteOffset, int offset)
		{
			while (offset < bytesRead - MINIMUM_BUFFER_LENGTH)
			{
				if (bytes[offset] == unchecked((byte)unchecked((int)(0xFF))) &&
                    (bytes[offset + 1] & unchecked((byte)unchecked((int)(0xE0)))) == unchecked((byte)unchecked((int)(0xE0))))
				{
					try
					{
						MpegFrame frame = new MpegFrame(bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3]);
						if (xingOffset < 0 && IsXingFrame(bytes, offset))
						{
							xingOffset = absoluteOffset + offset;
							xingBitrate = frame.GetBitrate();
							offset += frame.GetLengthInBytes();
						}
						else
						{
							startOffset = absoluteOffset + offset;
							channelMode = frame.GetChannelMode();
							emphasis = frame.GetEmphasis();
							layer = frame.GetLayer();
							modeExtension = frame.GetModeExtension();
							sampleRate = frame.GetSampleRate();
							version = frame.GetVersion();
							copyright = frame.IsCopyright();
							original = frame.IsOriginal();
							frameCount++;
							AddBitrate(frame.GetBitrate());
							offset += frame.GetLengthInBytes();
							return offset;
						}
					}
					catch (InvalidDataException)
					{
						offset++;
					}
				}
				else
				{
					offset++;
				}
			}
			return offset;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private int ScanBlock(byte[] bytes, int bytesRead, int absoluteOffset, int offset)
		{
			while (offset < bytesRead - MINIMUM_BUFFER_LENGTH)
			{
				MpegFrame frame = new MpegFrame(bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3]);
				SanityCheckFrame(frame, absoluteOffset + offset);
				int newEndOffset = absoluteOffset + offset + frame.GetLengthInBytes() - 1;
				if (newEndOffset < GetMaxEndOffset())
				{
					endOffset = absoluteOffset + offset + frame.GetLengthInBytes() - 1;
					frameCount++;
					AddBitrate(frame.GetBitrate());
					offset += frame.GetLengthInBytes();
				}
				else
				{
					break;
				}
			}
			return offset;
		}

        public int GetMaxEndOffset()
		{
			int maxEndOffset = (int)GetLength();
			if (HasId3v1Tag())
			{
				maxEndOffset -= ID3v1Tag.TAG_LENGTH;
			}
			return maxEndOffset;
		}

		private bool IsXingFrame(byte[] bytes, int offset)
		{
			if (bytes.Length >= offset + XING_MARKER_OFFSET_1 + 3)
			{
				if ("Xing".Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues(bytes, offset
					 + XING_MARKER_OFFSET_1, 4)))
				{
					return true;
				}
				if ("Info".Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues(bytes, offset
					 + XING_MARKER_OFFSET_1, 4)))
				{
					return true;
				}
				if (bytes.Length >= offset + XING_MARKER_OFFSET_2 + 3)
				{
					if ("Xing".Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues(bytes, offset
						 + XING_MARKER_OFFSET_2, 4)))
					{
						return true;
					}
					if ("Info".Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues(bytes, offset
						 + XING_MARKER_OFFSET_2, 4)))
					{
						return true;
					}
					if (bytes.Length >= offset + XING_MARKER_OFFSET_3 + 3)
					{
						if ("Xing".Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues(bytes, offset
							 + XING_MARKER_OFFSET_3, 4)))
						{
							return true;
						}
						if ("Info".Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues(bytes, offset
							 + XING_MARKER_OFFSET_3, 4)))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SanityCheckFrame(MpegFrame frame, int offset)
		{
			if (sampleRate != frame.GetSampleRate())
			{
				throw new InvalidDataException("Inconsistent frame header");
			}
			if (!layer.Equals(frame.GetLayer()))
			{
				throw new InvalidDataException("Inconsistent frame header");
			}
			if (!version.Equals(frame.GetVersion()))
			{
				throw new InvalidDataException("Inconsistent frame header");
			}
			if (offset + frame.GetLengthInBytes() > GetLength())
			{
				throw new InvalidDataException("Frame would extend beyond end of file");
			}
		}

		private void AddBitrate(int bitrate)
		{
			int key = bitrate;
            MutableInteger count = null;
            if (bitrates.TryGetValue(key, out count) && count != null)
			{
				count.Increment();
			}
			else
			{
				bitrates[key] = new MutableInteger(1);
			}
			this.bitrate = ((this.bitrate * (frameCount - 1)) + bitrate) / frameCount;
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void InitId3v1Tag(Stream stream)
		{
			byte[] bytes = new byte[ID3v1Tag.TAG_LENGTH];
			stream.Position = GetLength() - ID3v1Tag.TAG_LENGTH;
			int bytesRead = stream.Read(bytes, 0, ID3v1Tag.TAG_LENGTH);
			if (bytesRead < ID3v1Tag.TAG_LENGTH)
			{
				throw new IOException("Not enough bytes read");
			}
			try
			{
				id3v1Tag = new ID3v1Tag(bytes);
			}
			catch (NoSuchTagException)
			{
				id3v1Tag = null;
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void InitId3v2Tag(Stream stream)
		{
			if (xingOffset == 0 || startOffset == 0)
			{
				id3v2Tag = null;
			}
			else
			{
				int bufferLength;
				if (HasXingFrame())
				{
					bufferLength = xingOffset;
				}
				else
				{
					bufferLength = startOffset;
				}
				byte[] bytes = new byte[bufferLength];
				stream.Position = 0;
				int bytesRead = stream.Read(bytes, 0, bufferLength);
				if (bytesRead < bufferLength)
				{
					throw new IOException("Not enough bytes read");
				}
				try
				{
					id3v2Tag = ID3v2TagFactory.CreateTag(bytes);
				}
				catch (NoSuchTagException)
				{
					id3v2Tag = null;
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void InitCustomTag(Stream stream)
		{
			int bufferLength = (int)(GetLength() - (endOffset + 1));
			if (HasId3v1Tag())
			{
				bufferLength -= ID3v1Tag.TAG_LENGTH;
			}
			if (bufferLength <= 0)
			{
				customTag = null;
			}
			else
			{
				customTag = new byte[bufferLength];
				stream.Position = endOffset + 1;
				int bytesRead = stream.Read(customTag, 0, bufferLength);
				if (bytesRead < bufferLength)
				{
					throw new IOException("Not enough bytes read");
				}
			}
		}

		public virtual int GetFrameCount()
		{
			return frameCount;
		}

		public virtual int GetStartOffset()
		{
			return startOffset;
		}

		public virtual int GetEndOffset()
		{
			return endOffset;
		}

		public virtual long GetLengthInMilliseconds()
		{
			double d = 8 * (endOffset - startOffset);
			return (long)((d / bitrate) + 0.5);
		}

		public virtual long GetLengthInSeconds()
		{
			return ((GetLengthInMilliseconds() + 500) / 1000);
		}

		public virtual bool IsVbr()
		{
			return bitrates.Count > 1;
		}

		public virtual int GetBitrate()
		{
			return (int)(bitrate + 0.5);
		}

		public virtual IDictionary<int, MutableInteger> GetBitrates()
		{
			return bitrates;
		}

		public virtual string GetChannelMode()
		{
			return channelMode;
		}

		public virtual bool IsCopyright()
		{
			return copyright;
		}

		public virtual string GetEmphasis()
		{
			return emphasis;
		}

		public virtual string GetLayer()
		{
			return layer;
		}

		public virtual string GetModeExtension()
		{
			return modeExtension;
		}

		public virtual bool IsOriginal()
		{
			return original;
		}

		public virtual int GetSampleRate()
		{
			return sampleRate;
		}

		public virtual string GetVersion()
		{
			return version;
		}

		public virtual bool HasXingFrame()
		{
			return (xingOffset >= 0);
		}

		public virtual int GetXingOffset()
		{
			return xingOffset;
		}

		public virtual int GetXingBitrate()
		{
			return xingBitrate;
		}

		public virtual bool HasId3v1Tag()
		{
			return id3v1Tag != null;
		}

		public virtual ID3v1 GetId3v1Tag()
		{
			return id3v1Tag;
		}

		public virtual void SetId3v1Tag(ID3v1 id3v1Tag)
		{
			this.id3v1Tag = id3v1Tag;
		}

		public virtual void RemoveId3v1Tag()
		{
			this.id3v1Tag = null;
		}

		public virtual bool HasId3v2Tag()
		{
			return id3v2Tag != null;
		}

		public virtual ID3v2 GetId3v2Tag()
		{
			return id3v2Tag;
		}

		public virtual void SetId3v2Tag(ID3v2 id3v2Tag)
		{
			this.id3v2Tag = id3v2Tag;
		}

		public virtual void RemoveId3v2Tag()
		{
			this.id3v2Tag = null;
		}

		public virtual bool HasCustomTag()
		{
			return customTag != null;
		}

		public virtual byte[] GetCustomTag()
		{
			return customTag;
		}

		public virtual void SetCustomTag(byte[] customTag)
		{
			this.customTag = customTag;
		}

		public virtual void RemoveCustomTag()
		{
			this.customTag = null;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Mp3net.NotSupportedException"></exception>
		public virtual void Save(string newFilename)
		{
		    if (String.Equals(filename, newFilename, StringComparison.OrdinalIgnoreCase))
		    {
                throw new ArgumentException("Save filename same as source filename");
		    }		    
			Stream saveStream = new FileStream (newFilename, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite);
			try
			{
				if (HasId3v2Tag())
				{
					byte[] buffer = id3v2Tag.ToBytes();
					saveStream.Write(buffer, 0, buffer.Length);
				}
				SaveMpegFrames(saveStream);
				if (HasCustomTag())
				{
					saveStream.Write(customTag, 0, customTag.Length);
				}
				if (HasId3v1Tag())
				{
					byte[] buffer = id3v1Tag.ToBytes();
					saveStream.Write(buffer, 0, buffer.Length);
				}
			}
			finally
			{
				saveStream.Close();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void SaveMpegFrames(Stream saveStream)
		{
			int filePos = xingOffset;
			if (filePos < 0)
			{
				filePos = startOffset;
			}
			if (filePos < 0)
			{
				return;
			}
			if (endOffset < filePos)
			{
				return;
			}
			Stream stream = new FileStream (filename, System.IO.FileMode.Open, FileAccess.Read);
			byte[] bytes = new byte[bufferLength];
			try
			{
				stream.Position = filePos;
				while (true)
				{
					int bytesRead = stream.Read(bytes, 0, bufferLength);
					if (filePos + bytesRead <= endOffset)
					{
						saveStream.Write(bytes, 0, bytesRead);
						filePos += bytesRead;
					}
					else
					{
						saveStream.Write(bytes, 0, endOffset - filePos + 1);
						break;
					}
				}
			}
			finally
			{
				stream.Close();
			}
		}
	}
}
