using NUnit.Framework;
using WalkerScript.Exceptions;
using WalkerScript.Operations;

namespace Walker.UnitTests
{
    [TestFixture]
    public class WalkerOperationTests
    {
        public ScriptOperations Operations { get; private set; }
        
        [SetUp]
        public void SetUp()
        {
            Operations = new ScriptOperations();
        }

        [TearDown]
        public void Cleanup()
        {
            Operations = null;
        }

        [Test]
        public void AssertEqTest()
        {
            Assert.DoesNotThrow(() => { Operations.Assert("abc", eq: "abc"); });
        }

        [Test]
        public void AssertEqFailedTest()
        {
            Assert.Throws<AssertException>(() => { Operations.Assert("abc", eq: "other value"); });
        }

        [Test]
        public void AssertNeFailedTest()
        {
            Assert.Throws<AssertException>(() => { Operations.Assert("abc", ne: "abc"); });
        }

        [Test]
        public void AssertNeTest()
        {
            Assert.DoesNotThrow(() => { Operations.Assert("abc", ne: "other value"); });
        }

        [Test]
        public void AssertLtIntegerTest()
        {
            Assert.DoesNotThrow(() => { Operations.Assert("1", lt: "2"); });
        }

        [Test]
        public void AssertLtTest()
        {
            Assert.DoesNotThrow(() => { Operations.Assert("abc", lt: "bcd"); });
        }

        [Test]
        public void AssertGtIntegerTest()
        {
            Assert.DoesNotThrow(() => { Operations.Assert("5", gt: "3"); });
        }

        [Test]
        public void AssertGtTest()
        {
            Assert.DoesNotThrow(() => { Operations.Assert("CDE", gt: "BCE"); });
        }


        [Test]
        public void AssertLtIntegerFailedTest()
        {
            Assert.Throws<AssertException>(() => { Operations.Assert("7", lt: "2"); });
        }

        [Test]
        public void AssertLtFailedTest()
        {
            Assert.Throws<AssertException>(() => { Operations.Assert("zzz", lt: "bcd"); });
        }

        [Test]
        public void AssertGtIntegerFailedTest()
        {
            Assert.Throws<AssertException>(() => { Operations.Assert("1", gt: "3"); });
        }

        [Test]
        public void AssertGtFailedTest()
        {
            Assert.Throws<AssertException>(() => { Operations.Assert("BBB", gt: "BCE"); });
        }
    }
}
