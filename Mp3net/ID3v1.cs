namespace Mp3net
{
	public interface ID3v1
	{
		string GetVersion();

		string GetTrack();

		void SetTrack(string track);

		string GetArtist();

		void SetArtist(string artist);

		string GetTitle();

		void SetTitle(string title);

		string GetAlbum();

		void SetAlbum(string album);

		string GetYear();

		void SetYear(string year);

		int GetGenre();

		void SetGenre(int genre);

		string GetGenreDescription();

		string GetComment();

		void SetComment(string comment);

		/// <exception cref="Mp3net.NotSupportedException"></exception>
		byte[] ToBytes();
	}
}
