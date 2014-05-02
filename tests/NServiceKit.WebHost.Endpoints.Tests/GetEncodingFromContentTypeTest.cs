using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.WebHost.Endpoints.Tests.TestExistingDir
{
    /// <summary>A get encoding from content type test.</summary>
    [TestFixture]
    public class GetEncodingFromContentTypeTest
    {

        /// <summary>Can get correct encoding.</summary>
        [Test]
        public void Can_Get_Correct_Encoding()
        {
            var ct = "Content-Type: text/plain; charset=KOI8-R";

            var encoding = HttpListenerRequestWrapper.GetEncoding(ct);

            Assert.AreEqual("koi8-r", encoding.BodyName);

        }

        /// <summary>Returns null when no encoding.</summary>
        [Test]
        public void Return_Null_When_No_Encoding()
        {
            var ct = "Content-Type: text/plain";

            var encoding = HttpListenerRequestWrapper.GetEncoding(ct);

            Assert.IsNull(encoding);

        }

        /// <summary>Returns null when wrong encoding.</summary>
        [Test]
        public void Return_Null_When_Wrong_Encoding()
        {
            var ct = "Content-Type: text/plain; charset=ASDFG";

            var encoding = HttpListenerRequestWrapper.GetEncoding(ct);

            Assert.IsNull(encoding);

        }


    }
}
