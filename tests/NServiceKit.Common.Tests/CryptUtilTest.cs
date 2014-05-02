using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.Common;
using NUnit.Framework;

namespace NServiceKit.Common.Tests
{
    /// <summary>A crypt utilities test.</summary>
    [TestFixture]
    public class CryptUtilsTest
    {
        /// <summary>Can encrypt with string extension.</summary>
        [TestCase]
        public void CanEncryptWithStringExtension()
        {
            CryptUtils.Length = RsaKeyLengths.Bit1024;
            CryptUtils.KeyPair = CryptUtils.CreatePublicAndPrivateKeyPair();

            string TestStart = "Mr. Watson--come here--I want to see you.";
            string Encrypted;
            string Decrypted;

            Encrypted = TestStart.Encrypt();
            Assert.AreNotEqual(Encrypted, TestStart);

            Decrypted = Encrypted.Decrypt();
            Assert.AreEqual(Decrypted, TestStart);

        }

        /// <summary>Can encrypt with string extension fails without key pair.</summary>
        [TestCase]
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CanEncryptWithStringExtensionFailsWithoutKeyPair()
        {
            CryptUtils.Length = RsaKeyLengths.Bit1024;
            CryptUtils.KeyPair = null;
            string TestStart = "Mr. Watson--come here--I want to see you.";
            string Encrypted;

            Encrypted = TestStart.Encrypt();
                      

        }
    }
}