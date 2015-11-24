using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MTProto.TL;
using System;
using System.IO;

namespace MTProto_Tests.TL
{
    [TestClass]
    public class TLLongTests
    {

        [TestMethod]
        public void TLLongHydration()
        {
            var buffer1 = BitConverter.GetBytes(25565L);
            var buffer2 = BitConverter.GetBytes(99999L);

            var pos = 0;
            Assert.AreEqual(25565L, new TLLong(buffer1, ref pos).Value);
            pos = 0;
            Assert.AreEqual(99999L, new TLLong(buffer2, ref pos).Value);

            pos = 0;
            using (var stream = new MemoryStream())
            {
                stream.Write(buffer1, 0, buffer1.Length);
                stream.Write(buffer2, 0, buffer2.Length);
                stream.Position = 0;

                Assert.AreEqual(25565L, new TLLong(stream, ref pos).Value);
                Assert.AreEqual(99999L, new TLLong(stream, ref pos).Value);
            }
        }

        public void TLLongSerialization()
        {
            var buffer1 = BitConverter.GetBytes(25565L);
            var buffer2 = BitConverter.GetBytes(99999L);

            var pos = 0;
            CollectionAssert.AreEquivalent(buffer1, new TLLong(buffer1, ref pos).ToBytes());
            pos = 0;
            CollectionAssert.AreEquivalent(buffer2, new TLLong(buffer2, ref pos).ToBytes());

            using (var stream = new MemoryStream())
            {
                new TLLong(25565L).ToStream(stream);
                new TLLong(99999L).ToStream(stream);

                stream.Position = 0;
                var actualBuffer1 = new byte[8];
                var actualBuffer2 = new byte[8];
                stream.Read(actualBuffer1, 0, 8);
                stream.Read(actualBuffer2, 0, 8);

                CollectionAssert.AreEquivalent(buffer1, actualBuffer1);
                CollectionAssert.AreEquivalent(buffer2, actualBuffer2);
            }
        }
    }
}
