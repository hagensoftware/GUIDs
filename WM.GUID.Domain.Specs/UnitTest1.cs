using NUnit.Framework;
using System;
using WM.GUID.Domain;

namespace UnitTests
{
    public class UnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EmptyOrNullNameCausesException()
        {
            string myGuid = null;
            long myExpire = 123;
            string myName = "";

            Assert.Throws<ArgumentNullException>(() => new GuidMetadata( myGuid, myExpire, myName));
        }

        [Test]
        public void GuidIsNullCreatesRandomGuid()
        {
            string myGuid = null; 
            long myExpire = 123;
            string myName = "John Doe";

            GuidMetadata metadata = new GuidMetadata(myGuid, myExpire, myName);

            Assert.IsNotNull(Guid.Parse(metadata.Id.ToString()));
        }

        [Test]
        public void BadGuidFormatThrowsException()
        {
            string myGuid = "1234567890123%^&*890123456789012"; 
            long myExpire = 123;
            string myName = "John Doe";

            Assert.Throws<FormatException>(() => new GuidMetadata( myGuid, myExpire, myName));
        }

        [Test]
        public void IncorrectGuidLengthThrowsException()
        {
            string myGuid = "1234567890ABCDEF1234567890ABCDE"; 
            long myExpire = 123;
            string myName = "John Doe";

            Assert.Throws<FormatException>(() => new GuidMetadata(myGuid, myExpire, myName));
        }

        [Test]
        public void LowerCaseGuidIsValid()
        {
            string myGuid = "1234567890abcdef1234567890abcdef"; 
            long myExpire = 123;
            string myName = "John Doe";

            GuidMetadata metadata = new GuidMetadata( myGuid, myExpire, myName);

            Assert.AreEqual(metadata.Id.ToString(), metadata.Id.ToString().ToUpper());
        }

        [Test]
        public void GuidWithHyphensIsValid()
        {
            string myGuid = "12345678-90ab-cdef-1234-567890abcdef"; 
            long myExpire = 123;
            string myName = "John Doe";

            GuidMetadata metadata = new GuidMetadata(myGuid, myExpire, myName);

            Assert.AreEqual(metadata.Id.ToString(), new Guid(metadata.Id).ToString("N").ToUpper());
        }

        [Test]
        public void ExpirationTimeProvidedIsApplied()
        {
            string myGuid = null;
            long? myExpire = 1547078400;
            string myName = "John Doe";

            DateTime unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime myDateTime = new DateTime(2019, 1, 1, 0, 0, 0);

            GuidMetadata metadata = new GuidMetadata(myGuid, myExpire, myName);

            Assert.AreEqual(unixStartTime.AddSeconds((double)metadata.Expire), new DateTime(2019, 1, 10, 0, 0, 0));
        }

    }
}