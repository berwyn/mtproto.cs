using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MTProto.TL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MTProto_Tests.TL
{
    [TestClass]
    public class TLInputPhoneContactTests
    {
        private const long clientID = 25565;
        private const string phoneNumber = "720-555-5555";
        private const string firstName = "John";
        private const string lastName = "Denver";

        [TestMethod]
        public void TLInputPhoneContactHydration()
        {
            var buffer = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0xf392b7f4),
                new TLLong(clientID).ToBytes(),
                new TLString(phoneNumber).ToBytes(),
                new TLString(firstName).ToBytes(),
                new TLString(lastName).ToBytes()
            }.SelectMany(x => x).ToArray();

            var pos = 0;
            var contact = new TLInputPhoneContact(buffer, ref pos);
            Assert.AreEqual(clientID, contact.ClientID.Value);
            Assert.AreEqual(phoneNumber, contact.Phone.Value);
            Assert.AreEqual(firstName, contact.FirstName.Value);
            Assert.AreEqual(lastName, contact.LastName.Value);

            using (var stream = new MemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0;

                pos = 0;
                contact = new TLInputPhoneContact(stream, ref pos);
                Assert.AreEqual(clientID, contact.ClientID.Value);
                Assert.AreEqual(phoneNumber, contact.Phone.Value);
                Assert.AreEqual(firstName, contact.FirstName.Value);
                Assert.AreEqual(lastName, contact.LastName.Value);
            }
        }

        [TestMethod]
        public void TLInputPhoneContactSerialization()
        {
            var expected = new List<byte[]>
            {
                BitConverter.GetBytes((uint)0xf392b7f4),
                new TLLong(clientID).ToBytes(),
                new TLString(phoneNumber).ToBytes(),
                new TLString(firstName).ToBytes(),
                new TLString(lastName).ToBytes()
            }.SelectMany(x => x).ToArray();

            var contact = new TLInputPhoneContact(clientID, phoneNumber, firstName, lastName);
            var actual = contact.ToBytes();
            CollectionAssert.AreEquivalent(expected, actual);

            using (var stream = new MemoryStream())
            {
                contact.ToStream(stream);
                stream.Position = 0;

                actual = new byte[expected.Length];
                stream.Read(actual, 0, expected.Length);
                CollectionAssert.AreEquivalent(expected, actual);
            }
        }
    }
}
