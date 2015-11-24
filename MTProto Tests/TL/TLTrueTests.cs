using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MTProto.TL;
using System;
using System.IO;

namespace MTProto_Tests.TL
{
    [TestClass]
    public class TLTrueTests
    {

        [TestMethod]
        public void TLTrueSerialization()
        {
            var tltrue = new TLTrue();
            var expectedBuffer = BitConverter.GetBytes(0x3fedd339);
            CollectionAssert.AreEquivalent(expectedBuffer, tltrue.ToBytes());

            using (var stream = new MemoryStream())
            {
                tltrue.ToStream(stream);
                stream.Position = 0;

                var actualBuffer = new byte[4];
                stream.Read(actualBuffer, 0, 4);
                CollectionAssert.AreEquivalent(expectedBuffer, actualBuffer);
            }
        }

        [TestMethod]
        public void TLTrueHydration()
        {
            var pos = 0;
            var buffer = BitConverter.GetBytes(0x3fedd339);
            var tltrue = new TLTrue(buffer, ref pos);
            Assert.AreEqual(true, tltrue.Value); // Always true, we rely on exceptions raised

            using (var stream = new MemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0;

                pos = 0;
                tltrue = new TLTrue(stream, ref pos);
                Assert.AreEqual(true, tltrue.Value); // Always true, we rely on exceptions raised
            }

        }

    }
}
