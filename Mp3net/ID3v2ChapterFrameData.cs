using System.Collections.Generic;
using System.Text;
using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v2ChapterFrameData : AbstractID3v2FrameData
	{
		protected internal string id;

		protected internal int startTime;

		protected internal int endTime;

		protected internal int startOffset;

		protected internal int endOffset;

		protected internal IList<ID3v2Frame> subframes = new List<ID3v2Frame>();

		public ID3v2ChapterFrameData(bool unsynchronisation) : base(unsynchronisation)
		{
		}

		public ID3v2ChapterFrameData(bool unsynchronisation, string id, int startTime, int
			 endTime, int startOffset, int endOffset) : base(unsynchronisation)
		{
			this.id = id;
			this.startTime = startTime;
			this.endTime = endTime;
			this.startOffset = startOffset;
			this.endOffset = endOffset;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2ChapterFrameData(bool unsynchronisation, byte[] bytes) : base(unsynchronisation
			)
		{
			SynchroniseAndUnpackFrameData(bytes);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal override void UnpackFrameData(byte[] bytes)
		{
			ByteBuffer bb = ByteBuffer.Wrap(bytes);
			id = ByteBufferUtils.ExtractNullTerminatedString(bb);
			bb.Position(id.Length + 1);
			startTime = bb.GetInt();
			endTime = bb.GetInt();
			startOffset = bb.GetInt();
			endOffset = bb.GetInt();
			for (int offset = bb.Position(); offset < bytes.Length; )
			{
				ID3v2Frame frame = new ID3v2Frame(bytes, offset);
				offset += frame.GetLength();
				subframes.AddItem(frame);
			}
		}

		public virtual void AddSubframe(string id, AbstractID3v2FrameData frame)
		{
			subframes.AddItem(new ID3v2Frame(id, frame.ToBytes()));
		}

		protected internal override byte[] PackFrameData()
		{
			ByteBuffer bb = ByteBuffer.Allocate(GetLength());
			bb.Put(Runtime.GetBytesForString(id));
			bb.Put(unchecked((byte)0));
			bb.PutInt(startTime);
			bb.PutInt(endTime);
			bb.PutInt(startOffset);
			bb.PutInt(endOffset);
			foreach (ID3v2Frame frame in subframes)
			{
				try
				{
					bb.Put(frame.ToBytes());
				}
				catch (NotSupportedException e)
				{
					Runtime.PrintStackTrace(e);
				}
			}
			return ((byte[])bb.Array());
		}

		public virtual string GetId()
		{
			return id;
		}

		public virtual void SetId(string id)
		{
			this.id = id;
		}

		public virtual int GetStartTime()
		{
			return startTime;
		}

		public virtual void SetStartTime(int startTime)
		{
			this.startTime = startTime;
		}

		public virtual int GetEndTime()
		{
			return endTime;
		}

		public virtual void SetEndTime(int endTime)
		{
			this.endTime = endTime;
		}

		public virtual int GetStartOffset()
		{
			return startOffset;
		}

		public virtual void SetStartOffset(int startOffset)
		{
			this.startOffset = startOffset;
		}

		public virtual int GetEndOffset()
		{
			return endOffset;
		}

		public virtual void SetEndOffset(int endOffset)
		{
			this.endOffset = endOffset;
		}

		public virtual IList<ID3v2Frame> GetSubframes()
		{
			return subframes;
		}

		public virtual void SetSubframes(IList<ID3v2Frame> subframes)
		{
			this.subframes = subframes;
		}

		protected internal override int GetLength()
		{
			int length = 1;
			length += 16;
			if (id != null)
			{
				length += id.Length;
			}
			if (subframes != null)
			{
				foreach (ID3v2Frame frame in subframes)
				{
					length += frame.GetLength();
				}
			}
			return length;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("ID3v2ChapterFrameData [id=");
			builder.Append(id);
			builder.Append(", startTime=");
			builder.Append(startTime);
			builder.Append(", endTime=");
			builder.Append(endTime);
			builder.Append(", startOffset=");
			builder.Append(startOffset);
			builder.Append(", endOffset=");
			builder.Append(endOffset);
			builder.Append(", subframes=");
			builder.Append(subframes);
			builder.Append("]");
			return builder.ToString();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!base.Equals(obj))
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			Mp3net.ID3v2ChapterFrameData other = (Mp3net.ID3v2ChapterFrameData)obj;
			if (endOffset != other.endOffset)
			{
				return false;
			}
			if (endTime != other.endTime)
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
				if (!id.Equals(other.id))
				{
					return false;
				}
			}
			if (startOffset != other.startOffset)
			{
				return false;
			}
			if (startTime != other.startTime)
			{
				return false;
			}
			if (subframes == null)
			{
				if (other.subframes != null)
				{
					return false;
				}
			}
			else
			{
                return (Arrays.Equals(Collections.ToArray(subframes), Collections.ToArray(other.subframes)));
                /*
				if (!subframes.Equals(other.subframes))
				{
					return false;
				}
                 */
			}
			return true;
		}
	}
}
