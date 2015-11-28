using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MTProto.TL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MTProto_Tests.TL
{
    [TestClass]
    public class TLIntTests
    {

        [TestMethod]
        public void TLIntSerialization()
        {
            CollectionAssert.AreEqual(BitConverter.GetBytes(4), new TLInt(4).ToBytes());
            CollectionAssert.AreEqual(BitConverter.GetBytes(544), new TLInt(544).ToBytes());

            using (var stream = new MemoryStream())
            {
                var expected = new List<byte[]>
                {
                    BitConverter.GetBytes(4),
                    BitConverter.GetBytes(544)
                }.SelectMany(x => x).ToArray();

                new TLInt(4).ToStream(stream);
                new TLInt(544).ToStream(stream);

                var actual = new byte[expected.Length];
                stream.Position = 0;
                stream.Read(actual, 0, actual.Length);
                CollectionAssert.AreEquivalent(expected, actual);
            }
        }

        [TestMethod]
        public void TLIntHydration()
        {
            var buffer4 = BitConverter.GetBytes(4);
            var buffer544 = BitConverter.GetBytes(544);

            int pos = 0;
            var int4 = new TLInt(buffer4, ref pos);
            pos = 0;
            var int544 = new TLInt(buffer544, ref pos);

            Assert.AreEqual(4, int4.Value);
            Assert.AreEqual(544, int544.Value);

            using (var stream = new MemoryStream())
            {
                stream.Write(buffer4, 0, 4);
                stream.Write(buffer544, 0, 4);
                stream.Position = 0;

                pos = 0;
                var streamInt4 = new TLInt(stream, ref pos);
                var streamInt544 = new TLInt(stream, ref pos);

                Assert.AreEqual(4, streamInt4.Value);
                Assert.AreEqual(544, streamInt544.Value);
            }
        }

    }
}
