namespace Mp3net
{
	public class ID3Wrapper
	{
		private ID3v1 id3v1Tag;

		private ID3v2 id3v2Tag;

		public ID3Wrapper(ID3v1 id3v1Tag, ID3v2 id3v2Tag)
		{
			this.id3v1Tag = id3v1Tag;
			this.id3v2Tag = id3v2Tag;
		}

		public virtual ID3v1 GetId3v1Tag()
		{
			return id3v1Tag;
		}

		public virtual ID3v2 GetId3v2Tag()
		{
			return id3v2Tag;
		}

		public virtual string GetTrack()
		{
			if (id3v2Tag != null && id3v2Tag.GetTrack() != null && id3v2Tag.GetTrack().Length
				 > 0)
			{
				return id3v2Tag.GetTrack();
			}
			else
			{
				if (id3v1Tag != null)
				{
					return id3v1Tag.GetTrack();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void SetTrack(string track)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetTrack(track);
			}
			if (id3v1Tag != null)
			{
				id3v1Tag.SetTrack(track);
			}
		}

		public virtual string GetArtist()
		{
			if (id3v2Tag != null && id3v2Tag.GetArtist() != null && id3v2Tag.GetArtist().Length
				 > 0)
			{
				return id3v2Tag.GetArtist();
			}
			else
			{
				if (id3v1Tag != null)
				{
					return id3v1Tag.GetArtist();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void SetArtist(string artist)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetArtist(artist);
			}
			if (id3v1Tag != null)
			{
				id3v1Tag.SetArtist(artist);
			}
		}

		public virtual string GetTitle()
		{
			if (id3v2Tag != null && id3v2Tag.GetTitle() != null && id3v2Tag.GetTitle().Length
				 > 0)
			{
				return id3v2Tag.GetTitle();
			}
			else
			{
				if (id3v1Tag != null)
				{
					return id3v1Tag.GetTitle();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void SetTitle(string title)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetTitle(title);
			}
			if (id3v1Tag != null)
			{
				id3v1Tag.SetTitle(title);
			}
		}

		public virtual string GetAlbum()
		{
			if (id3v2Tag != null && id3v2Tag.GetAlbum() != null && id3v2Tag.GetAlbum().Length
				 > 0)
			{
				return id3v2Tag.GetAlbum();
			}
			else
			{
				if (id3v1Tag != null)
				{
					return id3v1Tag.GetAlbum();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void SetAlbum(string album)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetAlbum(album);
			}
			if (id3v1Tag != null)
			{
				id3v1Tag.SetAlbum(album);
			}
		}

		public virtual string GetYear()
		{
			if (id3v2Tag != null && id3v2Tag.GetYear() != null && id3v2Tag.GetYear().Length >
				 0)
			{
				return id3v2Tag.GetYear();
			}
			else
			{
				if (id3v1Tag != null)
				{
					return id3v1Tag.GetYear();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void SetYear(string year)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetYear(year);
			}
			if (id3v1Tag != null)
			{
				id3v1Tag.SetYear(year);
			}
		}

		public virtual int GetGenre()
		{
			if (id3v1Tag != null && id3v1Tag.GetGenre() != -1)
			{
				return id3v1Tag.GetGenre();
			}
			else
			{
				if (id3v2Tag != null)
				{
					return id3v2Tag.GetGenre();
				}
				else
				{
					return -1;
				}
			}
		}

		public virtual void SetGenre(int genre)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetGenre(genre);
			}
			if (id3v1Tag != null)
			{
				id3v1Tag.SetGenre(genre);
			}
		}

		public virtual string GetGenreDescription()
		{
			if (id3v1Tag != null)
			{
				return id3v1Tag.GetGenreDescription();
			}
			else
			{
				if (id3v2Tag != null)
				{
					return id3v2Tag.GetGenreDescription();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual string GetComment()
		{
			if (id3v2Tag != null && id3v2Tag.GetComment() != null && id3v2Tag.GetComment().Length
				 > 0)
			{
				return id3v2Tag.GetComment();
			}
			else
			{
				if (id3v1Tag != null)
				{
					return id3v1Tag.GetComment();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void SetComment(string comment)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetComment(comment);
			}
			if (id3v1Tag != null)
			{
				id3v1Tag.SetComment(comment);
			}
		}

		public virtual string GetComposer()
		{
			if (id3v2Tag != null)
			{
				return id3v2Tag.GetComposer();
			}
			else
			{
				return null;
			}
		}

		public virtual void SetComposer(string composer)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetComposer(composer);
			}
		}

		public virtual string GetOriginalArtist()
		{
			if (id3v2Tag != null)
			{
				return id3v2Tag.GetOriginalArtist();
			}
			else
			{
				return null;
			}
		}

		public virtual void SetOriginalArtist(string originalArtist)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetOriginalArtist(originalArtist);
			}
		}

		public virtual void SetAlbumArtist(string albumArtist)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetAlbumArtist(albumArtist);
			}
		}

		public virtual string GetAlbumArtist()
		{
			if (id3v2Tag != null)
			{
				return id3v2Tag.GetAlbumArtist();
			}
			else
			{
				return null;
			}
		}

		public virtual string GetCopyright()
		{
			if (id3v2Tag != null)
			{
				return id3v2Tag.GetCopyright();
			}
			else
			{
				return null;
			}
		}

		public virtual void SetCopyright(string copyright)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetCopyright(copyright);
			}
		}

		public virtual string GetUrl()
		{
			if (id3v2Tag != null)
			{
				return id3v2Tag.GetUrl();
			}
			else
			{
				return null;
			}
		}

		public virtual void SetUrl(string url)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetUrl(url);
			}
		}

		public virtual string GetEncoder()
		{
			if (id3v2Tag != null)
			{
				return id3v2Tag.GetEncoder();
			}
			else
			{
				return null;
			}
		}

		public virtual void SetEncoder(string encoder)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetEncoder(encoder);
			}
		}

		public virtual byte[] GetAlbumImage()
		{
			if (id3v2Tag != null)
			{
				return id3v2Tag.GetAlbumImage();
			}
			else
			{
				return null;
			}
		}

		public virtual void SetAlbumImage(byte[] albumImage, string mimeType)
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.SetAlbumImage(albumImage, mimeType);
			}
		}

		public virtual string GetAlbumImageMimeType()
		{
			if (id3v2Tag != null)
			{
				return id3v2Tag.GetAlbumImageMimeType();
			}
			else
			{
				return null;
			}
		}

		public virtual void ClearComment()
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.ClearFrameSet(AbstractID3v2Tag.ID_COMMENT);
			}
			if (id3v1Tag != null)
			{
				id3v1Tag.SetComment(null);
			}
		}

		public virtual void ClearCopyright()
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.ClearFrameSet(AbstractID3v2Tag.ID_COPYRIGHT);
			}
		}

		public virtual void ClearEncoder()
		{
			if (id3v2Tag != null)
			{
				id3v2Tag.ClearFrameSet(AbstractID3v2Tag.ID_ENCODER);
			}
		}
	}
}
