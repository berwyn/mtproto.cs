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
    public class TLInputUserTests
    {
        [TestMethod]
        public void TLInputUserHydration()
        {
            var inputEmptyBuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0xb98886cf)
            }.SelectMany(x => x).ToArray();

            var inputSelfBuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0xf7c1b13f)
            }.SelectMany(x => x).ToArray();

            var inputContactbuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x86e94f65),
                new TLInt(42).ToBytes()
            }.SelectMany(x => x).ToArray();

            var inputForeignBuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x655e74ff),
                new TLInt(42).ToBytes(),
                new TLLong(25565L).ToBytes()
            }.SelectMany(x => x).ToArray();

            var pos = 0;
            var userEmpty = new TLInputUser(inputEmptyBuffer, ref pos);
            Assert.AreEqual(TLInputUser.Signature.InputUserEmpty, userEmpty.SIGNATURE);
            Assert.IsNull(userEmpty.UserID);
            Assert.IsNull(userEmpty.AccessHash);

            pos = 0;
            var userSelf = new TLInputUser(inputSelfBuffer, ref pos);
            Assert.AreEqual(TLInputUser.Signature.InputUserSelf, userSelf.SIGNATURE);
            Assert.IsNull(userSelf.UserID);
            Assert.IsNull(userSelf.AccessHash);

            pos = 0;
            var userContact = new TLInputUser(inputContactbuffer, ref pos);
            Assert.AreEqual(TLInputUser.Signature.InputUserContact, userContact.SIGNATURE);
            Assert.AreEqual(42, userContact.UserID.Value);
            Assert.IsNull(userContact.AccessHash);

            pos = 0;
            var userForeign = new TLInputUser(inputForeignBuffer, ref pos);
            Assert.AreEqual(TLInputUser.Signature.InputUserForeign, userForeign.SIGNATURE);
            Assert.AreEqual(42, userForeign.UserID.Value);
            Assert.AreEqual(25565L, userForeign.AccessHash.Value);

            using (var stream = new MemoryStream())
            {
                stream.Write(inputEmptyBuffer, 0, inputEmptyBuffer.Length);
                stream.Write(inputSelfBuffer, 0, inputSelfBuffer.Length);
                stream.Write(inputContactbuffer, 0, inputContactbuffer.Length);
                stream.Write(inputForeignBuffer, 0, inputForeignBuffer.Length);
                stream.Position = 0;

                pos = 0;
                userEmpty = new TLInputUser(stream, ref pos);
                Assert.AreEqual(TLInputUser.Signature.InputUserEmpty, userEmpty.SIGNATURE);
                Assert.IsNull(userEmpty.UserID);
                Assert.IsNull(userEmpty.AccessHash);

                userSelf = new TLInputUser(stream, ref pos);
                Assert.AreEqual(TLInputUser.Signature.InputUserSelf, userSelf.SIGNATURE);
                Assert.IsNull(userSelf.UserID);
                Assert.IsNull(userSelf.AccessHash);

                userContact = new TLInputUser(stream, ref pos);
                Assert.AreEqual(TLInputUser.Signature.InputUserContact, userContact.SIGNATURE);
                Assert.AreEqual(42, userContact.UserID.Value);
                Assert.IsNull(userContact.AccessHash);

                userForeign = new TLInputUser(stream, ref pos);
                Assert.AreEqual(TLInputUser.Signature.InputUserForeign, userForeign.SIGNATURE);
                Assert.AreEqual(42, userForeign.UserID.Value);
                Assert.AreEqual(25565L, userForeign.AccessHash.Value);
            }
        }

        [TestMethod]
        public void TLInputUserSerialization()
        {
            var expectedEmpty = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0xb98886cf)
            }.SelectMany(x => x).ToArray();
            var actualEmpty = new TLInputUser
            {
                SIGNATURE = TLInputUser.Signature.InputUserEmpty
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedEmpty, actualEmpty);

            var expectedSelf = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0xf7c1b13f)
            }.SelectMany(x => x).ToArray();
            var actualSelf = new TLInputUser
            {
                SIGNATURE = TLInputUser.Signature.InputUserSelf
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedSelf, actualSelf);

            var expectedContact = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x86e94f65),
                new TLInt(42).ToBytes()
            }.SelectMany(x => x).ToArray();
            var actualContact = new TLInputUser
            {
                SIGNATURE = TLInputUser.Signature.InputUserContact,
                UserID = new TLInt(42)
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedContact, actualContact);

            var expectedForeign = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x655e74ff),
                new TLInt(42).ToBytes(),
                new TLLong(25565L).ToBytes()
            }.SelectMany(x => x).ToArray();
            var actualForeign = new TLInputUser
            {
                SIGNATURE = TLInputUser.Signature.InputUserForeign,
                UserID = new TLInt(42),
                AccessHash = new TLLong(25565L)
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedForeign, actualForeign);
        }
    }
}
