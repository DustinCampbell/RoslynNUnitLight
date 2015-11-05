using System;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace RoslynNUnitLight.Tests
{
    [TestFixture]
    public class TestCodeTests
    {
        [Test]
        public void NullTextThrows()
        {
            Assert.That(() => TestCode.Parse(null), Throws.InstanceOf<ArgumentNullException>());
        }

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
        public void TwoPositionsThrow()
        {
            Assert.That(() => TestCode.Parse("$$text$$"), Throws.InvalidOperationException);
        }

        [Test]
        public void EmptySpanAtStartOfText()
        {
            var testCode = TestCode.Parse("[||]text");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.Null);
            Assert.That(testCode.GetSpans(), Is.EqualTo(new[] { TextSpan.FromBounds(0, 0) }));
        }

        [Test]
        public void EmptySpanAtEndOfText()
        {
            var testCode = TestCode.Parse("text[||]");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.Null);
            Assert.That(testCode.GetSpans(), Is.EqualTo(new[] { TextSpan.FromBounds(4, 4) }));
        }

        [Test]
        public void EmptySpanInMiddleOfText()
        {
            var testCode = TestCode.Parse("te[||]xt");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.Null);
            Assert.That(testCode.GetSpans(), Is.EqualTo(new[] { TextSpan.FromBounds(2, 2) }));
        }

        [Test]
        public void EmptySpanOnly()
        {
            var testCode = TestCode.Parse("[||]");
            Assert.That(testCode.Code, Is.EqualTo(""));
            Assert.That(testCode.Position, Is.Null);
            Assert.That(testCode.GetSpans(), Is.EqualTo(new[] { TextSpan.FromBounds(0, 0) }));
        }

        [Test]
        public void SpanSurroundingPipeCharacter()
        {
            var testCode = TestCode.Parse("[|||]");
            Assert.That(testCode.Code, Is.EqualTo("|"));
            Assert.That(testCode.Position, Is.Null);
            Assert.That(testCode.GetSpans(), Is.EqualTo(new[] { TextSpan.FromBounds(0, 1) }));
        }

        [Test]
        public void SingleEndSpanThrows()
        {
            Assert.That(() => TestCode.Parse("|]"), Throws.InvalidOperationException);
        }

        [Test]
        public void SingleStartSpanThrows()
        {
            Assert.That(() => TestCode.Parse("[|"), Throws.InvalidOperationException);
        }

        [Test]
        public void ImbalancedStartSpanThrows()
        {
            Assert.That(() => TestCode.Parse("[|[||]"), Throws.InvalidOperationException);
        }

        [Test]
        public void ImbalancedEndSpanThrows()
        {
            Assert.That(() => TestCode.Parse("[||]|]"), Throws.InvalidOperationException);
        }

        [Test]
        public void TwoNonOverlappingSpans()
        {
            var testCode = TestCode.Parse("t[|e|]x[|t|]");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.Null);
            Assert.That(testCode.GetSpans(), Is.EqualTo(new[] { TextSpan.FromBounds(1, 2), TextSpan.FromBounds(3, 4) }));
        }

        [Test]
        public void TwoOverlappingSpans()
        {
            var testCode = TestCode.Parse("t[|e[|x|]t|]");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.Null);
            Assert.That(testCode.GetSpans(), Is.EqualTo(new[] { TextSpan.FromBounds(2, 3), TextSpan.FromBounds(1, 4) }));
        }

        [Test]
        public void TwoOverlappingSpansAndPosition()
        {
            var testCode = TestCode.Parse("t[|e[|x$$|]t|]");
            Assert.That(testCode.Code, Is.EqualTo("text"));
            Assert.That(testCode.Position, Is.EqualTo(3));
            Assert.That(testCode.GetSpans(), Is.EqualTo(new[] { TextSpan.FromBounds(2, 3), TextSpan.FromBounds(1, 4) }));
        }
    }
}
