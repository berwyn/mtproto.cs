using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MTProto.TL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTProto_Tests.TL
{
    [TestClass]
    public class TLNullTests
    {

        [TestMethod]
        public void TLNullSerializtion()
        {
            var expectedBuffer = BitConverter.GetBytes(0x56730bcc);
            var actualBuffer = new TLNull().ToBytes();
            CollectionAssert.AreEquivalent(expectedBuffer, actualBuffer);

            using (var stream = new MemoryStream())
            {
                new TLNull().ToStream(stream);
                stream.Position = 0;

                actualBuffer = new byte[4];
                stream.Read(actualBuffer, 0, 4);
                CollectionAssert.AreEquivalent(expectedBuffer, actualBuffer);
            }
        }

        [TestMethod]
        public void TLNullHydration()
        {
            var pos = 0;
            var buffer = BitConverter.GetBytes(0x56730bcc);
            var tlnull = new TLNull(buffer, ref pos);
            Assert.AreEqual(null, tlnull.Value);

            using (var stream = new MemoryStream())
            {
                stream.Write(buffer, 0, 4);
                stream.Position = 0;

                pos = 0;
                tlnull = new TLNull(stream, ref pos);
                Assert.AreEqual(null, tlnull.Value);
            }
        }

    }
}
