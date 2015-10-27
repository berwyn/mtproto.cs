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
    public class TLVectorTests
    {

        private static int[] testedInts;
        private static byte[] bufferInt;

        [ClassInitialize]
        public static void Setup(TestContext ctxt)
        {
            testedInts = new int[] { 244, 122, 5565, 2, 3 };
            bufferInt = new List<byte>()
                .Concat(BitConverter.GetBytes(0x1cb5c415))
                .Concat(BitConverter.GetBytes(new TLInt(testedInts.Length).Value))
                .Concat(testedInts.Select(i => BitConverter.GetBytes(i)).SelectMany(a => a))
                .ToArray();
        }

        [TestMethod]
        public void TLVectorHydration()
        {
            var pos = 0;
            var intvec = new TLVector<TLInt>(bufferInt, ref pos);
            Assert.AreEqual(testedInts.Length, intvec.Count);
            CollectionAssert.AreEquivalent(testedInts, intvec.Value.Select(i => i.Value).ToArray());

            using (var stream = new MemoryStream(bufferInt))
            {
                pos = 0;
                intvec = new TLVector<TLInt>(stream, ref pos);
                Assert.AreEqual(testedInts.Length, intvec.Count);
                CollectionAssert.AreEquivalent(testedInts, intvec.Value.Select(i => i.Value).ToArray());
            }
        }

        [TestMethod]
        public void TLVectorSerialization()
        {
            var intvec = new TLVector<TLInt>(testedInts.Length);
            foreach(var i in testedInts)
            {
                intvec.Add(new TLInt(i));
            }

            var buffer = intvec.ToBytes();
            CollectionAssert.AreEqual(bufferInt, buffer);

            using (var stream = new MemoryStream())
            {
                intvec.ToStream(stream);
                stream.Read(buffer, 0, 28); // Observed through debugging
                CollectionAssert.AreEqual(bufferInt, buffer);
            }
        }

    }
}
