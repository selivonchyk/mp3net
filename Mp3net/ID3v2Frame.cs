using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v2Frame
	{
		private const int HEADER_LENGTH = 10;

		private const int ID_OFFSET = 0;

		private const int ID_LENGTH = 4;

		protected internal const int DATA_LENGTH_OFFSET = 4;

		private const int FLAGS1_OFFSET = 8;

		private const int FLAGS2_OFFSET = 9;

		private const int PRESERVE_TAG_BIT = 6;

		private const int PRESERVE_FILE_BIT = 5;

		private const int READ_ONLY_BIT = 4;

		private const int GROUP_BIT = 6;

		private const int COMPRESSION_BIT = 3;

		private const int ENCRYPTION_BIT = 2;

		private const int UNSYNCHRONISATION_BIT = 1;

		private const int DATA_LENGTH_INDICATOR_BIT = 0;

		protected internal string id;

		protected internal int dataLength = 0;

		protected internal byte[] data = null;

		private bool preserveTag = false;

		private bool preserveFile = false;

		private bool readOnly = false;

		private bool group = false;

		private bool compression = false;

		private bool encryption = false;

		private bool unsynchronisation = false;

		private bool dataLengthIndicator = false;

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2Frame(byte[] buffer, int offset)
		{
			UnpackFrame(buffer, offset);
		}

		public ID3v2Frame(string id, byte[] data)
		{
			this.id = id;
			this.data = data;
			dataLength = data.Length;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal virtual void UnpackFrame(byte[] buffer, int offset)
		{
			int dataOffset = UnpackHeader(buffer, offset);
			SanityCheckUnpackedHeader();
			data = BufferTools.CopyBuffer(buffer, dataOffset, dataLength);
		}

		protected internal virtual int UnpackHeader(byte[] buffer, int offset)
		{
			id = BufferTools.ByteBufferToStringIgnoringEncodingIssues(buffer, offset + ID_OFFSET, ID_LENGTH);
			UnpackDataLength(buffer, offset);
			UnpackFlags(buffer, offset);
			return offset + HEADER_LENGTH;
		}

		protected internal virtual void UnpackDataLength(byte[] buffer, int offset)
		{
			dataLength = BufferTools.UnpackInteger(buffer[offset + DATA_LENGTH_OFFSET], buffer
				[offset + DATA_LENGTH_OFFSET + 1], buffer[offset + DATA_LENGTH_OFFSET + 2], buffer
				[offset + DATA_LENGTH_OFFSET + 3]);
		}

		private void UnpackFlags(byte[] buffer, int offset)
		{
			preserveTag = BufferTools.CheckBit(buffer[offset + FLAGS1_OFFSET], PRESERVE_TAG_BIT
				);
			preserveFile = BufferTools.CheckBit(buffer[offset + FLAGS1_OFFSET], PRESERVE_FILE_BIT
				);
			readOnly = BufferTools.CheckBit(buffer[offset + FLAGS1_OFFSET], READ_ONLY_BIT);
			group = BufferTools.CheckBit(buffer[offset + FLAGS2_OFFSET], GROUP_BIT);
			compression = BufferTools.CheckBit(buffer[offset + FLAGS2_OFFSET], COMPRESSION_BIT
				);
			encryption = BufferTools.CheckBit(buffer[offset + FLAGS2_OFFSET], ENCRYPTION_BIT);
			unsynchronisation = BufferTools.CheckBit(buffer[offset + FLAGS2_OFFSET], UNSYNCHRONISATION_BIT
				);
			dataLengthIndicator = BufferTools.CheckBit(buffer[offset + FLAGS2_OFFSET], DATA_LENGTH_INDICATOR_BIT
				);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal virtual void SanityCheckUnpackedHeader()
		{
			for (int i = 0; i < id.Length; i++)
			{
				if (!((id[i] >= 'A' && id[i] <= 'Z') || (id[i] >= '0' && id[i] <= '9')))
				{
					throw new InvalidDataException("Not a valid frame - invalid tag " + id);
				}
			}
		}

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		public virtual byte[] ToBytes()
		{
			byte[] bytes = new byte[GetLength()];
			PackFrame(bytes, 0);
			return bytes;
		}

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		public virtual void ToBytes(byte[] bytes, int offset)
		{
			PackFrame(bytes, offset);
		}

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		public virtual void PackFrame(byte[] bytes, int offset)
		{
			PackHeader(bytes, offset);
			BufferTools.CopyIntoByteBuffer(data, 0, data.Length, bytes, offset + HEADER_LENGTH
				);
		}

		private void PackHeader(byte[] bytes, int i)
		{
			try
			{
				BufferTools.StringIntoByteBuffer(id, 0, id.Length, bytes, 0);
			}
			catch (UnsupportedEncodingException)
			{
			}
			BufferTools.CopyIntoByteBuffer(PackDataLength(), 0, 4, bytes, 4);
			BufferTools.CopyIntoByteBuffer(PackFlags(), 0, 2, bytes, 8);
		}

		protected internal virtual byte[] PackDataLength()
		{
			return BufferTools.PackInteger(dataLength);
		}

		private byte[] PackFlags()
		{
			byte[] bytes = new byte[2];
			bytes[0] = BufferTools.SetBit(bytes[0], PRESERVE_TAG_BIT, preserveTag);
			bytes[0] = BufferTools.SetBit(bytes[0], PRESERVE_FILE_BIT, preserveFile);
			bytes[0] = BufferTools.SetBit(bytes[0], READ_ONLY_BIT, readOnly);
			bytes[1] = BufferTools.SetBit(bytes[1], GROUP_BIT, group);
			bytes[1] = BufferTools.SetBit(bytes[1], COMPRESSION_BIT, compression);
			bytes[1] = BufferTools.SetBit(bytes[1], ENCRYPTION_BIT, encryption);
			bytes[1] = BufferTools.SetBit(bytes[1], UNSYNCHRONISATION_BIT, unsynchronisation);
			bytes[1] = BufferTools.SetBit(bytes[1], DATA_LENGTH_INDICATOR_BIT, dataLengthIndicator
				);
			return bytes;
		}

		public virtual string GetId()
		{
			return id;
		}

		public virtual int GetDataLength()
		{
			return dataLength;
		}

		public virtual int GetLength()
		{
			return dataLength + HEADER_LENGTH;
		}

		public virtual byte[] GetData()
		{
			return data;
		}

		public virtual void SetData(byte[] data)
		{
			this.data = data;
			if (data == null)
			{
				dataLength = 0;
			}
			else
			{
				dataLength = data.Length;
			}
		}

		public virtual bool HasDataLengthIndicator()
		{
			return dataLengthIndicator;
		}

		public virtual bool HasCompression()
		{
			return compression;
		}

		public virtual bool HasEncryption()
		{
			return encryption;
		}

		public virtual bool HasGroup()
		{
			return group;
		}

		public virtual bool HasPreserveFile()
		{
			return preserveFile;
		}

		public virtual bool HasPreserveTag()
		{
			return preserveTag;
		}

		public virtual bool IsReadOnly()
		{
			return readOnly;
		}

		public virtual bool HasUnsynchronisation()
		{
			return unsynchronisation;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.ID3v2Frame))
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			Mp3net.ID3v2Frame other = (Mp3net.ID3v2Frame)obj;
			if (dataLength != other.dataLength)
			{
				return false;
			}
			if (preserveTag != other.preserveTag)
			{
				return false;
			}
			if (preserveFile != other.preserveFile)
			{
				return false;
			}
			if (readOnly != other.readOnly)
			{
				return false;
			}
			if (group != other.group)
			{
				return false;
			}
			if (compression != other.compression)
			{
				return false;
			}
			if (encryption != other.encryption)
			{
				return false;
			}
			if (unsynchronisation != other.encryption)
			{
				return false;
			}
			if (dataLengthIndicator != other.dataLengthIndicator)
			{
				return false;
			}
			if (id == null)
			{
				if (other.id != null)
				{
					return false;
				}
			}
			else
			{
				if (other.id == null)
				{
					return false;
				}
				else
				{
					if (!id.Equals(other.id))
					{
						return false;
					}
				}
			}
			if (data == null)
			{
				if (other.data != null)
				{
					return false;
				}
			}
			else
			{
				if (other.data == null)
				{
					return false;
				}
				else
				{
					if (!Arrays.Equals(data, other.data))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
