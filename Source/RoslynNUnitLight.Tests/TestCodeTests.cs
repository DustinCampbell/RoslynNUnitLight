using NUnit.Framework;

namespace RoslynNUnitLight.Tests
{
    [TestFixture]
    public class TestCodeTests
    {
        [Test]
        public void PositionAtStartOfText()
        {
            var testCode = TestCode.Parse("$$text");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.EqualTo(0));
        }

        [Test]
        public void PositionAtEndOfText()
        {
            var testCode = TestCode.Parse("text$$");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.EqualTo(4));
        }

        [Test]
        public void PositionInMiddleOfText()
        {
            var testCode = TestCode.Parse("te$$xt");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.EqualTo(2));
        }

        [Test]
        public void NoPosition()
        {
            var testCode = TestCode.Parse("text");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.Null);
        }

        [Test]
        public void NoPosition_1_DollarSign()
        {
            var testCode = TestCode.Parse("te$xt");
            Assert.That(testCode.Code, Is.EqualTo("te$xt"));
            Assert.That(testCode.Position, Is.Null);
        }

        [Test]
        public void NoPosition_3_DollarSigns()
        {
            var testCode = TestCode.Parse("te$$$xt");
            Assert.That(testCode.Code, Is.EqualTo("te$$$xt"));
            Assert.That(testCode.Position, Is.Null);
        }

        [Test]
        public void NoPosition_4_DollarSigns()
        {
            var testCode = TestCode.Parse("te$$$$xt");
            Assert.That(testCode.Code, Is.EqualTo("te$$$$xt"));
            Assert.That(testCode.Position, Is.Null);
        }

        [Test]
        public void TwoPositionsThrows()
        {
            Assert.That(() => TestCode.Parse("$$text$$"), Throws.ArgumentException);
        }
    }
}
