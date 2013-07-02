using Mp3net.Helpers;

namespace Mp3net
{
	public class ID3v2ObseletePictureFrameData : ID3v2PictureFrameData
	{
		public ID3v2ObseletePictureFrameData(bool unsynchronisation) : base(unsynchronisation
			)
		{
		}

		public ID3v2ObseletePictureFrameData(bool unsynchronisation, string mimeType, byte
			 pictureType, EncodedText description, byte[] imageData) : base(unsynchronisation
			, mimeType, pictureType, description, imageData)
		{
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public ID3v2ObseletePictureFrameData(bool unsynchronisation, byte[] bytes) : base
			(unsynchronisation, bytes)
		{
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		protected internal override void UnpackFrameData(byte[] bytes)
		{
			string filetype;
			try
			{
				filetype = BufferTools.ByteBufferToString(bytes, 1, 3);
			}
			catch (UnsupportedEncodingException)
			{
				filetype = "unknown";
			}
			mimeType = "image/" + filetype.ToLower();
			pictureType = bytes[4];
			int marker = BufferTools.IndexOfTerminatorForEncoding(bytes, 5, bytes[0]);
			if (marker >= 0)
			{
				description = new EncodedText(bytes[0], BufferTools.CopyBuffer(bytes, 5, marker -
					 5));
				marker += description.GetTerminator().Length;
			}
			else
			{
				description = new EncodedText(bytes[0], string.Empty);
				marker = 1;
			}
			imageData = BufferTools.CopyBuffer(bytes, marker, bytes.Length - marker);
		}
	}
}
