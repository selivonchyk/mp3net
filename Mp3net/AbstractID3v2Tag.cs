using System;
using System.Collections.Generic;
using Mp3net.Helpers;

namespace Mp3net
{
	public abstract class AbstractID3v2Tag : ID3v2
	{
		public static readonly string ID_IMAGE = "APIC";

		public static readonly string ID_ENCODER = "TENC";

		public static readonly string ID_URL = "WXXX";

		public static readonly string ID_COPYRIGHT = "TCOP";

		public static readonly string ID_ORIGINAL_ARTIST = "TOPE";

		public static readonly string ID_COMPOSER = "TCOM";

		public static readonly string ID_PUBLISHER = "TPUB";

		public static readonly string ID_COMMENT = "COMM";

		public static readonly string ID_GENRE = "TCON";

		public static readonly string ID_YEAR = "TYER";

		public static readonly string ID_ALBUM = "TALB";

		public static readonly string ID_TITLE = "TIT2";

		public static readonly string ID_ARTIST = "TPE1";

		public static readonly string ID_ALBUM_ARTIST = "TPE2";

		public static readonly string ID_TRACK = "TRCK";

		public static readonly string ID_PART_OF_SET = "TPOS";

		public static readonly string ID_COMPILATION = "TCMP";

		public static readonly string ID_CHAPTER_TOC = "CTOC";

		public static readonly string ID_CHAPTER = "CHAP";

		public static readonly string ID_IMAGE_OBSELETE = "PIC";

		public static readonly string ID_ENCODER_OBSELETE = "TEN";

		public static readonly string ID_URL_OBSELETE = "WXX";

		public static readonly string ID_COPYRIGHT_OBSELETE = "TCR";

		public static readonly string ID_ORIGINAL_ARTIST_OBSELETE = "TOA";

		public static readonly string ID_COMPOSER_OBSELETE = "TCM";

		public static readonly string ID_PUBLISHER_OBSELETE = "TBP";

		public static readonly string ID_COMMENT_OBSELETE = "COM";

		public static readonly string ID_GENRE_OBSELETE = "TCO";

		public static readonly string ID_YEAR_OBSELETE = "TYE";

		public static readonly string ID_ALBUM_OBSELETE = "TAL";

		public static readonly string ID_TITLE_OBSELETE = "TT2";

		public static readonly string ID_ARTIST_OBSELETE = "TP1";

		public static readonly string ID_ALBUM_ARTIST_OBSELETE = "TP2";

		public static readonly string ID_TRACK_OBSELETE = "TRK";

		public static readonly string ID_PART_OF_SET_OBSELETE = "TPA";

		public static readonly string ID_COMPILATION_OBSELETE = "TCP";

		protected internal static readonly string TAG = "ID3";

		protected internal static readonly string FOOTER_TAG = "3DI";

		protected internal const int HEADER_LENGTH = 10;

		protected internal const int FOOTER_LENGTH = 10;

		protected internal const int MAJOR_VERSION_OFFSET = 3;

		protected internal const int MINOR_VERSION_OFFSET = 4;

		protected internal const int FLAGS_OFFSET = 5;

		protected internal const int DATA_LENGTH_OFFSET = 6;

		protected internal const int FOOTER_BIT = 4;

		protected internal const int EXPERIMENTAL_BIT = 5;

		protected internal const int EXTENDED_HEADER_BIT = 6;

		protected internal const int COMPRESSION_BIT = 6;

		protected internal const int UNSYNCHRONISATION_BIT = 7;

		protected internal const int PADDING_LENGTH = 256;

		private static readonly string ITUNES_COMMENT_DESCRIPTION = "iTunNORM";

		protected internal bool unsynchronisation = false;

		protected internal bool extendedHeader = false;

		protected internal bool experimental = false;

		protected internal bool footer = false;

		protected internal bool compression = false;

		protected internal bool padding = false;

		protected internal string version = null;

		private int dataLength = 0;

		private int extendedHeaderLength;

		private byte[] extendedHeaderData;

		private bool obseleteFormat = false;

		private readonly IDictionary<string, ID3v2FrameSet> frameSets;

		public AbstractID3v2Tag()
		{
			frameSets = new SortedDictionary<string, ID3v2FrameSet>();
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public AbstractID3v2Tag(byte[] bytes) : this(bytes, false)
		{
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public AbstractID3v2Tag(byte[] bytes, bool obseleteFormat)
		{
			frameSets = new SortedDictionary<string, ID3v2FrameSet>();
			this.obseleteFormat = obseleteFormat;
			UnpackTag(bytes);
		}

		/// <exception cref="Mp3net.NoSuchTagException"></exception>
		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void UnpackTag(byte[] bytes)
		{
			ID3v2TagFactory.SanityCheckTag(bytes);
			int offset = UnpackHeader(bytes);
			try
			{
				if (extendedHeader)
				{
					offset = UnpackExtendedHeader(bytes, offset);
				}
				int framesLength = dataLength;
				if (footer)
				{
					framesLength -= 10;
				}
				offset = UnpackFrames(bytes, offset, framesLength);
				if (footer)
				{
					offset = UnpackFooter(bytes, dataLength);
				}
			}
			catch (IndexOutOfRangeException e)
			{
				throw new InvalidDataException("Premature end of tag", e);
			}
		}

		/// <exception cref="Mp3net.UnsupportedTagException"></exception>
		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private int UnpackHeader(byte[] bytes)
		{
			int majorVersion = bytes[MAJOR_VERSION_OFFSET];
			int minorVersion = bytes[MINOR_VERSION_OFFSET];
			version = majorVersion + "." + minorVersion;
			if (majorVersion != 2 && majorVersion != 3 && majorVersion != 4)
			{
				throw new UnsupportedTagException("Unsupported version " + version);
			}
			UnpackFlags(bytes);
			if ((bytes[FLAGS_OFFSET] & unchecked((int)(0x0F))) != 0)
			{
				throw new UnsupportedTagException("Unrecognised bits in header");
			}
			dataLength = BufferTools.UnpackSynchsafeInteger(bytes[DATA_LENGTH_OFFSET], bytes[
				DATA_LENGTH_OFFSET + 1], bytes[DATA_LENGTH_OFFSET + 2], bytes[DATA_LENGTH_OFFSET
				 + 3]);
			if (dataLength < 1)
			{
				throw new InvalidDataException("Zero size tag");
			}
			return HEADER_LENGTH;
		}

		protected internal abstract void UnpackFlags(byte[] bytes);

		private int UnpackExtendedHeader(byte[] bytes, int offset)
		{
			extendedHeaderLength = BufferTools.UnpackSynchsafeInteger(bytes[offset], bytes[offset
				 + 1], bytes[offset + 2], bytes[offset + 3]) + 4;
			extendedHeaderData = BufferTools.CopyBuffer(bytes, offset + 4, extendedHeaderLength
				);
			return extendedHeaderLength;
		}

		protected internal virtual int UnpackFrames(byte[] bytes, int offset, int framesLength
			)
		{
			int currentOffset = offset;
			while (currentOffset <= framesLength)
			{
				ID3v2Frame frame;
				try
				{
					frame = CreateFrame(bytes, currentOffset);
					AddFrame(frame, false);
					currentOffset += frame.GetLength();
				}
				catch (InvalidDataException)
				{
					break;
				}
			}
			return currentOffset;
		}

		private void AddFrame(ID3v2Frame frame, bool replace)
		{
			ID3v2FrameSet frameSet = frameSets.Get(frame.GetId());
			if (frameSet == null)
			{
				frameSet = new ID3v2FrameSet(frame.GetId());
				frameSet.AddFrame(frame);
				frameSets.Put(frame.GetId(), frameSet);
			}
			else
			{
				if (replace)
				{
					frameSet.Clear();
					frameSet.AddFrame(frame);
				}
				else
				{
					frameSet.AddFrame(frame);
				}
			}
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal virtual ID3v2Frame CreateFrame(byte[] bytes, int currentOffset
			)
		{
			if (obseleteFormat)
			{
				return new ID3v2ObseleteFrame(bytes, currentOffset);
			}
			return new ID3v2Frame(bytes, currentOffset);
		}

		protected internal virtual ID3v2Frame CreateFrame(string id, byte[] data)
		{
			if (obseleteFormat)
			{
				return new ID3v2ObseleteFrame(id, data);
			}
			else
			{
				return new ID3v2Frame(id, data);
			}
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private int UnpackFooter(byte[] bytes, int offset)
		{
			if (!FOOTER_TAG.Equals(BufferTools.ByteBufferToStringIgnoringEncodingIssues(bytes
				, offset, FOOTER_TAG.Length)))
			{
				throw new InvalidDataException("Invalid footer");
			}
			return FOOTER_LENGTH;
		}

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		public virtual byte[] ToBytes()
		{
			byte[] bytes = new byte[GetLength()];
			PackTag(bytes);
			return bytes;
		}

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		public virtual void PackTag(byte[] bytes)
		{
			int offset = PackHeader(bytes, 0);
			if (extendedHeader)
			{
				offset = PackExtendedHeader(bytes, offset);
			}
			offset = PackFrames(bytes, offset);
			if (footer)
			{
				offset = PackFooter(bytes, dataLength);
			}
		}

		private int PackHeader(byte[] bytes, int offset)
		{
			try
			{
				BufferTools.StringIntoByteBuffer(TAG, 0, TAG.Length, bytes, offset);
			}
			catch (UnsupportedEncodingException)
			{
			}
			string[] s = version.Split("\\.");
			if (s.Length > 0)
			{
                byte majorVersion = Convert.ToByte(s[0]);
				bytes[offset + MAJOR_VERSION_OFFSET] = majorVersion;
			}
			if (s.Length > 1)
			{
                byte minorVersion = Convert.ToByte(s[1]);
				bytes[offset + MINOR_VERSION_OFFSET] = minorVersion;
			}
			PackFlags(bytes, offset);
			BufferTools.PackSynchsafeInteger(GetDataLength(), bytes, offset + DATA_LENGTH_OFFSET
				);
			return offset + HEADER_LENGTH;
		}

		protected internal abstract void PackFlags(byte[] bytes, int i);

		private int PackExtendedHeader(byte[] bytes, int offset)
		{
			BufferTools.PackSynchsafeInteger(extendedHeaderLength, bytes, offset);
			BufferTools.CopyIntoByteBuffer(extendedHeaderData, 0, extendedHeaderData.Length, 
				bytes, offset + 4);
			return offset + 4 + extendedHeaderData.Length;
		}

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		public virtual int PackFrames(byte[] bytes, int offset)
		{
			int newOffset = PackSpecifiedFrames(bytes, offset, null, "APIC");
			newOffset = PackSpecifiedFrames(bytes, newOffset, "APIC", null);
			return newOffset;
		}

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		private int PackSpecifiedFrames(byte[] bytes, int offset, string onlyId, string notId
			)
		{
			Iterator<ID3v2FrameSet> setIterator = frameSets.Values.Iterator();
			while (setIterator.HasNext())
			{
				ID3v2FrameSet frameSet = setIterator.Next();
				if ((onlyId == null || onlyId.Equals(frameSet.GetId())) && (notId == null || !notId
					.Equals(frameSet.GetId())))
				{
					Iterator<ID3v2Frame> frameIterator = frameSet.GetFrames().Iterator();
					while (frameIterator.HasNext())
					{
						ID3v2Frame frame = (ID3v2Frame)frameIterator.Next();
						if (frame.GetDataLength() > 0)
						{
							byte[] frameData = frame.ToBytes();
							BufferTools.CopyIntoByteBuffer(frameData, 0, frameData.Length, bytes, offset);
							offset += frameData.Length;
						}
					}
				}
			}
			return offset;
		}

		private int PackFooter(byte[] bytes, int offset)
		{
			try
			{
				BufferTools.StringIntoByteBuffer(FOOTER_TAG, 0, FOOTER_TAG.Length, bytes, offset);
			}
			catch (UnsupportedEncodingException)
			{
			}
			string[] s = version.Split("\\.");
			if (s.Length > 0)
			{
                byte majorVersion = Convert.ToByte(s[0]);
				bytes[offset + MAJOR_VERSION_OFFSET] = majorVersion;
			}
			if (s.Length > 1)
			{
                byte minorVersion = Convert.ToByte(s[1]);
				bytes[offset + MINOR_VERSION_OFFSET] = minorVersion;
			}
			PackFlags(bytes, offset);
			BufferTools.PackSynchsafeInteger(GetDataLength(), bytes, offset + DATA_LENGTH_OFFSET
				);
			return offset + FOOTER_LENGTH;
		}

		private int CalculateDataLength()
		{
			int length = 0;
			if (extendedHeader)
			{
				length += extendedHeaderLength;
			}
			if (footer)
			{
				length += FOOTER_LENGTH;
			}
			else
			{
				if (padding)
				{
					length += PADDING_LENGTH;
				}
			}
			Iterator<ID3v2FrameSet> setIterator = frameSets.Values.Iterator();
			while (setIterator.HasNext())
			{
				ID3v2FrameSet frameSet = setIterator.Next();
				Iterator<ID3v2Frame> frameIterator = frameSet.GetFrames().Iterator();
				while (frameIterator.HasNext())
				{
					ID3v2Frame frame = (ID3v2Frame)frameIterator.Next();
					length += frame.GetLength();
				}
			}
			return length;
		}

		protected internal virtual bool UseFrameUnsynchronisation()
		{
			return false;
		}

		public virtual string GetVersion()
		{
			return version;
		}

		private void InvalidateDataLength()
		{
			dataLength = 0;
		}

		public virtual int GetDataLength()
		{
			if (dataLength == 0)
			{
				dataLength = CalculateDataLength();
			}
			return dataLength;
		}

		public virtual int GetLength()
		{
			return GetDataLength() + HEADER_LENGTH;
		}

		public virtual IDictionary<string, ID3v2FrameSet> GetFrameSets()
		{
			return frameSets;
		}

		public virtual bool GetPadding()
		{
			return padding;
		}

		public virtual void SetPadding(bool padding)
		{
			if (this.padding != padding)
			{
				InvalidateDataLength();
				this.padding = padding;
			}
		}

		public virtual bool HasFooter()
		{
			return footer;
		}

		public virtual void SetFooter(bool footer)
		{
			if (this.footer != footer)
			{
				InvalidateDataLength();
				this.footer = footer;
			}
		}

		public virtual bool HasUnsynchronisation()
		{
			return unsynchronisation;
		}

		public virtual void SetUnsynchronisation(bool unsynchronisation)
		{
			if (this.unsynchronisation != unsynchronisation)
			{
				InvalidateDataLength();
				this.unsynchronisation = unsynchronisation;
			}
		}

		public virtual bool GetObseleteFormat()
		{
			return obseleteFormat;
		}

		public virtual string GetTrack()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_TRACK_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_TRACK);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetTrack(string track)
		{
			if (track != null && track.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(track));
				AddFrame(CreateFrame(ID_TRACK, frameData.ToBytes()), true);
			}
		}

		public virtual string GetPartOfSet()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_PART_OF_SET_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_PART_OF_SET);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetPartOfSet(string partOfSet)
		{
			if (partOfSet != null && partOfSet.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(partOfSet));
				AddFrame(CreateFrame(ID_PART_OF_SET, frameData.ToBytes()), true);
			}
		}

		public virtual bool IsCompilation()
		{
			// unofficial frame used by iTunes
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_COMPILATION_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_COMPILATION);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return "1".Equals(frameData.GetText().ToString());
			}
			return false;
		}

		public virtual void SetCompilation(bool compilation)
		{
			InvalidateDataLength();
			ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
				, new EncodedText(compilation ? "1" : "0"));
			AddFrame(CreateFrame(ID_COMPILATION, frameData.ToBytes()), true);
		}

		public virtual string GetArtist()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_ARTIST_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_ARTIST);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetArtist(string artist)
		{
			if (artist != null && artist.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(artist));
				AddFrame(CreateFrame(ID_ARTIST, frameData.ToBytes()), true);
			}
		}

		public virtual string GetAlbumArtist()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_ALBUM_ARTIST_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_ALBUM_ARTIST);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetAlbumArtist(string albumArtist)
		{
			if (albumArtist != null && albumArtist.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(albumArtist));
				AddFrame(CreateFrame(ID_ALBUM_ARTIST, frameData.ToBytes()), true);
			}
		}

		public virtual string GetTitle()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_TITLE_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_TITLE);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetTitle(string title)
		{
			if (title != null && title.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(title));
				AddFrame(CreateFrame(ID_TITLE, frameData.ToBytes()), true);
			}
		}

		public virtual string GetAlbum()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_ALBUM_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_ALBUM);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetAlbum(string album)
		{
			if (album != null && album.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(album));
				AddFrame(CreateFrame(ID_ALBUM, frameData.ToBytes()), true);
			}
		}

		public virtual string GetYear()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_YEAR_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_YEAR);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetYear(string year)
		{
			if (year != null && year.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(year));
				AddFrame(CreateFrame(ID_YEAR, frameData.ToBytes()), true);
			}
		}

		public virtual int GetGenre()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_GENRE_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_GENRE);
			}
			if (frameData == null || frameData.GetText() == null)
			{
				return -1;
			}
			string text = frameData.GetText().ToString();
			if (text == null || text.Length == 0)
			{
				return -1;
			}
			try
			{
				return ExtractGenreNumber(text);
			}
			catch (FormatException)
			{
				string description = ExtractGenreDescription(text);
				if (description != null && description.Length > 0)
				{
					for (int i = 0; i < ID3v1Genres.GENRES.Length; i++)
					{
                        if (String.Equals(ID3v1Genres.GENRES[i], description, StringComparison.OrdinalIgnoreCase))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public virtual void SetGenre(int genre)
		{
			if (genre >= 0)
			{
				InvalidateDataLength();
				string genreDescription;
				try
				{
					genreDescription = ID3v1Genres.GENRES[genre];
				}
				catch (IndexOutOfRangeException)
				{
					genreDescription = string.Empty;
				}
				string combinedGenre = "(" + Mp3net.Helpers.Extensions.ToString(genre) + ")" + genreDescription;
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(combinedGenre));
				AddFrame(CreateFrame(ID_GENRE, frameData.ToBytes()), true);
			}
		}

		public virtual string GetGenreDescription()
		{
			int genreNum = GetGenre();
			if (genreNum >= 0)
			{
				try
				{
					return ID3v1Genres.GENRES[genreNum];
				}
				catch (IndexOutOfRangeException)
				{
					return null;
				}
			}
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_GENRE_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_GENRE);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				string text = frameData.GetText().ToString();
				if (text != null && text.Length > 0)
				{
					string description = ExtractGenreDescription(text);
					if (description != null)
					{
						return description;
					}
				}
			}
			return null;
		}

		/// <exception cref="System.FormatException"></exception>
		protected internal virtual int ExtractGenreNumber(string genreValue)
		{
			string value = genreValue.Trim();
			if (value.Length > 0)
			{
				if (value[0] == '(')
				{
					int pos = value.IndexOf(')');
					if (pos > 0)
					{
						return System.Convert.ToInt32(value.Substring(1, pos-1));
					}
				}
			}
			return System.Convert.ToInt32(value);
		}

		/// <exception cref="System.FormatException"></exception>
		protected internal virtual string ExtractGenreDescription(string genreValue)
		{
			string value = genreValue.Trim();
			if (value.Length > 0)
			{
				if (value[0] == '(')
				{
					int pos = value.IndexOf(')');
					if (pos > 0)
					{
						return value.Substring(pos + 1);
					}
				}
				return value;
			}
			return null;
		}

		public virtual string GetComment()
		{
			ID3v2CommentFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractCommentFrameData(ID_COMMENT_OBSELETE, false);
			}
			else
			{
				frameData = ExtractCommentFrameData(ID_COMMENT, false);
			}
			if (frameData != null && frameData.GetComment() != null)
			{
				return frameData.GetComment().ToString();
			}
			return null;
		}

		public virtual void SetComment(string comment)
		{
			if (comment != null && comment.Length > 0)
			{
				InvalidateDataLength();
				ID3v2CommentFrameData frameData = new ID3v2CommentFrameData(UseFrameUnsynchronisation
					(), "eng", null, new EncodedText(comment));
				AddFrame(CreateFrame(ID_COMMENT, frameData.ToBytes()), true);
			}
		}

		public virtual string GetItunesComment()
		{
			ID3v2CommentFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractCommentFrameData(ID_COMMENT_OBSELETE, true);
			}
			else
			{
				frameData = ExtractCommentFrameData(ID_COMMENT, true);
			}
			if (frameData != null && frameData.GetComment() != null)
			{
				return frameData.GetComment().ToString();
			}
			return null;
		}

		public virtual void SetItunesComment(string itunesComment)
		{
			if (itunesComment != null && itunesComment.Length > 0)
			{
				InvalidateDataLength();
				ID3v2CommentFrameData frameData = new ID3v2CommentFrameData(UseFrameUnsynchronisation
					(), ITUNES_COMMENT_DESCRIPTION, null, new EncodedText(itunesComment));
				AddFrame(CreateFrame(ID_COMMENT, frameData.ToBytes()), true);
			}
		}

		public virtual string GetComposer()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_COMPOSER_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_COMPOSER);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetComposer(string composer)
		{
			if (composer != null && composer.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(composer));
				AddFrame(CreateFrame(ID_COMPOSER, frameData.ToBytes()), true);
			}
		}

		public virtual string GetPublisher()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_PUBLISHER_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_PUBLISHER);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetPublisher(string publisher)
		{
			if (publisher != null && publisher.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(publisher));
				AddFrame(CreateFrame(ID_PUBLISHER, frameData.ToBytes()), true);
			}
		}

		public virtual string GetOriginalArtist()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_ORIGINAL_ARTIST_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_ORIGINAL_ARTIST);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetOriginalArtist(string originalArtist)
		{
			if (originalArtist != null && originalArtist.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(originalArtist));
				AddFrame(CreateFrame(ID_ORIGINAL_ARTIST, frameData.ToBytes()), true);
			}
		}

		public virtual string GetCopyright()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_COPYRIGHT_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_COPYRIGHT);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetCopyright(string copyright)
		{
			if (copyright != null && copyright.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(copyright));
				AddFrame(CreateFrame(ID_COPYRIGHT, frameData.ToBytes()), true);
			}
		}

		public virtual string GetUrl()
		{
			ID3v2UrlFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractUrlFrameData(ID_URL_OBSELETE);
			}
			else
			{
				frameData = ExtractUrlFrameData(ID_URL);
			}
			if (frameData != null)
			{
				return frameData.GetUrl();
			}
			return null;
		}

		public virtual void SetUrl(string url)
		{
			if (url != null && url.Length > 0)
			{
				InvalidateDataLength();
				ID3v2UrlFrameData frameData = new ID3v2UrlFrameData(UseFrameUnsynchronisation(), 
					null, url);
				AddFrame(CreateFrame(ID_URL, frameData.ToBytes()), true);
			}
		}

		public virtual IList<ID3v2ChapterFrameData> GetChapters()
		{
			if (obseleteFormat)
			{
				return null;
			}
			return ExtractChapterFrameData(ID_CHAPTER);
		}

		public virtual void SetChapters(IList<ID3v2ChapterFrameData> chapters)
		{
			if (chapters != null)
			{
				InvalidateDataLength();
				bool first = true;
				foreach (ID3v2ChapterFrameData chapter in chapters)
				{
					if (first)
					{
						first = false;
						AddFrame(CreateFrame(ID_CHAPTER, chapter.ToBytes()), true);
					}
					else
					{
						AddFrame(CreateFrame(ID_CHAPTER, chapter.ToBytes()), false);
					}
				}
			}
		}

		public virtual IList<ID3v2ChapterTOCFrameData> GetChapterTOC()
		{
			if (obseleteFormat)
			{
				return null;
			}
			return ExtractChapterTOCFrameData(ID_CHAPTER_TOC);
		}

		public virtual void SetChapterTOC(IList<ID3v2ChapterTOCFrameData> toc)
		{
			if (toc != null)
			{
				InvalidateDataLength();
				bool first = true;
				foreach (ID3v2ChapterTOCFrameData ct in toc)
				{
					if (first)
					{
						first = false;
						AddFrame(CreateFrame(ID_CHAPTER_TOC, ct.ToBytes()), true);
					}
					else
					{
						AddFrame(CreateFrame(ID_CHAPTER_TOC, ct.ToBytes()), false);
					}
				}
			}
		}

		public virtual string GetEncoder()
		{
			ID3v2TextFrameData frameData;
			if (obseleteFormat)
			{
				frameData = ExtractTextFrameData(ID_ENCODER_OBSELETE);
			}
			else
			{
				frameData = ExtractTextFrameData(ID_ENCODER);
			}
			if (frameData != null && frameData.GetText() != null)
			{
				return frameData.GetText().ToString();
			}
			return null;
		}

		public virtual void SetEncoder(string encoder)
		{
			if (encoder != null && encoder.Length > 0)
			{
				InvalidateDataLength();
				ID3v2TextFrameData frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation()
					, new EncodedText(encoder));
				AddFrame(CreateFrame(ID_ENCODER, frameData.ToBytes()), true);
			}
		}

		public virtual byte[] GetAlbumImage()
		{
			ID3v2PictureFrameData frameData;
			if (obseleteFormat)
			{
				frameData = CreatePictureFrameData(ID_IMAGE_OBSELETE);
			}
			else
			{
				frameData = CreatePictureFrameData(ID_IMAGE);
			}
			if (frameData != null)
			{
				return frameData.GetImageData();
			}
			return null;
		}

		public virtual void SetAlbumImage(byte[] albumImage, string mimeType)
		{
			if (albumImage != null && albumImage.Length > 0 && mimeType != null && mimeType.Length
				 > 0)
			{
				InvalidateDataLength();
				ID3v2PictureFrameData frameData = new ID3v2PictureFrameData(UseFrameUnsynchronisation
					(), mimeType, unchecked((byte)0), null, albumImage);
				AddFrame(CreateFrame(ID_IMAGE, frameData.ToBytes()), true);
			}
		}

		public virtual string GetAlbumImageMimeType()
		{
			ID3v2PictureFrameData frameData;
			if (obseleteFormat)
			{
				frameData = CreatePictureFrameData(ID_IMAGE_OBSELETE);
			}
			else
			{
				frameData = CreatePictureFrameData(ID_IMAGE);
			}
			if (frameData != null && frameData.GetMimeType() != null)
			{
				return frameData.GetMimeType();
			}
			return null;
		}

		public virtual void ClearFrameSet(string id)
		{
			if (Collections.Remove(frameSets, id) != null)
			{
				InvalidateDataLength();
			}
		}

		private IList<ID3v2ChapterFrameData> ExtractChapterFrameData(string id)
		{
			ID3v2FrameSet frameSet = frameSets.Get(id);
			if (frameSet != null)
			{
				IList<ID3v2ChapterFrameData> chapterData = new List<ID3v2ChapterFrameData>();
				IList<ID3v2Frame> frames = frameSet.GetFrames();
				foreach (ID3v2Frame frame in frames)
				{
					ID3v2ChapterFrameData frameData;
					try
					{
						frameData = new ID3v2ChapterFrameData(UseFrameUnsynchronisation(), frame.GetData(
							));
						chapterData.AddItem(frameData);
					}
					catch (InvalidDataException)
					{
					}
				}
				// do nothing
				return chapterData;
			}
			return null;
		}

		private IList<ID3v2ChapterTOCFrameData> ExtractChapterTOCFrameData(string id)
		{
			ID3v2FrameSet frameSet = frameSets.Get(id);
			if (frameSet != null)
			{
				IList<ID3v2ChapterTOCFrameData> chapterData = new List<ID3v2ChapterTOCFrameData>();
				IList<ID3v2Frame> frames = frameSet.GetFrames();
				foreach (ID3v2Frame frame in frames)
				{
					ID3v2ChapterTOCFrameData frameData;
					try
					{
						frameData = new ID3v2ChapterTOCFrameData(UseFrameUnsynchronisation(), frame.GetData
							());
						chapterData.AddItem(frameData);
					}
					catch (InvalidDataException)
					{
					}
				}
				// do nothing
				return chapterData;
			}
			return null;
		}

		private ID3v2TextFrameData ExtractTextFrameData(string id)
		{
			ID3v2FrameSet frameSet = frameSets.Get(id);
			if (frameSet != null)
			{
				ID3v2Frame frame = (ID3v2Frame)frameSet.GetFrames()[0];
				ID3v2TextFrameData frameData;
				try
				{
					frameData = new ID3v2TextFrameData(UseFrameUnsynchronisation(), frame.GetData());
					return frameData;
				}
				catch (InvalidDataException)
				{
				}
			}
			// do nothing
			return null;
		}

		private ID3v2UrlFrameData ExtractUrlFrameData(string id)
		{
			ID3v2FrameSet frameSet = frameSets.Get(id);
			if (frameSet != null)
			{
				ID3v2Frame frame = (ID3v2Frame)frameSet.GetFrames()[0];
				ID3v2UrlFrameData frameData;
				try
				{
					frameData = new ID3v2UrlFrameData(UseFrameUnsynchronisation(), frame.GetData());
					return frameData;
				}
				catch (InvalidDataException)
				{
				}
			}
			// do nothing
			return null;
		}

		private ID3v2CommentFrameData ExtractCommentFrameData(string id, bool itunes)
		{
			ID3v2FrameSet frameSet = frameSets.Get(id);
			if (frameSet != null)
			{
				Iterator<ID3v2Frame> iterator = frameSet.GetFrames().Iterator();
				while (iterator.HasNext())
				{
					ID3v2Frame frame = (ID3v2Frame)iterator.Next();
					ID3v2CommentFrameData frameData;
					try
					{
						frameData = new ID3v2CommentFrameData(UseFrameUnsynchronisation(), frame.GetData(
							));
						if (itunes && ITUNES_COMMENT_DESCRIPTION.Equals(frameData.GetDescription().ToString
							()))
						{
							return frameData;
						}
						else
						{
							if (!itunes)
							{
								return frameData;
							}
						}
					}
					catch (InvalidDataException)
					{
					}
				}
			}
			// Do nothing
			return null;
		}

		private ID3v2PictureFrameData CreatePictureFrameData(string id)
		{
			ID3v2FrameSet frameSet = frameSets.Get(id);
			if (frameSet != null)
			{
				ID3v2Frame frame = (ID3v2Frame)frameSet.GetFrames()[0];
				ID3v2PictureFrameData frameData;
				try
				{
					if (obseleteFormat)
					{
						frameData = new ID3v2ObseletePictureFrameData(UseFrameUnsynchronisation(), frame.
							GetData());
					}
					else
					{
						frameData = new ID3v2PictureFrameData(UseFrameUnsynchronisation(), frame.GetData(
							));
					}
					return frameData;
				}
				catch (InvalidDataException)
				{
				}
			}
			// do nothing
			return null;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.AbstractID3v2Tag))
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			Mp3net.AbstractID3v2Tag other = (Mp3net.AbstractID3v2Tag)obj;
			if (unsynchronisation != other.unsynchronisation)
			{
				return false;
			}
			if (extendedHeader != other.extendedHeader)
			{
				return false;
			}
			if (experimental != other.experimental)
			{
				return false;
			}
			if (footer != other.footer)
			{
				return false;
			}
			if (compression != other.compression)
			{
				return false;
			}
			if (dataLength != other.dataLength)
			{
				return false;
			}
			if (extendedHeaderLength != other.extendedHeaderLength)
			{
				return false;
			}
			if (version == null)
			{
				if (other.version != null)
				{
					return false;
				}
			}
			else
			{
				if (other.version == null)
				{
					return false;
				}
				else
				{
					if (!version.Equals(other.version))
					{
						return false;
					}
				}
			}
			if (frameSets == null)
			{
				if (other.frameSets != null)
				{
					return false;
				}
			}
			else
			{
				if (other.frameSets == null)
				{
					return false;
				}
				else
				{
				    if (frameSets == other.frameSets)
				    {
				        return true;
				    }
				    if ((frameSets == null) || (other.frameSets == null))
				    {
				        return false;
				    }
				    if (frameSets.Count != other.frameSets.Count)
				    {
				        return false;
				    }
                    var comparer = EqualityComparer<ID3v2FrameSet>.Default;
                    foreach (KeyValuePair<string, ID3v2FrameSet> kvp in frameSets)
                    {
                        ID3v2FrameSet secondValue;
                        if (!other.frameSets.TryGetValue(kvp.Key, out secondValue))
                        {
                            return false;
                        }
                        if (!comparer.Equals(kvp.Value, secondValue))
                        {
                            return false;
                        }
                    }
                    return true;
				}
			}
			return true;
		}
	}
}
