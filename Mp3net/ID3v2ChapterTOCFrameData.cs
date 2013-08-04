using System;
using System.Collections.Generic;
using System.Text;
using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v2ChapterTOCFrameData : AbstractID3v2FrameData
	{
		protected internal bool isRoot;

		protected internal bool isOrdered;

		protected internal string id;

		protected internal string[] childs;

		protected internal IList<ID3v2Frame> subframes = new List<ID3v2Frame>();

		public ID3v2ChapterTOCFrameData(bool unsynchronisation) : base(unsynchronisation)
		{
		}

		public ID3v2ChapterTOCFrameData(bool unsynchronisation, bool isRoot, bool isOrdered
			, string id, string[] childs) : base(unsynchronisation)
		{
			this.isRoot = isRoot;
			this.isOrdered = isOrdered;
			this.id = id;
			this.childs = childs;
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2ChapterTOCFrameData(bool unsynchronisation, byte[] bytes) : base(unsynchronisation
			)
		{
			SynchroniseAndUnpackFrameData(bytes);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal override void UnpackFrameData(byte[] bytes)
		{
			ByteBuffer bb = ByteBuffer.Wrap(bytes);
			id = ByteBufferUtils.ExtractNullTerminatedString(bb);
			byte flags = bb.Get();
			if ((flags & unchecked((int)(0x01))) == unchecked((int)(0x01)))
			{
				isRoot = true;
			}
			if ((flags & unchecked((int)(0x02))) == unchecked((int)(0x02)))
			{
				isOrdered = true;
			}
			int childCount = bb.Get();
			// TODO: 0xFF -> int = 255; byte = -128;
			childs = new string[childCount];
			for (int i = 0; i < childCount; i++)
			{
				childs[i] = ByteBufferUtils.ExtractNullTerminatedString(bb);
			}
			for (int offset = bb.Position(); offset < bytes.Length; )
			{
				ID3v2Frame frame = new ID3v2Frame(bytes, offset);
				offset += frame.GetLength();
				subframes.Add(frame);
			}
		}

		public virtual void AddSubframe(string id, AbstractID3v2FrameData frame)
		{
			subframes.Add(new ID3v2Frame(id, frame.ToBytes()));
		}

		protected internal override byte[] PackFrameData()
		{
			ByteBuffer bb = ByteBuffer.Allocate(GetLength());
			bb.Put(Runtime.GetBytesForString(id));
			bb.Put(unchecked((byte)0));
			bb.Put(GetFlags());
			bb.Put(unchecked((byte)childs.Length));
			foreach (string child in childs)
			{
				bb.Put(Runtime.GetBytesForString(child));
				bb.Put(unchecked((byte)0));
			}
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

		private byte GetFlags()
		{
			byte b = 0;
			if (isRoot)
			{
				b |= unchecked((int)(0x01));
			}
			if (isOrdered)
			{
				b |= unchecked((int)(0x02));
			}
			return b;
		}

		public virtual bool IsRoot()
		{
			return isRoot;
		}

		public virtual void SetRoot(bool isRoot)
		{
			this.isRoot = isRoot;
		}

		public virtual bool IsOrdered()
		{
			return isOrdered;
		}

		public virtual void SetOrdered(bool isOrdered)
		{
			this.isOrdered = isOrdered;
		}

		public virtual string GetId()
		{
			return id;
		}

		public virtual void SetId(string id)
		{
			this.id = id;
		}

		public virtual string[] GetChilds()
		{
			return childs;
		}

		public virtual void SetChilds(string[] childs)
		{
			this.childs = childs;
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
			int length = 3;
			if (id != null)
			{
				length += id.Length;
			}
			if (childs != null)
			{
				length += childs.Length;
				foreach (string child in childs)
				{
					length += child.Length;
				}
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

		// public boolean equals(Object obj) {
		// if (! (obj instanceof ID3v2ChapterFrameData)) return false;
		// if (! super.equals(obj)) return false;
		// ID3v2ChapterFrameData other = (ID3v2ChapterFrameData) obj;
		// if (text == null) {
		// if (other.text != null) return false;
		// } else if (other.text == null) return false;
		// else if (! text.equals(other.text)) return false;
		// return true;
		// }
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("ID3v2ChapterTOCFrameData [isRoot=");
			builder.Append(isRoot);
			builder.Append(", isOrdered=");
			builder.Append(isOrdered);
			builder.Append(", id=");
			builder.Append(id);
			builder.Append(", childs=");
			builder.Append(String.Join(",",childs));
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
			Mp3net.ID3v2ChapterTOCFrameData other = (Mp3net.ID3v2ChapterTOCFrameData)obj;
			if (!Arrays.Equals(childs, other.childs))
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
			if (isOrdered != other.isOrdered)
			{
				return false;
			}
			if (isRoot != other.isRoot)
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
			}
			return true;
		}
	}
}
