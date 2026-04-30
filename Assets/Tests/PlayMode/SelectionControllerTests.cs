using NUnit.Framework;
using UnityEngine;
using ChessWorld.Core;

namespace ChessWorld.Game.Tests
{
    public class SelectionControllerTests
    {
        private static (GameObject, SelectionController) BuildRig()
        {
            var go = new GameObject("SelTestRig");
            var bv = go.AddComponent<BoardView>();  // Awake spawns 64 squares
            var sel = go.AddComponent<SelectionController>();
            var engine = new GameEngine();
            engine.StartNewGame();
            sel.Engine = engine;
            sel.Board = bv;
            sel.HumanSide = Side.White;
            return (go, sel);
        }

        [Test]
        public void TappingOwnPiece_EmitsNoMove()
        {
            var (go, sel) = BuildRig();
            Move? emitted = null;
            sel.MoveRequested += m => emitted = m;

            sel.OnSquareTapped(new Square(4, 1));  // white pawn
            Assert.That(emitted, Is.Null);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void TappingLegalDestinationAfterSelection_EmitsMove()
        {
            var (go, sel) = BuildRig();
            Move? emitted = null;
            sel.MoveRequested += m => emitted = m;

            sel.OnSquareTapped(new Square(4, 1));
            sel.OnSquareTapped(new Square(4, 3));

            Assert.That(emitted.HasValue, Is.True);
            Assert.That(emitted.Value.To, Is.EqualTo(new Square(4, 3)));

            Object.DestroyImmediate(go);
        }

        [Test]
        public void TappingIllegalSquareAfterSelection_EmitsNoMoveAndClears()
        {
            var (go, sel) = BuildRig();
            Move? emitted = null;
            sel.MoveRequested += m => emitted = m;

            sel.OnSquareTapped(new Square(4, 1));
            sel.OnSquareTapped(new Square(4, 5));   // illegal destination
            Assert.That(emitted, Is.Null);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void IgnoresTaps_WhenNotHumansTurn()
        {
            var (go, sel) = BuildRig();
            sel.Engine.TryPlayerMove(new Square(4, 1), new Square(4, 3));

            Move? emitted = null;
            sel.MoveRequested += m => emitted = m;
            sel.OnSquareTapped(new Square(3, 6));  // black pawn — human shouldn't control
            Assert.That(emitted, Is.Null);

            Object.DestroyImmediate(go);
        }
    }
}
