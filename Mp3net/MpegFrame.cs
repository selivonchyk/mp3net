namespace Mp3net
{
	public class MpegFrame
	{
		public static readonly string MPEG_VERSION_1_0 = "1.0";

		public static readonly string MPEG_VERSION_2_0 = "2.0";

		public static readonly string MPEG_VERSION_2_5 = "2.5";

		public static readonly string MPEG_LAYER_1 = "I";

		public static readonly string MPEG_LAYER_2 = "II";

		public static readonly string MPEG_LAYER_3 = "III";

		public static readonly string[] MPEG_LAYERS = new string[] { null, MPEG_LAYER_1, 
			MPEG_LAYER_2, MPEG_LAYER_3 };

		public static readonly string CHANNEL_MODE_MONO = "Mono";

		public static readonly string CHANNEL_MODE_DUAL_MONO = "Dual mono";

		public static readonly string CHANNEL_MODE_JOINT_STEREO = "Joint stereo";

		public static readonly string CHANNEL_MODE_STEREO = "Stereo";

		public static readonly string MODE_EXTENSION_BANDS_4_31 = "Bands 4-31";

		public static readonly string MODE_EXTENSION_BANDS_8_31 = "Bands 8-31";

		public static readonly string MODE_EXTENSION_BANDS_12_31 = "Bands 12-31";

		public static readonly string MODE_EXTENSION_BANDS_16_31 = "Bands 16-31";

		public static readonly string MODE_EXTENSION_NONE = "None";

		public static readonly string MODE_EXTENSION_INTENSITY_STEREO = "Intensity stereo";

		public static readonly string MODE_EXTENSION_M_S_STEREO = "M/S stereo";

		public static readonly string MODE_EXTENSION_INTENSITY_M_S_STEREO = "Intensity & M/S stereo";

		public static readonly string MODE_EXTENSION_NA = "n/a";

		public static readonly string EMPHASIS_NONE = "None";

		public static readonly string EMPHASIS__50_15_MS = "50/15 ms";

		public static readonly string EMPHASIS_CCITT_J_17 = "CCITT J.17";

		private const int FRAME_DATA_LENGTH = 4;

		private const int FRAME_SYNC = unchecked((int)(0x7FF));

		private const long BITMASK_FRAME_SYNC = unchecked((long)(0xFFE00000L));

		private const long BITMASK_VERSION = unchecked((long)(0x180000L));

		private const long BITMASK_LAYER = unchecked((long)(0x60000L));

		private const long BITMASK_PROTECTION = unchecked((long)(0x10000L));

		private const long BITMASK_BITRATE = unchecked((long)(0xF000L));

		private const long BITMASK_SAMPLE_RATE = unchecked((long)(0xC00L));

		private const long BITMASK_PADDING = unchecked((long)(0x200L));

		private const long BITMASK_PRIVATE = unchecked((long)(0x100L));

		private const long BITMASK_CHANNEL_MODE = unchecked((long)(0xC0L));

		private const long BITMASK_MODE_EXTENSION = unchecked((long)(0x30L));

		private const long BITMASK_COPYRIGHT = unchecked((long)(0x8L));

		private const long BITMASK_ORIGINAL = unchecked((long)(0x4L));

		private const long BITMASK_EMPHASIS = unchecked((long)(0x3L));

		private string version;

		private int layer;

		private bool protection;

		private int bitrate;

		private int sampleRate;

		private bool padding;

		private bool privat;

		private string channelMode;

		private string modeExtension;

		private bool copyright;

		private bool original;

		private string emphasis;

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public MpegFrame(byte[] frameData)
		{
			if (frameData.Length < FRAME_DATA_LENGTH)
			{
				throw new InvalidDataException("Mpeg frame too short");
			}
			long frameHeader = BufferTools.UnpackInteger(frameData[0], frameData[1], frameData
				[2], frameData[3]);
			SetFields(frameHeader);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		public MpegFrame(byte frameData1, byte frameData2, byte frameData3, byte frameData4)
		{
			long frameHeader = BufferTools.UnpackInteger(frameData1, frameData2, frameData3, frameData4);
            System.Console.WriteLine("FrameHeader: {0}", (uint)frameHeader);
			SetFields(frameHeader);
		}

		public MpegFrame()
		{
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SetFields(long frameHeader)
		{
			long frameSync = ExtractField(frameHeader, BITMASK_FRAME_SYNC);
			if (frameSync != FRAME_SYNC)
			{
				throw new InvalidDataException("Frame sync missing");
			}
			SetVersion(ExtractField(frameHeader, BITMASK_VERSION));
			SetLayer(ExtractField(frameHeader, BITMASK_LAYER));
			SetProtection(ExtractField(frameHeader, BITMASK_PROTECTION));
			SetBitRate(ExtractField(frameHeader, BITMASK_BITRATE));
			SetSampleRate(ExtractField(frameHeader, BITMASK_SAMPLE_RATE));
			SetPadding(ExtractField(frameHeader, BITMASK_PADDING));
			SetPrivate(ExtractField(frameHeader, BITMASK_PRIVATE));
			SetChannelMode(ExtractField(frameHeader, BITMASK_CHANNEL_MODE));
			SetModeExtension(ExtractField(frameHeader, BITMASK_MODE_EXTENSION));
			SetCopyright(ExtractField(frameHeader, BITMASK_COPYRIGHT));
			SetOriginal(ExtractField(frameHeader, BITMASK_ORIGINAL));
			SetEmphasis(ExtractField(frameHeader, BITMASK_EMPHASIS));
		}

		protected internal virtual int ExtractField(long frameHeader, long bitMask)
		{
			int shiftBy = 0;
			for (int i = 0; i <= 31; i++)
			{
				if (((bitMask >> i) & 1) != 0)
				{
					shiftBy = i;
					break;
				}
			}
			return (int)((frameHeader >> shiftBy) & (bitMask >> shiftBy));
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SetVersion(int version)
		{
            System.Console.WriteLine("Version: {0}", version);
			switch (version)
			{
				case 0:
				{
					this.version = MPEG_VERSION_2_5;
					break;
				}

				case 2:
				{
					this.version = MPEG_VERSION_2_0;
					break;
				}

				case 3:
				{
					this.version = MPEG_VERSION_1_0;
					break;
				}

				default:
				{
					throw new InvalidDataException("Invalid mpeg audio version in frame header");
				}
			}
            System.Console.WriteLine("Version: {0}", this.version);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SetLayer(int layer)
		{
            System.Console.WriteLine("Layer: {0}", layer);
			switch (layer)
			{
				case 1:
				{
					this.layer = 3;
					break;
				}

				case 2:
				{
					this.layer = 2;
					break;
				}

				case 3:
				{
					this.layer = 1;
					break;
				}

				default:
				{
					throw new InvalidDataException("Invalid mpeg layer description in frame header");
				}
			}
		}

		private void SetProtection(int protectionBit)
		{
			this.protection = (protectionBit == 1);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SetBitRate(int bitrate)
		{
            System.Console.WriteLine("BiRate: {0}, version: {1}", bitrate, version);
			if (MPEG_VERSION_1_0.Equals(version))
			{
				if (layer == 1)
				{
					switch (bitrate)
					{
						case 1:
						{
							this.bitrate = 32;
							return;
						}

						case 2:
						{
							this.bitrate = 64;
							return;
						}

						case 3:
						{
							this.bitrate = 96;
							return;
						}

						case 4:
						{
							this.bitrate = 128;
							return;
						}

						case 5:
						{
							this.bitrate = 160;
							return;
						}

						case 6:
						{
							this.bitrate = 192;
							return;
						}

						case 7:
						{
							this.bitrate = 224;
							return;
						}

						case 8:
						{
							this.bitrate = 256;
							return;
						}

						case 9:
						{
							this.bitrate = 288;
							return;
						}

						case 10:
						{
							this.bitrate = 320;
							return;
						}

						case 11:
						{
							this.bitrate = 352;
							return;
						}

						case 12:
						{
							this.bitrate = 384;
							return;
						}

						case 13:
						{
							this.bitrate = 416;
							return;
						}

						case 14:
						{
							this.bitrate = 448;
							return;
						}
					}
				}
				else
				{
					if (layer == 2)
					{
						switch (bitrate)
						{
							case 1:
							{
								this.bitrate = 32;
								return;
							}

							case 2:
							{
								this.bitrate = 48;
								return;
							}

							case 3:
							{
								this.bitrate = 56;
								return;
							}

							case 4:
							{
								this.bitrate = 64;
								return;
							}

							case 5:
							{
								this.bitrate = 80;
								return;
							}

							case 6:
							{
								this.bitrate = 96;
								return;
							}

							case 7:
							{
								this.bitrate = 112;
								return;
							}

							case 8:
							{
								this.bitrate = 128;
								return;
							}

							case 9:
							{
								this.bitrate = 160;
								return;
							}

							case 10:
							{
								this.bitrate = 192;
								return;
							}

							case 11:
							{
								this.bitrate = 224;
								return;
							}

							case 12:
							{
								this.bitrate = 256;
								return;
							}

							case 13:
							{
								this.bitrate = 320;
								return;
							}

							case 14:
							{
								this.bitrate = 384;
								return;
							}
						}
					}
					else
					{
						if (layer == 3)
						{
							switch (bitrate)
							{
								case 1:
								{
									this.bitrate = 32;
									return;
								}

								case 2:
								{
									this.bitrate = 40;
									return;
								}

								case 3:
								{
									this.bitrate = 48;
									return;
								}

								case 4:
								{
									this.bitrate = 56;
									return;
								}

								case 5:
								{
									this.bitrate = 64;
									return;
								}

								case 6:
								{
									this.bitrate = 80;
									return;
								}

								case 7:
								{
									this.bitrate = 96;
									return;
								}

								case 8:
								{
									this.bitrate = 112;
									return;
								}

								case 9:
								{
									this.bitrate = 128;
									return;
								}

								case 10:
								{
									this.bitrate = 160;
									return;
								}

								case 11:
								{
									this.bitrate = 192;
									return;
								}

								case 12:
								{
									this.bitrate = 224;
									return;
								}

								case 13:
								{
									this.bitrate = 256;
									return;
								}

								case 14:
								{
									this.bitrate = 320;
									return;
								}
							}
						}
					}
				}
			}
			else
			{
				if (MPEG_VERSION_2_0.Equals(version) || MPEG_VERSION_2_5.Equals(version))
				{
					if (layer == 1)
					{
						switch (bitrate)
						{
							case 1:
							{
								this.bitrate = 32;
								return;
							}

							case 2:
							{
								this.bitrate = 48;
								return;
							}

							case 3:
							{
								this.bitrate = 56;
								return;
							}

							case 4:
							{
								this.bitrate = 64;
								return;
							}

							case 5:
							{
								this.bitrate = 80;
								return;
							}

							case 6:
							{
								this.bitrate = 96;
								return;
							}

							case 7:
							{
								this.bitrate = 112;
								return;
							}

							case 8:
							{
								this.bitrate = 128;
								return;
							}

							case 9:
							{
								this.bitrate = 144;
								return;
							}

							case 10:
							{
								this.bitrate = 160;
								return;
							}

							case 11:
							{
								this.bitrate = 176;
								return;
							}

							case 12:
							{
								this.bitrate = 192;
								return;
							}

							case 13:
							{
								this.bitrate = 224;
								return;
							}

							case 14:
							{
								this.bitrate = 256;
								return;
							}
						}
					}
					else
					{
						if (layer == 2 || layer == 3)
						{
							switch (bitrate)
							{
								case 1:
								{
									this.bitrate = 8;
									return;
								}

								case 2:
								{
									this.bitrate = 16;
									return;
								}

								case 3:
								{
									this.bitrate = 24;
									return;
								}

								case 4:
								{
									this.bitrate = 32;
									return;
								}

								case 5:
								{
									this.bitrate = 40;
									return;
								}

								case 6:
								{
									this.bitrate = 48;
									return;
								}

								case 7:
								{
									this.bitrate = 56;
									return;
								}

								case 8:
								{
									this.bitrate = 64;
									return;
								}

								case 9:
								{
									this.bitrate = 80;
									return;
								}

								case 10:
								{
									this.bitrate = 96;
									return;
								}

								case 11:
								{
									this.bitrate = 112;
									return;
								}

								case 12:
								{
									this.bitrate = 128;
									return;
								}

								case 13:
								{
									this.bitrate = 144;
									return;
								}

								case 14:
								{
									this.bitrate = 160;
									return;
								}
							}
						}
					}
				}
			}
			throw new InvalidDataException("Invalid bitrate in frame header");
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SetSampleRate(int sampleRate)
		{
            System.Console.WriteLine("SampleRate: {0}, version: {1}", sampleRate, version);
			if (MPEG_VERSION_1_0.Equals(version))
			{
				switch (sampleRate)
				{
					case 0:
					{
						this.sampleRate = 44100;
						return;
					}

					case 1:
					{
						this.sampleRate = 48000;
						return;
					}

					case 2:
					{
						this.sampleRate = 32000;
						return;
					}
				}
			}
			else
			{
				if (MPEG_VERSION_2_0.Equals(version))
				{
					switch (sampleRate)
					{
						case 0:
						{
							this.sampleRate = 22050;
							return;
						}

						case 1:
						{
							this.sampleRate = 24000;
							return;
						}

						case 2:
						{
							this.sampleRate = 16000;
							return;
						}
					}
				}
				else
				{
					if (MPEG_VERSION_2_5.Equals(version))
					{
						switch (sampleRate)
						{
							case 0:
							{
								this.sampleRate = 11025;
								return;
							}

							case 1:
							{
								this.sampleRate = 12000;
								return;
							}

							case 2:
							{
								this.sampleRate = 8000;
								return;
							}
						}
					}
				}
			}
			throw new InvalidDataException("Invalid sample rate in frame header");
		}

		private void SetPadding(int paddingBit)
		{
			this.padding = (paddingBit == 1);
		}

		private void SetPrivate(int privateBit)
		{
			this.privat = (privateBit == 1);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SetChannelMode(int channelMode)
		{
			switch (channelMode)
			{
				case 0:
				{
					this.channelMode = CHANNEL_MODE_STEREO;
					break;
				}

				case 1:
				{
					this.channelMode = CHANNEL_MODE_JOINT_STEREO;
					break;
				}

				case 2:
				{
					this.channelMode = CHANNEL_MODE_DUAL_MONO;
					break;
				}

				case 3:
				{
					this.channelMode = CHANNEL_MODE_MONO;
					break;
				}

				default:
				{
					throw new InvalidDataException("Invalid channel mode in frame header");
				}
			}
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SetModeExtension(int modeExtension)
		{
			if (!CHANNEL_MODE_JOINT_STEREO.Equals(channelMode))
			{
				this.modeExtension = MODE_EXTENSION_NA;
			}
			else
			{
				if (layer == 1 || layer == 2)
				{
					switch (modeExtension)
					{
						case 0:
						{
							this.modeExtension = MODE_EXTENSION_BANDS_4_31;
							return;
						}

						case 1:
						{
							this.modeExtension = MODE_EXTENSION_BANDS_8_31;
							return;
						}

						case 2:
						{
							this.modeExtension = MODE_EXTENSION_BANDS_12_31;
							return;
						}

						case 3:
						{
							this.modeExtension = MODE_EXTENSION_BANDS_16_31;
							return;
						}
					}
				}
				else
				{
					if (layer == 3)
					{
						switch (modeExtension)
						{
							case 0:
							{
								this.modeExtension = MODE_EXTENSION_NONE;
								return;
							}

							case 1:
							{
								this.modeExtension = MODE_EXTENSION_INTENSITY_STEREO;
								return;
							}

							case 2:
							{
								this.modeExtension = MODE_EXTENSION_M_S_STEREO;
								return;
							}

							case 3:
							{
								this.modeExtension = MODE_EXTENSION_INTENSITY_M_S_STEREO;
								return;
							}
						}
					}
				}
				throw new InvalidDataException("Invalid mode extension in frame header");
			}
		}

		private void SetCopyright(int copyrightBit)
		{
			this.copyright = (copyrightBit == 1);
		}

		private void SetOriginal(int originalBit)
		{
			this.original = (originalBit == 1);
		}

		/// <exception cref="Mp3net.InvalidDataException"></exception>
		private void SetEmphasis(int emphasis)
		{
			switch (emphasis)
			{
				case 0:
				{
					this.emphasis = EMPHASIS_NONE;
					break;
				}

				case 1:
				{
					this.emphasis = EMPHASIS__50_15_MS;
					break;
				}

				case 3:
				{
					this.emphasis = EMPHASIS_CCITT_J_17;
					break;
				}

				default:
				{
					throw new InvalidDataException("Invalid emphasis in frame header");
				}
			}
		}

		public virtual int GetBitrate()
		{
			return bitrate;
		}

		public virtual string GetChannelMode()
		{
			return channelMode;
		}

		public virtual bool IsCopyright()
		{
			return copyright;
		}

		public virtual string GetEmphasis()
		{
			return emphasis;
		}

		public virtual string GetLayer()
		{
			return MPEG_LAYERS[layer];
		}

		public virtual string GetModeExtension()
		{
			return modeExtension;
		}

		public virtual bool IsOriginal()
		{
			return original;
		}

		public virtual bool HasPadding()
		{
			return padding;
		}

		public virtual bool IsPrivate()
		{
			return privat;
		}

		public virtual bool IsProtection()
		{
			return protection;
		}

		public virtual int GetSampleRate()
		{
			return sampleRate;
		}

		public virtual string GetVersion()
		{
			return version;
		}

		public virtual int GetLengthInBytes()
		{
			long length;
			int pad;
			if (padding)
			{
				pad = 1;
			}
			else
			{
				pad = 0;
			}
			if (layer == 1)
			{
				length = ((48000 * bitrate) / sampleRate) + (pad * 4);
			}
			else
			{
				length = ((144000 * bitrate) / sampleRate) + pad;
			}
			return (int)length;
		}
	}
}
