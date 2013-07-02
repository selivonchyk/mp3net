using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v2UrlFrameData : AbstractID3v2FrameData
	{
		protected internal string url;

		protected internal EncodedText description;

		public ID3v2UrlFrameData(bool unsynchronisation) : base(unsynchronisation)
		{
		}

		public ID3v2UrlFrameData(bool unsynchronisation, EncodedText description, string 
			url) : base(unsynchronisation)
		{
			this.description = description;
			this.url = url;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2UrlFrameData(bool unsynchronisation, byte[] bytes) : base(unsynchronisation
			)
		{
			SynchroniseAndUnpackFrameData(bytes);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal override void UnpackFrameData(byte[] bytes)
		{
			int marker = BufferTools.IndexOfTerminatorForEncoding(bytes, 1, bytes[0]);
			if (marker >= 0)
			{
				description = new EncodedText(bytes[0], BufferTools.CopyBuffer(bytes, 1, marker -
					 1));
				marker += description.GetTerminator().Length;
			}
			else
			{
				description = new EncodedText(bytes[0], string.Empty);
				marker = 1;
			}
			try
			{
				url = BufferTools.ByteBufferToString(bytes, marker, bytes.Length - marker);
			}
			catch (UnsupportedEncodingException)
			{
				url = string.Empty;
			}
		}

		protected internal override byte[] PackFrameData()
		{
			byte[] bytes = new byte[GetLength()];
			if (description != null)
			{
				bytes[0] = description.GetTextEncoding();
			}
			else
			{
				bytes[0] = 0;
			}
			int marker = 1;
			if (description != null)
			{
				byte[] descriptionBytes = description.ToBytes(true, true);
				BufferTools.CopyIntoByteBuffer(descriptionBytes, 0, descriptionBytes.Length, bytes
					, marker);
				marker += descriptionBytes.Length;
			}
			else
			{
				bytes[marker++] = 0;
			}
			if (url != null && url.Length > 0)
			{
				try
				{
					BufferTools.StringIntoByteBuffer(url, 0, url.Length, bytes, marker);
				}
				catch (UnsupportedEncodingException)
				{
				}
			}
			return bytes;
		}

		protected internal override int GetLength()
		{
			int length = 1;
			if (description != null)
			{
				length += description.ToBytes(true, true).Length;
			}
			else
			{
				length++;
			}
			if (url != null)
			{
				length += url.Length;
			}
			return length;
		}

		public virtual EncodedText GetDescription()
		{
			return description;
		}

		public virtual void SetDescription(EncodedText description)
		{
			this.description = description;
		}

		public virtual string GetUrl()
		{
			return url;
		}

		public virtual void SetUrl(string url)
		{
			this.url = url;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.ID3v2UrlFrameData))
			{
				return false;
			}
			if (!base.Equals(obj))
			{
				return false;
			}
			Mp3net.ID3v2UrlFrameData other = (Mp3net.ID3v2UrlFrameData)obj;
			if (url == null)
			{
				if (other.url != null)
				{
					return false;
				}
			}
			else
			{
				if (other.url == null)
				{
					return false;
				}
				else
				{
					if (!url.Equals(other.url))
					{
						return false;
					}
				}
			}
			if (description == null)
			{
				if (other.description != null)
				{
					return false;
				}
			}
			else
			{
				if (other.description == null)
				{
					return false;
				}
				else
				{
					if (!description.Equals(other.description))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
