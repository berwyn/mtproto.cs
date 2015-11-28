using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MTProto.TL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MTProto_Tests.TL
{
    [TestClass]
    public class TLUintTests
    {
        [TestMethod]
        public void TLUintSerialization()
        {
            CollectionAssert.AreEqual(BitConverter.GetBytes(25565U), new TLUint(25565U).ToBytes());
            CollectionAssert.AreEqual(BitConverter.GetBytes(99999U), new TLUint(99999U).ToBytes());

            using (var stream = new MemoryStream())
            {
                new TLUint(25565U).ToStream(stream);
                new TLUint(99999U).ToStream(stream);

                var buffer = new byte[8];
                stream.Position = 0;
                stream.Read(buffer, 0, 4);
                stream.Read(buffer, 4, 4);

                var expected = new List<byte[]>
                {
                    BitConverter.GetBytes(25565U),
                    BitConverter.GetBytes(99999U)
                }.SelectMany(x => x).ToArray();
                CollectionAssert.AreEquivalent(expected, buffer);
            }
        }

        [TestMethod]
        public void TLUintHydration()
        {
            var buffer4 = BitConverter.GetBytes(4);
            var buffer544 = BitConverter.GetBytes(544);

            int pos = 0;
            var int4 = new TLUint(buffer4, ref pos);
            pos = 0;
            var int544 = new TLUint(buffer544, ref pos);

            Assert.AreEqual(4U, int4.Value);
            Assert.AreEqual(544U, int544.Value);

            using (var stream = new MemoryStream())
            {
                stream.Write(buffer4, 0, 4);
                stream.Write(buffer544, 0, 4);
                stream.Position = 0;

                pos = 0;
                var streamInt4 = new TLUint(stream, ref pos);
                var streamInt544 = new TLUint(stream, ref pos);

                Assert.AreEqual(4U, streamInt4.Value);
                Assert.AreEqual(544U, streamInt544.Value);
            }
        }
    }
}
