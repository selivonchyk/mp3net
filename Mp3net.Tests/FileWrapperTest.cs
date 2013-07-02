using System;
using System.IO;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class FileWrapperTest
	{
		private static readonly string VALID_FILENAME = "Resources/notags.mp3";

		private const long VALID_FILE_LENGTH = 2869;

		private static readonly string NON_EXISTANT_FILENAME = "just-not.there";

		private static readonly string MALFORMED_FILENAME = "malformed.?";

        [TestCase]
		public virtual void TestShouldReadValidFile()
		{
			FileWrapper fileWrapper = new FileWrapper(VALID_FILENAME);
			Assert.AreEqual(fileWrapper.GetFilename(), VALID_FILENAME);
			Assert.IsTrue(fileWrapper.GetLastModified() > 0);
			Assert.AreEqual(fileWrapper.GetLength(), VALID_FILE_LENGTH);
		}

        [TestCase]
		public virtual void TestShouldFailForNonExistentFile()
		{
			try
			{
				new FileWrapper(NON_EXISTANT_FILENAME);
				Assert.Fail("FileNotFoundException expected but not thrown");
			}
			catch (FileNotFoundException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldFailForMalformedFilename()
		{
			try
			{
				new FileWrapper(MALFORMED_FILENAME);
				Assert.Fail("FileNotFoundException expected but not thrown");
			}
			catch (FileNotFoundException)
			{
			}
		}

        [TestCase]
		public virtual void TestShouldFailForNullFilename()
		{
			try
			{
				new FileWrapper(null);
				Assert.Fail("NullPointerException expected but not thrown");
			}
			catch (ArgumentNullException)
			{
			}
		}
	}
}
