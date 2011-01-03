using Detergent.Mime;
using MbUnit.Framework;

namespace Detergent.Tests
{
    public class BinaryBufferWithPatternWatchingTests
    {
        [Test]
        public void Test()
        {
            buffer.Start();
            Assert.AreEqual(false, buffer.WriteByte(0));
            Assert.AreEqual(false, buffer.WriteByte(1));
            Assert.AreEqual(false, buffer.WriteByte(2));
            Assert.AreEqual(true, buffer.WriteByte(3));
            Assert.AreEqual(4, buffer.ToArray().Length);
        }

        [Test]
        public void Test3()
        {
            buffer.Start();
            Assert.AreEqual(false, buffer.WriteByte(3));
            Assert.AreEqual(false, buffer.WriteByte(0));
            Assert.AreEqual(false, buffer.WriteByte(1));
            Assert.AreEqual(false, buffer.WriteByte(1));
            Assert.AreEqual(false, buffer.WriteByte(2));
            Assert.AreEqual(true, buffer.WriteByte(3));
            Assert.AreEqual(6, buffer.ToArray().Length);
            buffer.Start();
            Assert.AreEqual(false, buffer.WriteByte(2));
            Assert.AreEqual(true, buffer.WriteByte(3));
            Assert.AreEqual(2, buffer.ToArray().Length);
        }

        [SetUp]
        private void Setup()
        {
            buffer = new BinaryBufferWithPatternWatching(new byte[] { 2, 3 });
        }

        private BinaryBufferWithPatternWatching buffer;
    }
}