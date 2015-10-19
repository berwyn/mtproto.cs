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
    public class TLBoolTests
    {

        private static byte[] bufferTrue;
        private static byte[] bufferFalse;

        [ClassInitialize]
        public static void Setup(TestContext ctxt)
        {
            bufferTrue = BitConverter.GetBytes(0x997275b5);
            bufferFalse = BitConverter.GetBytes(0xbc799737);
        }

        [TestMethod]
        public void TLBoolSerialization()
        {
            var boolTrue = new TLBool();
            boolTrue.Value = true;
            CollectionAssert.AreEquivalent(bufferTrue, boolTrue.ToBytes());

            var boolFalse = new TLBool();
            boolFalse.Value = false;
            CollectionAssert.AreEquivalent(bufferFalse, boolFalse.ToBytes());

            using (var streamTrue = new MemoryStream())
            {
                boolTrue.ToStream(streamTrue);
                var buffer = new byte[4];
                streamTrue.Position = 0;
                streamTrue.Read(buffer, 0, 4);
                CollectionAssert.AreEquivalent(bufferTrue, buffer);
            }

            using (var streamFalse = new MemoryStream())
            {
                boolFalse.ToStream(streamFalse);
                var buffer = new byte[4];
                streamFalse.Position = 0;
                streamFalse.Read(buffer, 0, 4);
                CollectionAssert.AreEquivalent(bufferFalse, buffer);
            }
        }

        [TestMethod]
        public void TLBoolHydration()
        {
            int pos = 0;
            var boolTrue = new TLBool();
            boolTrue.FromBytes(bufferTrue, ref pos);
            Assert.AreEqual(true, boolTrue.Value);

            pos = 0;
            var boolFalse = new TLBool();
            boolFalse.FromBytes(bufferFalse, ref pos);
            Assert.AreEqual(false, boolFalse.Value);

            pos = 0;
            using (var streamTrue = new MemoryStream(bufferTrue))
            {
                var streamBoolTrue = new TLBool();
                streamBoolTrue.FromStream(streamTrue, ref pos);
                Assert.AreEqual(true, streamBoolTrue.Value);
            }

            pos = 0;
            using (var streamFalse = new MemoryStream(bufferFalse))
            {
                var streamBoolFalse = new TLBool();
                streamBoolFalse.FromStream(streamFalse, ref pos);
                Assert.AreEqual(false, streamBoolFalse.Value);
            }
        }
    }
}
