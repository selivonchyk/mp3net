using System.Collections.Generic;
using Mp3net.Helpers;

namespace Mp3net
{
	public interface ID3v2 : ID3v1
	{
		bool GetPadding();

		void SetPadding(bool padding);

		bool HasFooter();

		void SetFooter(bool footer);

		bool HasUnsynchronisation();

		void SetUnsynchronisation(bool unsynchronisation);

		string GetComposer();

		void SetComposer(string composer);

		string GetPublisher();

		void SetPublisher(string publisher);

		string GetOriginalArtist();

		void SetOriginalArtist(string originalArtist);

		string GetAlbumArtist();

		void SetAlbumArtist(string albumArtist);

		string GetCopyright();

		void SetCopyright(string copyright);

		string GetUrl();

		void SetUrl(string url);

		string GetPartOfSet();

		void SetPartOfSet(string partOfSet);

		bool IsCompilation();

		void SetCompilation(bool compilation);

		IList<ID3v2ChapterFrameData> GetChapters();

		void SetChapters(IList<ID3v2ChapterFrameData> chapters);

		IList<ID3v2ChapterTOCFrameData> GetChapterTOC();

		void SetChapterTOC(IList<ID3v2ChapterTOCFrameData> ctoc);

		string GetEncoder();

		void SetEncoder(string encoder);

		byte[] GetAlbumImage();

		void SetAlbumImage(byte[] albumImage, string mimeType);

		string GetAlbumImageMimeType();

		string GetItunesComment();

		void SetItunesComment(string itunesComment);

		int GetDataLength();

		int GetLength();

		bool GetObseleteFormat();

		IDictionary<string, ID3v2FrameSet> GetFrameSets();

		void ClearFrameSet(string id);
	}
}
