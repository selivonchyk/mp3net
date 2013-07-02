using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v2PictureFrameData : AbstractID3v2FrameData
	{
		protected internal string mimeType;

		protected internal byte pictureType;

		protected internal EncodedText description;

		protected internal byte[] imageData;

		public ID3v2PictureFrameData(bool unsynchronisation) : base(unsynchronisation)
		{
		}

		public ID3v2PictureFrameData(bool unsynchronisation, string mimeType, byte pictureType
			, EncodedText description, byte[] imageData) : base(unsynchronisation)
		{
			this.mimeType = mimeType;
			this.pictureType = pictureType;
			this.description = description;
			this.imageData = imageData;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2PictureFrameData(bool unsynchronisation, byte[] bytes) : base(unsynchronisation
			)
		{
			SynchroniseAndUnpackFrameData(bytes);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal override void UnpackFrameData(byte[] bytes)
		{
			int marker = BufferTools.IndexOfTerminator(bytes, 1, 1);
			if (marker >= 0)
			{
				try
				{
					mimeType = BufferTools.ByteBufferToString(bytes, 1, marker - 1);
				}
				catch (UnsupportedEncodingException)
				{
					mimeType = "image/unknown";
				}
			}
			else
			{
				mimeType = "image/unknown";
			}
			pictureType = bytes[marker + 1];
			marker += 2;
			int marker2 = BufferTools.IndexOfTerminatorForEncoding(bytes, marker, bytes[0]);
			if (marker2 >= 0)
			{
				description = new EncodedText(bytes[0], BufferTools.CopyBuffer(bytes, marker, marker2
					 - marker));
				marker2 += description.GetTerminator().Length;
			}
			else
			{
				description = new EncodedText(bytes[0], string.Empty);
				marker2 = marker;
			}
			imageData = BufferTools.CopyBuffer(bytes, marker2, bytes.Length - marker2);
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
			int mimeTypeLength = 0;
			if (mimeType != null && mimeType.Length > 0)
			{
				mimeTypeLength = mimeType.Length;
				try
				{
					BufferTools.StringIntoByteBuffer(mimeType, 0, mimeTypeLength, bytes, 1);
				}
				catch (UnsupportedEncodingException)
				{
				}
			}
			int marker = mimeTypeLength + 1;
			bytes[marker++] = 0;
			bytes[marker++] = pictureType;
			if (description != null && description.ToBytes().Length > 0)
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
			if (imageData != null && imageData.Length > 0)
			{
				BufferTools.CopyIntoByteBuffer(imageData, 0, imageData.Length, bytes, marker);
			}
			return bytes;
		}

		protected internal override int GetLength()
		{
			int length = 3;
			if (mimeType != null)
			{
				length += mimeType.Length;
			}
			if (description != null)
			{
				length += description.ToBytes(true, true).Length;
			}
			else
			{
				length++;
			}
			if (imageData != null)
			{
				length += imageData.Length;
			}
			return length;
		}

		public virtual string GetMimeType()
		{
			return mimeType;
		}

		public virtual void SetMimeType(string mimeType)
		{
			this.mimeType = mimeType;
		}

		public virtual byte GetPictureType()
		{
			return pictureType;
		}

		public virtual void SetPictureType(byte pictureType)
		{
			this.pictureType = pictureType;
		}

		public virtual EncodedText GetDescription()
		{
			return description;
		}

		public virtual void SetDescription(EncodedText description)
		{
			this.description = description;
		}

		public virtual byte[] GetImageData()
		{
			return imageData;
		}

		public virtual void SetImageData(byte[] imageData)
		{
			this.imageData = imageData;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.ID3v2PictureFrameData))
			{
				return false;
			}
			if (!base.Equals(obj))
			{
				return false;
			}
			Mp3net.ID3v2PictureFrameData other = (Mp3net.ID3v2PictureFrameData)obj;
			if (pictureType != other.pictureType)
			{
				return false;
			}
			if (mimeType == null)
			{
				if (other.mimeType != null)
				{
					return false;
				}
			}
			else
			{
				if (other.mimeType == null)
				{
					return false;
				}
				else
				{
					if (!mimeType.Equals(other.mimeType))
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
			if (imageData == null)
			{
				if (other.imageData != null)
				{
					return false;
				}
			}
			else
			{
				if (other.imageData == null)
				{
					return false;
				}
				else
				{
					if (!Arrays.Equals(imageData, other.imageData))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
