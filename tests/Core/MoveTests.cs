using NUnit.Framework;

namespace ChessWorld.Core.Tests
{
    public class MoveTests
    {
        [Test]
        public void Construction_StoresFromToKind()
        {
            var m = new Move(new Square(4, 1), new Square(4, 3), MoveKind.DoublePawnPush);
            Assert.That(m.From, Is.EqualTo(new Square(4, 1)));
            Assert.That(m.To, Is.EqualTo(new Square(4, 3)));
            Assert.That(m.Kind, Is.EqualTo(MoveKind.DoublePawnPush));
            Assert.That(m.PromotionTo, Is.EqualTo(PieceType.None));
        }

        [Test]
        public void PromotionConstructor_StoresPromotionPiece()
        {
            var m = Move.Promotion(new Square(0, 6), new Square(0, 7),
                                   PieceType.Queen, isCapture: false);
            Assert.That(m.Kind, Is.EqualTo(MoveKind.Promotion));
            Assert.That(m.PromotionTo, Is.EqualTo(PieceType.Queen));
        }

        [Test]
        public void Equality_ByAllFields()
        {
            var a = new Move(new Square(0, 0), new Square(0, 1), MoveKind.Normal);
            var b = new Move(new Square(0, 0), new Square(0, 1), MoveKind.Normal);
            Assert.That(a, Is.EqualTo(b));
        }
    }
}
