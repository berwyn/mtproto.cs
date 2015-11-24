using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MTProto.TL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MTProto_Tests.TL
{
    [TestClass]
    public class TLInputPeerTests
    {

        [TestMethod]
        public void TLInputPeerHydration()
        {
            var inputEmptyBuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x7f3b18ea)
            }.SelectMany(x => x).ToArray();

            var inputSelfBuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x7da07ec9)
            }.SelectMany(x => x).ToArray();

            var inputContactbuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x1023dbe8),
                new TLInt(42).ToBytes()
            }.SelectMany(x => x).ToArray();

            var inputForeignBuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x9b447325),
                new TLInt(42).ToBytes(),
                new TLLong(25565L).ToBytes()
            }.SelectMany(x => x).ToArray();

            var inputChatBuffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x179be863),
                new TLInt(42).ToBytes()
            }.SelectMany(x => x).ToArray();

            var pos = 0;
            var peerEmpty = new TLInputPeer(inputEmptyBuffer, ref pos);
            Assert.AreEqual(TLInputPeer.Signature.InputPeerEmpty, peerEmpty.SIGNATURE);
            Assert.IsNull(peerEmpty.UserID);
            Assert.IsNull(peerEmpty.AccessHash);
            Assert.IsNull(peerEmpty.ChatID);

            pos = 0;
            var peerSelf = new TLInputPeer(inputSelfBuffer, ref pos);
            Assert.AreEqual(TLInputPeer.Signature.InputPeerSelf, peerSelf.SIGNATURE);
            Assert.IsNull(peerEmpty.UserID);
            Assert.IsNull(peerEmpty.AccessHash);
            Assert.IsNull(peerEmpty.ChatID);

            pos = 0;
            var peerContact = new TLInputPeer(inputContactbuffer, ref pos);
            Assert.AreEqual(TLInputPeer.Signature.InputPeerContact, peerContact.SIGNATURE);
            Assert.AreEqual(42, peerContact.UserID.Value);
            Assert.IsNull(peerContact.AccessHash);
            Assert.IsNull(peerContact.ChatID);

            pos = 0;
            var peerForeign = new TLInputPeer(inputForeignBuffer, ref pos);
            Assert.AreEqual(TLInputPeer.Signature.InputPeerForeign, peerForeign.SIGNATURE);
            Assert.AreEqual(42, peerForeign.UserID.Value);
            Assert.AreEqual(25565L, peerForeign.AccessHash.Value);
            Assert.IsNull(peerForeign.ChatID);

            pos = 0;
            var peerChat = new TLInputPeer(inputChatBuffer, ref pos);
            Assert.AreEqual(TLInputPeer.Signature.InputPeerChat, peerChat.SIGNATURE);
            Assert.AreEqual(42, peerChat.ChatID.Value);
            Assert.IsNull(peerChat.UserID);
            Assert.IsNull(peerChat.AccessHash);

            using (var stream = new MemoryStream())
            {
                stream.Write(inputEmptyBuffer, 0, inputEmptyBuffer.Length);
                stream.Write(inputSelfBuffer, 0, inputSelfBuffer.Length);
                stream.Write(inputContactbuffer, 0, inputContactbuffer.Length);
                stream.Write(inputForeignBuffer, 0, inputForeignBuffer.Length);
                stream.Write(inputChatBuffer, 0, inputChatBuffer.Length);
                stream.Position = 0;

                pos = 0;
                peerEmpty = new TLInputPeer(stream, ref pos);
                Assert.AreEqual(TLInputPeer.Signature.InputPeerEmpty, peerEmpty.SIGNATURE);
                Assert.IsNull(peerEmpty.UserID);
                Assert.IsNull(peerEmpty.AccessHash);
                Assert.IsNull(peerEmpty.ChatID);

                peerSelf = new TLInputPeer(stream, ref pos);
                Assert.AreEqual(TLInputPeer.Signature.InputPeerSelf, peerSelf.SIGNATURE);
                Assert.IsNull(peerEmpty.UserID);
                Assert.IsNull(peerEmpty.AccessHash);
                Assert.IsNull(peerEmpty.ChatID);

                peerContact = new TLInputPeer(stream, ref pos);
                Assert.AreEqual(TLInputPeer.Signature.InputPeerContact, peerContact.SIGNATURE);
                Assert.AreEqual(42, peerContact.UserID.Value);
                Assert.IsNull(peerContact.AccessHash);
                Assert.IsNull(peerContact.ChatID);

                peerForeign = new TLInputPeer(stream, ref pos);
                Assert.AreEqual(TLInputPeer.Signature.InputPeerForeign, peerForeign.SIGNATURE);
                Assert.AreEqual(42, peerForeign.UserID.Value);
                Assert.AreEqual(25565L, peerForeign.AccessHash.Value);
                Assert.IsNull(peerForeign.ChatID);

                peerChat = new TLInputPeer(stream, ref pos);
                Assert.AreEqual(TLInputPeer.Signature.InputPeerChat, peerChat.SIGNATURE);
                Assert.AreEqual(42, peerChat.ChatID.Value);
                Assert.IsNull(peerChat.UserID);
                Assert.IsNull(peerChat.AccessHash);
            }
        }

        [TestMethod]
        public void TLInputPeerSerialization()
        {
            var expectedEmpty = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x7f3b18ea)
            }.SelectMany(x => x).ToArray();
            var actualEmpty = new TLInputPeer
            {
                SIGNATURE = TLInputPeer.Signature.InputPeerEmpty
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedEmpty, actualEmpty);

            var expectedSelf = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x7da07ec9)
            }.SelectMany(x => x).ToArray();
            var actualSelf = new TLInputPeer
            {
                SIGNATURE = TLInputPeer.Signature.InputPeerSelf
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedSelf, actualSelf);

            var expectedContact = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x1023dbe8),
                new TLInt(42).ToBytes()
            }.SelectMany(x => x).ToArray();
            var actualContact = new TLInputPeer
            {
                SIGNATURE = TLInputPeer.Signature.InputPeerContact,
                UserID = new TLInt(42)
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedContact, actualContact);

            var expectedForeign = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x9b447325),
                new TLInt(42).ToBytes(),
                new TLLong(25565L).ToBytes()
            }.SelectMany(x => x).ToArray();
            var actualForeign = new TLInputPeer
            {
                SIGNATURE = TLInputPeer.Signature.InputPeerForeign,
                UserID = new TLInt(42),
                AccessHash = new TLLong(25565L)
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedForeign, actualForeign);

            var expectedChat = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0x179be863),
                new TLInt(42).ToBytes()
            }.SelectMany(x => x).ToArray();
            var actualChat = new TLInputPeer
            {
                SIGNATURE = TLInputPeer.Signature.InputPeerChat,
                ChatID = new TLInt(42)
            }.ToBytes();
            CollectionAssert.AreEquivalent(expectedChat, actualChat);
        }
    }
}
