using NUnit.Framework;
using ChessBoom.GameBoard;

namespace ChessBoom.NUnitTests.GameTests
{
    public class PieceUnitTests
    {
        private Player _whitePlayer;
        private Player _blackPlayer;

        [SetUp]
        public void Setup()
        {
            _whitePlayer = Player.White;
            _blackPlayer = Player.Black;
        }
        /// <summary>
        /// Ensure that a Piece outputs a capital letter of on the White team and a lowercase letter if on the Black team.
        /// </summary>
        [Test]
        public void EnsurePieceOutputsRightCapitalizationPerTeam()
        {
            Piece whiteKing = new King(_whitePlayer, 0, 0);
            Piece blackKing = new King(_blackPlayer, 0, 1);
            Piece whiteQueen = new Queen(_whitePlayer, 1, 0);
            Piece blackQueen = new Queen(_blackPlayer, 1, 1);
            Piece whiteBishop = new Bishop(_whitePlayer, 2, 0);
            Piece blackBishop = new Bishop(_blackPlayer, 2, 1);
            Piece whiteKnight = new Knight(_whitePlayer, 3, 0);
            Piece blackKnight = new Knight(_blackPlayer, 3, 1);
            Piece whiteRook = new Rook(_whitePlayer, 4, 0);
            Piece blackRook = new Rook(_blackPlayer, 4, 1);
            Piece whitePawn = new Pawn(_whitePlayer, 5, 0);
            Piece blackPawn = new Pawn(_blackPlayer, 5, 1);


            Assert.AreEqual("K", whiteKing.ToString());
            Assert.AreEqual("k", blackKing.ToString());
            Assert.AreEqual("Q", whiteQueen.ToString());
            Assert.AreEqual("q", blackQueen.ToString());
            Assert.AreEqual("B", whiteBishop.ToString());
            Assert.AreEqual("b", blackBishop.ToString());
            Assert.AreEqual("N", whiteKnight.ToString());
            Assert.AreEqual("n", blackKnight.ToString());
            Assert.AreEqual("R", whiteRook.ToString());
            Assert.AreEqual("r", blackRook.ToString());
            Assert.AreEqual("P", whitePawn.ToString());
            Assert.AreEqual("p", blackPawn.ToString());
        }
    }
}
