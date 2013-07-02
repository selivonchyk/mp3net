using System;
using NUnit.Framework;

namespace Mp3net
{
    [TestFixture]
	public class BaseExceptionTest
	{
		[TestCase]
		public virtual void TestShouldGenerateCorrectDetailedMessageForSingleException()
		{
			BaseException e = new BaseException("ONE");
			Assert.AreEqual("ONE", e.Message);
            Assert.AreEqual("[Mp3net.BaseException: ONE]", e.GetDetailedMessage());
		}

        [TestCase]
		public virtual void TestShouldGenerateCorrectDetailedMessageForChainedBaseExceptions()
		{
			BaseException e1 = new BaseException("ONE");
			BaseException e2 = new UnsupportedTagException("TWO", e1);
			BaseException e3 = new NotSupportedException("THREE", e2);
			BaseException e4 = new NoSuchTagException("FOUR", e3);
			BaseException e5 = new InvalidDataException("FIVE", e4);
			Assert.AreEqual("FIVE", e5.Message);
            Assert.AreEqual("[Mp3net.InvalidDataException: FIVE] caused by [Mp3net.NoSuchTagException: FOUR] caused by [Mp3net.NotSupportedException: THREE] caused by [Mp3net.UnsupportedTagException: TWO] caused by [Mp3net.BaseException: ONE]", e5.GetDetailedMessage());
		}

        [TestCase]
		public virtual void TestShouldGenerateCorrectDetailedMessageForChainedExceptionsWithOtherExceptionInMix()
		{
			BaseException e1 = new BaseException("ONE");
			BaseException e2 = new UnsupportedTagException("TWO", e1);
			Exception e3 = new Exception("THREE", e2);
			BaseException e4 = new NoSuchTagException("FOUR", e3);
			BaseException e5 = new InvalidDataException("FIVE", e4);
			Assert.AreEqual("FIVE", e5.Message);
            Assert.AreEqual("[Mp3net.InvalidDataException: FIVE] caused by [Mp3net.NoSuchTagException: FOUR] caused by [System.Exception: THREE] caused by [Mp3net.UnsupportedTagException: TWO] caused by [Mp3net.BaseException: ONE]", e5.GetDetailedMessage());
		}
	}
}
