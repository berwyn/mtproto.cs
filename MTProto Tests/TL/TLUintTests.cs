﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MTProto.TL;
using System;
using System.IO;

namespace MTProto_Tests.TL
{
    [TestClass]
    public class TLUintTests
    {
        [TestMethod]
        public void TLUintSerialization()
        {
            var test1 = new TLUint(25565);
            var buffer1 = test1.ToBytes();
            var test2 = new TLUint(99999);
            var buffer2 = test2.ToBytes();

            Assert.AreEqual(4, BitConverter.ToInt32(buffer1, 0));
            Assert.AreEqual(544, BitConverter.ToInt32(buffer2, 0));

            using (var stream = new MemoryStream())
            {
                test1.ToStream(stream);
                test2.ToStream(stream);

                var streamBuffer1 = new byte[4];
                var streamBuffer2 = new byte[4];
                stream.Position = 0;
                stream.Read(streamBuffer1, 0, 4);
                stream.Read(streamBuffer2, 0, 4);

                Assert.AreEqual(4, BitConverter.ToInt32(streamBuffer1, 0));
                Assert.AreEqual(544, BitConverter.ToInt32(streamBuffer2, 0));
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

            Assert.AreEqual(4, int4.Value);
            Assert.AreEqual(544, int544.Value);

            using (var stream = new MemoryStream())
            {
                stream.Write(buffer4, 0, 4);
                stream.Write(buffer544, 0, 4);
                stream.Position = 0;

                pos = 0;
                var streamInt4 = new TLUint(stream, ref pos);
                var streamInt544 = new TLUint(stream, ref pos);

                Assert.AreEqual(4, streamInt4.Value);
                Assert.AreEqual(544, streamInt544.Value);
            }
        }
    }
}
