using System;
using System.Text;
using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v1Tag : ID3v1
	{
		public const int TAG_LENGTH = 128;

		private static readonly string VERSION_0 = "0";

		private static readonly string VERSION_1 = "1";

		private static readonly string TAG = "TAG";

		private const int TITLE_OFFSET = 3;

		private const int TITLE_LENGTH = 30;

		private const int ARTIST_OFFSET = 33;

		private const int ARTIST_LENGTH = 30;

		private const int ALBUM_OFFSET = 63;

		private const int ALBUM_LENGTH = 30;

		private const int YEAR_OFFSET = 93;

		private const int YEAR_LENGTH = 4;

		private const int COMMENT_OFFSET = 97;

		private const int COMMENT_LENGTH_V1_0 = 30;

		private const int COMMENT_LENGTH_V1_1 = 28;

		private const int TRACK_MARKER_OFFSET = 125;

		private const int TRACK_OFFSET = 126;

		private const int GENRE_OFFSET = 127;

		private string track = null;

		private string artist = null;

		private string title = null;

		private string album = null;

		private string year = null;

		private int genre = -1;

		private string comment = null;

		public ID3v1Tag()
		{
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		public ID3v1Tag(byte[] bytes)
		{
			UnpackTag(bytes);
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		private void UnpackTag(byte[] bytes)
		{
			SanityCheckTag(bytes);
			title = BufferTools.TrimStringRight(BufferTools.ByteBufferToStringIgnoringEncodingIssues
				(bytes, TITLE_OFFSET, TITLE_LENGTH));
			artist = BufferTools.TrimStringRight(BufferTools.ByteBufferToStringIgnoringEncodingIssues
				(bytes, ARTIST_OFFSET, ARTIST_LENGTH));
			album = BufferTools.TrimStringRight(BufferTools.ByteBufferToStringIgnoringEncodingIssues
				(bytes, ALBUM_OFFSET, ALBUM_LENGTH));
			year = BufferTools.TrimStringRight(BufferTools.ByteBufferToStringIgnoringEncodingIssues
				(bytes, YEAR_OFFSET, YEAR_LENGTH));
			genre = bytes[GENRE_OFFSET] & unchecked((int)(0xFF));
			if (genre == unchecked((int)(0xFF)))
			{
				genre = -1;
			}
			if (bytes[TRACK_MARKER_OFFSET] != 0)
			{
				comment = BufferTools.TrimStringRight(BufferTools.ByteBufferToStringIgnoringEncodingIssues
					(bytes, COMMENT_OFFSET, COMMENT_LENGTH_V1_0));
				track = null;
			}
			else
			{
				comment = BufferTools.TrimStringRight(BufferTools.ByteBufferToStringIgnoringEncodingIssues
					(bytes, COMMENT_OFFSET, COMMENT_LENGTH_V1_1));
				int trackInt = bytes[TRACK_OFFSET];
				if (trackInt == 0)
				{
					track = string.Empty;
				}
				else
				{
					track = Mp3net.Helpers.Extensions.ToString(trackInt);
				}
			}
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		private void SanityCheckTag(byte[] bytes)
		{
			if (bytes.Length != TAG_LENGTH)
			{
				throw new NoSuchTagException("Buffer length wrong");
			}
			if (!TAG.Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues(bytes, 0, TAG
				.Length)))
			{
				throw new NoSuchTagException();
			}
		}

		public virtual byte[] ToBytes()
		{
			byte[] bytes = new byte[TAG_LENGTH];
			PackTag(bytes);
			return bytes;
		}

		public virtual void ToBytes(byte[] bytes)
		{
			PackTag(bytes);
		}

		public virtual void PackTag(byte[] bytes)
		{
			Arrays.Fill(bytes, unchecked((byte)0));
			try
			{
				BufferTools.StringIntoByteBuffer(TAG, 0, 3, bytes, 0);
			}
			catch (UnsupportedEncodingException)
			{
			}
			PackField(bytes, title, TITLE_LENGTH, TITLE_OFFSET);
			PackField(bytes, artist, ARTIST_LENGTH, ARTIST_OFFSET);
			PackField(bytes, album, ALBUM_LENGTH, ALBUM_OFFSET);
			PackField(bytes, year, YEAR_LENGTH, YEAR_OFFSET);
			if (genre < 128)
			{
				bytes[GENRE_OFFSET] = unchecked((byte)genre);
			}
			else
			{
				bytes[GENRE_OFFSET] = unchecked((byte)(genre - 256));
			}
			if (track == null)
			{
				PackField(bytes, comment, COMMENT_LENGTH_V1_0, COMMENT_OFFSET);
			}
			else
			{
				PackField(bytes, comment, COMMENT_LENGTH_V1_1, COMMENT_OFFSET);
				string trackTemp = NumericsOnly(track);
				if (trackTemp.Length > 0)
				{
					int trackInt = System.Convert.ToInt32(trackTemp.ToString());
					if (trackInt < 128)
					{
						bytes[TRACK_OFFSET] = unchecked((byte)trackInt);
					}
					else
					{
						bytes[TRACK_OFFSET] = unchecked((byte)(trackInt - 256));
					}
				}
			}
		}

		private void PackField(byte[] bytes, string value, int maxLength, int offset)
		{
			if (value != null)
			{
				try
				{
					BufferTools.StringIntoByteBuffer(value, 0, Math.Min(value.Length, maxLength), bytes
						, offset);
				}
				catch (UnsupportedEncodingException)
				{
				}
			}
		}

		private string NumericsOnly(string s)
		{
			StringBuilder stringBuffer = new StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				char ch = s[i];
				if (ch >= '0' && ch <= '9')
				{
					stringBuffer.Append(ch);
				}
				else
				{
					break;
				}
			}
			return stringBuffer.ToString();
		}

		public virtual string GetVersion()
		{
			if (track == null)
			{
				return VERSION_0;
			}
			else
			{
				return VERSION_1;
			}
		}

		public virtual string GetTrack()
		{
			return track;
		}

		public virtual void SetTrack(string track)
		{
			this.track = track;
		}

		public virtual string GetArtist()
		{
			return artist;
		}

		public virtual void SetArtist(string artist)
		{
			this.artist = artist;
		}

		public virtual string GetTitle()
		{
			return title;
		}

		public virtual void SetTitle(string title)
		{
			this.title = title;
		}

		public virtual string GetAlbum()
		{
			return album;
		}

		public virtual void SetAlbum(string album)
		{
			this.album = album;
		}

		public virtual string GetYear()
		{
			return year;
		}

		public virtual void SetYear(string year)
		{
			this.year = year;
		}

		public virtual int GetGenre()
		{
			return genre;
		}

		public virtual void SetGenre(int genre)
		{
			this.genre = genre;
		}

		public virtual string GetGenreDescription()
		{
			try
			{
				return ID3v1Genres.GENRES[genre];
			}
			catch (IndexOutOfRangeException)
			{
				return "Unknown";
			}
		}

		public virtual string GetComment()
		{
			return comment;
		}

		public virtual void SetComment(string comment)
		{
			this.comment = comment;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.ID3v1Tag))
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			Mp3net.ID3v1Tag other = (Mp3net.ID3v1Tag)obj;
			if (genre != other.genre)
			{
				return false;
			}
			if (track == null)
			{
				if (other.track != null)
				{
					return false;
				}
			}
			else
			{
				if (other.track == null)
				{
					return false;
				}
				else
				{
					if (!track.Equals(other.track))
					{
						return false;
					}
				}
			}
			if (artist == null)
			{
				if (other.artist != null)
				{
					return false;
				}
			}
			else
			{
				if (other.artist == null)
				{
					return false;
				}
				else
				{
					if (!artist.Equals(other.artist))
					{
						return false;
					}
				}
			}
			if (title == null)
			{
				if (other.title != null)
				{
					return false;
				}
			}
			else
			{
				if (other.title == null)
				{
					return false;
				}
				else
				{
					if (!title.Equals(other.title))
					{
						return false;
					}
				}
			}
			if (album == null)
			{
				if (other.album != null)
				{
					return false;
				}
			}
			else
			{
				if (other.album == null)
				{
					return false;
				}
				else
				{
					if (!album.Equals(other.album))
					{
						return false;
					}
				}
			}
			if (year == null)
			{
				if (other.year != null)
				{
					return false;
				}
			}
			else
			{
				if (other.year == null)
				{
					return false;
				}
				else
				{
					if (!year.Equals(other.year))
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
