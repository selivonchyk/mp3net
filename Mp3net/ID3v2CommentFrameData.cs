using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v2CommentFrameData : AbstractID3v2FrameData
	{
		private static readonly string DEFAULT_LANGUAGE = "eng";

		private string language;

		private EncodedText description;

		private EncodedText comment;

		public ID3v2CommentFrameData(bool unsynchronisation) : base(unsynchronisation)
		{
		}

		public ID3v2CommentFrameData(bool unsynchronisation, string language, EncodedText
			 description, EncodedText comment) : base(unsynchronisation)
		{
			this.language = language;
			this.description = description;
			this.comment = comment;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2CommentFrameData(bool unsynchronisation, byte[] bytes) : base(unsynchronisation
			)
		{
			SynchroniseAndUnpackFrameData(bytes);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal override void UnpackFrameData(byte[] bytes)
		{
			try
			{
				language = BufferTools.ByteBufferToString(bytes, 1, 3);
			}
			catch (UnsupportedEncodingException)
			{
				language = string.Empty;
			}
			int marker = BufferTools.IndexOfTerminatorForEncoding(bytes, 4, bytes[0]);
			if (marker >= 4)
			{
				description = new EncodedText(bytes[0], BufferTools.CopyBuffer(bytes, 4, marker -
					 4));
				marker += description.GetTerminator().Length;
			}
			else
			{
				description = new EncodedText(bytes[0], string.Empty);
				marker = 4;
			}
			comment = new EncodedText(bytes[0], BufferTools.CopyBuffer(bytes, marker, bytes.Length
				 - marker));
		}

		protected internal override byte[] PackFrameData()
		{
			byte[] bytes = new byte[GetLength()];
			if (comment != null)
			{
				bytes[0] = comment.GetTextEncoding();
			}
			else
			{
				bytes[0] = 0;
			}
			string langPadded;
			if (language == null)
			{
				langPadded = DEFAULT_LANGUAGE;
			}
			else
			{
				if (language.Length > 3)
				{
					langPadded = language.Substring(0, 3);
				}
				else
				{
					langPadded = BufferTools.PadStringRight(language, 3, '\0');
				}
			}
			try
			{
				BufferTools.StringIntoByteBuffer(langPadded, 0, 3, bytes, 1);
			}
			catch (UnsupportedEncodingException)
			{
			}
			int marker = 4;
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
			if (comment != null)
			{
				byte[] commentBytes = comment.ToBytes(true, false);
				BufferTools.CopyIntoByteBuffer(commentBytes, 0, commentBytes.Length, bytes, marker
					);
			}
			return bytes;
		}

		protected internal override int GetLength()
		{
			int length = 4;
			if (description != null)
			{
				length += description.ToBytes(true, true).Length;
			}
			else
			{
				length++;
			}
			if (comment != null)
			{
				length += comment.ToBytes(true, false).Length;
			}
			return length;
		}

		public virtual string GetLanguage()
		{
			return language;
		}

		public virtual void SetLanguage(string language)
		{
			this.language = language;
		}

		public virtual EncodedText GetComment()
		{
			return comment;
		}

		public virtual void SetComment(EncodedText comment)
		{
			this.comment = comment;
		}

		public virtual EncodedText GetDescription()
		{
			return description;
		}

		public virtual void SetDescription(EncodedText description)
		{
			this.description = description;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.ID3v2CommentFrameData))
			{
				return false;
			}
			if (!base.Equals(obj))
			{
				return false;
			}
			Mp3net.ID3v2CommentFrameData other = (Mp3net.ID3v2CommentFrameData)obj;
			if (language == null)
			{
				if (other.language != null)
				{
					return false;
				}
			}
			else
			{
				if (other.language == null)
				{
					return false;
				}
				else
				{
					if (!language.Equals(other.language))
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
			if (comment == null)
			{
				if (other.comment != null)
				{
					return false;
				}
			}
			else
			{
				if (other.comment == null)
				{
					return false;
				}
				else
				{
					if (!comment.Equals(other.comment))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
