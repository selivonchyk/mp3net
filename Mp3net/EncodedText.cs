using System;
using System.Text;
using Mp3net.Helpers;
using Ude;

namespace Mp3net
{
	public class EncodedText
	{
		public const byte TEXT_ENCODING_ISO_8859_1 = 0;

		public const byte TEXT_ENCODING_UTF_16 = 1;

		public const byte TEXT_ENCODING_UTF_16BE = 2;

		public const byte TEXT_ENCODING_UTF_8 = 3;

		public static readonly string CHARSET_ISO_8859_1 = "ISO-8859-1";

		public static readonly string CHARSET_UTF_16 = "UTF-16LE";

		public static readonly string CHARSET_UTF_16BE = "UTF-16BE";

		public static readonly string CHARSET_UTF_8 = "UTF-8";

        /* original 
		private static readonly string[] characterSets = new string[] {
            CHARSET_ISO_8859_1, 
            CHARSET_UTF_16, 
            CHARSET_UTF_16BE, 
            CHARSET_UTF_8
        };
        */

        private static readonly string[] characterSets = new string[] {
            CHARSET_ISO_8859_1, 
            CHARSET_UTF_16BE, 
            CHARSET_UTF_16, 
            CHARSET_UTF_8
        };


		private static readonly byte[] textEncodingFallback = new byte[] { 0, 2, 1, 3 };

        /* original
		private static readonly byte[][] boms = new byte[][] { 
            new byte[] {  }, 
            new byte[] { 
                unchecked((byte)unchecked((int)(0xff))), 
                unchecked((byte)unchecked((int)(0xfe))) 
            }, 
            new byte[] { 
                unchecked((byte)unchecked((int)(0xfe))), 
                unchecked((byte)unchecked((int)(0xff))) 
            }, 
            new byte[] {  } 
        };
        */

        private static readonly byte[][] boms = new byte[][] { 
            new byte[] {  }, 
            new byte[] { 
                unchecked((byte)unchecked((int)(0xfe))), 
                unchecked((byte)unchecked((int)(0xff))) 
            }, 
            new byte[] { 
                unchecked((byte)unchecked((int)(0xff))), 
                unchecked((byte)unchecked((int)(0xfe))) 
            }, 
            new byte[] {  } 
        };


		private static readonly byte[][] terminators = new byte[][] { 
            new byte[] { 0 }, 
            new byte[] { 0, 0 }, 
            new byte[] { 0, 0 }, 
            new byte[] { 0 } };

		private byte[] value;

		private byte textEncoding;

		public EncodedText(byte textEncoding, byte[] value)
		{
			this.textEncoding = textEncoding;
			this.value = value;
			this.StripBomAndTerminator();
		}

		/// <exception cref="System.ArgumentException"></exception>
		public EncodedText(string @string)
		{
			foreach (byte textEncoding in textEncodingFallback)
			{
				this.textEncoding = textEncoding;
				value = StringToBytes(@string, CharacterSetForTextEncoding(textEncoding));
				if (value != null && this.ToString() != null)
				{
					this.StripBomAndTerminator();
					return;
				}
			}
			throw new ArgumentException("Invalid string, could not find appropriate encoding");
		}

		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="Mp3net.Helpers.CharacterCodingException"></exception>
		public EncodedText(string @string, byte transcodeToTextEncoding) : this(@string)
		{
			SetTextEncoding(transcodeToTextEncoding, true);
		}

		public EncodedText(byte textEncoding, string @string)
		{
            if (textEncoding == 0)
                textEncoding = 1; // Use UTF-16 instead of ISO-8859-1/Windows-1251
			this.textEncoding = textEncoding;
			value = StringToBytes(@string, CharacterSetForTextEncoding(textEncoding));
			this.StripBomAndTerminator();
		}

		public EncodedText(byte[] value) : this(TextEncodingForBytesFromBOM(value), value)
		{
		}

		private static byte TextEncodingForBytesFromBOM(byte[] value)
		{
			if (value.Length >= 2 && value[0] == unchecked((byte)unchecked((int)(0xff))) && value
				[1] == unchecked((byte)unchecked((int)(0xfe))))
			{
				return TEXT_ENCODING_UTF_16;
			}
			else
			{
				if (value.Length >= 2 && value[0] == unchecked((byte)unchecked((int)(0xfe))) && value
					[1] == unchecked((byte)unchecked((int)(0xff))))
				{
					return TEXT_ENCODING_UTF_16BE;
				}
				else
				{
					if (value.Length >= 3 && (value[0] == unchecked((byte)unchecked((int)(0xef))) && 
						value[1] == unchecked((byte)unchecked((int)(0xbb))) && value[2] == unchecked((byte
						)unchecked((int)(0xbf)))))
					{
						return TEXT_ENCODING_UTF_8;
					}
					else
					{
						return TEXT_ENCODING_ISO_8859_1;
					}
				}
			}
		}

		private static string CharacterSetForTextEncoding(byte textEncoding)
		{
			try
			{
				return characterSets[textEncoding];
			}
			catch (IndexOutOfRangeException)
			{
				throw new ArgumentException("Invalid text encoding " + textEncoding);
			}
		}

		private void StripBomAndTerminator()
		{
			int leadingCharsToRemove = 0;
			if (value.Length >= 2 &&
                ((value[0] == unchecked((byte)unchecked((int)(0xfe))) && value[1] == unchecked((byte)unchecked((int)(0xff)))) ||
                 (value[0] == unchecked((byte)unchecked((int)(0xff))) && value[1] == unchecked((byte)unchecked((int)(0xfe))))))
			{
				leadingCharsToRemove = 2;
			}
			else
			{
				if (value.Length >= 3 &&
                    (value[0] == unchecked((byte)unchecked((int)(0xef))) &&
                    value[1] == unchecked((byte)unchecked((int)(0xbb))) &&
                    value[2] == unchecked((byte)unchecked((int)(0xbf)))))
				{
					leadingCharsToRemove = 3;
				}
			}
			int trailingCharsToRemove = 0;
			byte[] terminator = terminators[textEncoding];
			if (value.Length - leadingCharsToRemove >= terminator.Length)
			{
				bool haveTerminator = true;
				for (int i = 0; i < terminator.Length; i++)
				{
					if (value[value.Length - terminator.Length + i] != terminator[i])
					{
						haveTerminator = false;
						break;
					}
				}
				if (haveTerminator)
				{
					trailingCharsToRemove = terminator.Length;
				}
			}
			if (leadingCharsToRemove + trailingCharsToRemove > 0)
			{
				int newLength = value.Length - leadingCharsToRemove - trailingCharsToRemove;
				byte[] newValue = new byte[newLength];
				if (newLength > 0)
				{
					System.Array.Copy(value, leadingCharsToRemove, newValue, 0, newValue.Length);
				}
				value = newValue;
			}
		}

		public virtual byte GetTextEncoding()
		{
			return textEncoding;
		}

		/// <exception cref="Mp3net.Helpers.CharacterCodingException"></exception>
		public virtual void SetTextEncoding(byte textEncoding)
		{
			SetTextEncoding(textEncoding, true);
		}

		/// <exception cref="Mp3net.Helpers.CharacterCodingException"></exception>
		public virtual void SetTextEncoding(byte textEncoding, bool transcode)
		{
			if (this.textEncoding != textEncoding)
			{
				string charBuffer = BytesToCharBuffer(this.value, CharacterSetForTextEncoding(this.textEncoding));
				byte[] transcodedBytes = CharBufferToBytes(charBuffer, CharacterSetForTextEncoding
					(textEncoding));
				this.textEncoding = textEncoding;
				this.value = transcodedBytes;
			}
		}

		public virtual byte[] GetTerminator()
		{
			return terminators[textEncoding];
		}

		public virtual byte[] ToBytes()
		{
			return ToBytes(false, false);
		}

		public virtual byte[] ToBytes(bool includeBom)
		{
			return ToBytes(includeBom, false);
		}

		public virtual byte[] ToBytes(bool includeBom, bool includeTerminator)
		{
			CharacterSetForTextEncoding(textEncoding);
			// ensured textEncoding is valid
			int newLength = value.Length + (includeBom ? boms[textEncoding].Length : 0) + (includeTerminator
				 ? GetTerminator().Length : 0);
			if (newLength == value.Length)
			{
				return value;
			}
			else
			{
				byte[] bytes = new byte[newLength];
				int i = 0;
				if (includeBom)
				{
					byte[] bom = boms[textEncoding];
					if (bom.Length > 0)
					{
						System.Array.Copy(boms[textEncoding], 0, bytes, i, boms[textEncoding].Length);
						i += boms[textEncoding].Length;
					}
				}
				if (value.Length > 0)
				{
					System.Array.Copy(value, 0, bytes, i, value.Length);
					i += value.Length;
				}
				if (includeTerminator)
				{
					byte[] terminator = GetTerminator();
					if (terminator.Length > 0)
					{
						System.Array.Copy(terminator, 0, bytes, i, terminator.Length);
					}
				}
				return bytes;
			}
		}

		public override string ToString()
		{
			try
			{
                string charsetName = textEncoding == 0 ? DetectCharset() : CharacterSetForTextEncoding(textEncoding);
                return BytesToString(value, charsetName);
			}
			catch (CharacterCodingException)
			{
				return null;
			}
		}

		public virtual string GetCharacterSet()
		{
			return CharacterSetForTextEncoding(textEncoding);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Mp3net.EncodedText))
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			Mp3net.EncodedText other = (Mp3net.EncodedText)obj;
			if (textEncoding != other.textEncoding)
			{
				return false;
			}
			if (!Arrays.Equals(value, other.value))
			{
				return false;
			}
			return true;
		}

		/// <exception cref="Mp3net.Helpers.CharacterCodingException"></exception>
		public static string BytesToString(byte[] bytes, string characterSet)
		{
			string s = BytesToCharBuffer(bytes, characterSet);
			int length = s.IndexOf('\0');
			if (length == -1)
			{
				return s;
			}
			return s.Substring(0, length);
		}

		/// <exception cref="Mp3net.Helpers.CharacterCodingException"></exception>
		internal static string BytesToCharBuffer(byte[] bytes, string characterSet)
		{
			Encoding charset = GetEncoding(characterSet);
            return charset.GetString(bytes);
		}

		public static byte[] StringToBytes(string s, string characterSet)
		{
			try
			{
				return CharBufferToBytes(s, characterSet);
			}
			catch (CharacterCodingException)
			{
				return null;
			}
		}

		/// <exception cref="Mp3net.Helpers.CharacterCodingException"></exception>
		internal static byte[] CharBufferToBytes(string charBuffer, string characterSet)
		{
			Encoding charset = GetEncoding(characterSet);
            byte[] byteBuffer = charset.GetBytes(charBuffer);
            return BufferTools.CopyBuffer(byteBuffer, 0, byteBuffer.Length);
		}

		static UTF8Encoding UTF8Encoder = new UTF8Encoding (false, true);
		public static Encoding GetEncoding (string name)
		{
			try {
				Encoding e = Encoding.GetEncoding (name.Replace ('_','-'));
				if (e is UTF8Encoding)
					return UTF8Encoder;
				return e;
			} catch (ArgumentException) {
				throw new UnsupportedCharsetException (name);
			}
		}

        // detect value's charset using UDE (mozilla universal charset detector)
        private string DetectCharset()
        {
            CharsetDetector detector = new CharsetDetector();
            byte[] bytes = value;
            detector.Feed(value, 1, value.Length - 1);
            detector.DataEnd();

            string charsetName = detector.Charset;
            if (String.IsNullOrEmpty(charsetName))
                charsetName = "ISO-8859-1";
            return charsetName;
        }		
	}
}
