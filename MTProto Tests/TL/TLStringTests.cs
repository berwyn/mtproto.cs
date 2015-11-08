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
    public class TLStringTests
    {
        private readonly byte[] SHORT_BUFFER = { 0x57, 0x68, 0x79, 0x20, 0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x74, 0x68, 0x65, 0x72, 0x65, 0x2c, 0x20, 0x55, 0x54, 0x46, 0x2d, 0x38, 0x21 };
        private const string SHORT_TEXT = "Why hello there, UTF-8!";

        private const string LONG_TEXT = "Pony ipsum dolor sit amet prince Blueblood pegasai Granny Smith Featherweight pie, elements of harmony Photo Finish. Twist flank Ponyville, Opalescence Granny Smith pony Lyra Cheese Sandwich Babs Seed Flim unicorn Dr Hooves Pumpkin Cake Sunset Shimmer Adagio Dazzle. Cranky Doodle Donkey Fluttershy Derpy Bon Bon wing apple Lightning Dust Bloomberg Princess Celestia muffins. Pies Opalescence Trixie, Caramel Adagio Dazzle Cranky Doodle Donkey Ms. Harshwhinny moon Twilight Sparkle. Babs Seed Trixie Opalescence, clop Ms. Peachbottom Peewee magic Vinyl Scratch. Shadowbolts Bon Bon Cranky Doodle Donkey, alicorn cupcakes waterfall Flash Sentry tail Ms. Harshwhinny generosity Scootaloo Flim King Sombra Dr Hooves.";

        [TestMethod]
        public void TLStringSerialization()
        {
            var tlStr = new TLString(SHORT_TEXT);
            var padding = 4 - (SHORT_BUFFER.Length % 4);
            byte[] expectedBuffer = new byte[SHORT_BUFFER.Length + padding + 1];
            expectedBuffer[0] = (byte)SHORT_BUFFER.Length;
            Array.Copy(SHORT_BUFFER, 0, expectedBuffer, 1, SHORT_BUFFER.Length);

            var actualBuffer = tlStr.ToBytes();
            Assert.AreEqual(expectedBuffer.Length, actualBuffer.Length);
            CollectionAssert.AreEquivalent(expectedBuffer, actualBuffer);

            using (var stream = new MemoryStream())
            {
                tlStr.ToStream(stream);

                stream.Position = 0;
                actualBuffer = new byte[SHORT_BUFFER.Length + padding + 1];
                stream.Read(actualBuffer, 0, actualBuffer.Length);
                CollectionAssert.AreEquivalent(expectedBuffer, actualBuffer);
            }

            var tlLongStr = new TLString(LONG_TEXT);
            var longBuffer = Encoding.UTF8.GetBytes(LONG_TEXT);
            padding =  4 - (longBuffer.Length % 4);
            byte[] lengthBuffer = BitConverter.GetBytes(longBuffer.Length)
                .Take(3)
                .Reverse()
                .ToArray();

            expectedBuffer = new byte[longBuffer.Length + padding + 4];
            expectedBuffer[0] = 254;
            Array.Copy(lengthBuffer, 0, expectedBuffer, 1, 3);
            Array.Copy(longBuffer, 0, expectedBuffer, 4, longBuffer.Length);

            actualBuffer = tlLongStr.ToBytes();
            Assert.AreEqual(expectedBuffer.Length, actualBuffer.Length);
            CollectionAssert.AreEquivalent(expectedBuffer, actualBuffer);

            using (var stream = new MemoryStream())
            {
                tlLongStr.ToStream(stream);

                stream.Position = 0;
                actualBuffer = new byte[longBuffer.Length + padding + 4];
                stream.Read(actualBuffer, 0, actualBuffer.Length);
                CollectionAssert.AreEquivalent(expectedBuffer, actualBuffer);
            }
        }

        [TestMethod]
        public void TLStringHydration()
        {
            var padding = 4 - (SHORT_BUFFER.Length % 4);
            var shortBuffer = new byte[1 + SHORT_BUFFER.Length + padding];
            shortBuffer[0] = (byte)SHORT_BUFFER.Length;
            Array.Copy(SHORT_BUFFER, 0, shortBuffer, 1, SHORT_BUFFER.Length);

            var pos = 0;
            var tlStr = new TLString(shortBuffer, ref pos);
            Assert.AreEqual(SHORT_TEXT, tlStr.Value);

            var longTextBuffer = Encoding.UTF8.GetBytes(LONG_TEXT);
            padding = 4 - (longTextBuffer.Length % 4);
            var longBuffer = new byte[4 + longTextBuffer.Length + padding];
            longBuffer[0] = 254;

            var lenBuffer = BitConverter.GetBytes(longTextBuffer.Length)
                .Take(3)
                .Reverse()
                .ToArray();
            Array.Copy(lenBuffer, 0, longBuffer, 1, 3);
            Array.Copy(longTextBuffer, 0, longBuffer, 4, longTextBuffer.Length);

            pos = 0;
            tlStr = new TLString(longBuffer, ref pos);
            Assert.AreEqual(LONG_TEXT, tlStr.Value);

            using (var stream = new MemoryStream())
            {
                stream.Write(shortBuffer, 0, shortBuffer.Length);
                stream.Position = 0;

                pos = 0;
                tlStr = new TLString(stream, ref pos);
                Assert.AreEqual(SHORT_TEXT, tlStr.Value);
            }

            using (var stream = new MemoryStream())
            {
                stream.Write(longBuffer, 0, longBuffer.Length);
                stream.Position = 0;

                pos = 0;
                tlStr = new TLString(stream, ref pos);
                Assert.AreEqual(LONG_TEXT, tlStr.Value);
            }
        }
    }
}
