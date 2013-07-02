using System.Collections.Generic;
using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v2FrameSet
	{
		private string id;

		//private AList<ID3v2Frame> frames;
        private IList<ID3v2Frame> frames;

		public ID3v2FrameSet(string id)
		{
			this.id = id;
			frames = new List<ID3v2Frame>();
		}

		public virtual string GetId()
		{
			return id;
		}

		public virtual void Clear()
		{
			frames.Clear();
		}

		public virtual void AddFrame(ID3v2Frame frame)
		{
			frames.AddItem(frame);
		}

		public virtual IList<ID3v2Frame> GetFrames()
		{
			return frames;
		}

		public override string ToString()
		{
			return this.id + ": " + frames.Count;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.ID3v2FrameSet))
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			Mp3net.ID3v2FrameSet other = (Mp3net.ID3v2FrameSet)obj;
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
			if (frames == null)
			{
				if (other.frames != null)
				{
					return false;
				}
			}
			else
			{
				if (other.frames == null)
				{
					return false;
				}
				else
				{
                    if (frames == other.frames)
                    {
                        return true;
                    }
                    if ((frames == null) || (other.frames == null))
                    {
                        return false;
                    }
				    return (Arrays.Equals(Collections.ToArray(frames), Collections.ToArray(other.frames)));
				}
			}
			return true;
		}
	}
}
