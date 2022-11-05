using NUnit.Framework;
using ChessBoom.GameBoard;

namespace ChessBoom.NUnitTests.GameTests
{
    public class BoardUnitTests
    {
        private Board _board = null!;

        [SetUp]
        public void Setup()
        {
            _board = new Board();
        }

        /// <summary>
        /// Test creating a board and printing out the pieces in the proper order
        /// </summary>
        [Test]
        public void BoardCreationTest()
        {
            //Setting up arbitrary placement of valid pieces
            _board.CreatePiece('p', (1, 1));
            _board.CreatePiece('p', (1, 2));
            _board.CreatePiece('p', (1, 3));
            _board.CreatePiece('p', (1, 4));
            _board.CreatePiece('p', (1, 5));
            _board.CreatePiece('k', (2, 4));
            _board.CreatePiece('q', (2, 5));
            _board.CreatePiece('r', (5, 1));
            _board.CreatePiece('R', (7, 7));


            Assert.AreEqual(".......R\n........\n.pq.....\n.pk.....\n.p......\n.p......\n.p...r..\n........\n", _board.ToString());
        }

        /// <summary>
        /// Test creating an invalid board by placing pieces out of the range of the board both greater and less than row/column size.
        /// </summary>
        [Test]
        public void InvalidPositionThrowsBoardCreationTest()
        {
            //Out of bounds negative
            var exception1 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('p', (-1, 1));
                });

            var exception2 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('p', (1, -1));
                });

            //Out of bounds positive (board is 8x8, we index by 0)
            var exception3 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('p', (8, 1));
                });

            var exception4 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('p', (1, 8));
                });

            //Do not need to do for all messages, validating here is enough
            if (exception1 != null)
                Assert.AreEqual("Coordinate (-1, 1) is an invalid coordinate (x, y).", exception1.Message);
            if (exception2 != null)
                Assert.AreEqual("Coordinate (1, -1) is an invalid coordinate (x, y).", exception2.Message);
        }
        /// <summary>
        /// If an invalid piece, such as 'h' is created, we get an ArgumentException
        /// </summary>
        [Test]
        public void InvalidPieceThrowsBoardCreationTest()
        {
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.CreatePiece('h', (1, 1));
                });

            if (exception != null)
                Assert.AreEqual("Error. h is an invalid piece type.", exception.Message);
        }

        /// <summary>
        /// If we create a piece with an uppercase character, it is white, else it is black.
        /// </summary>
        [Test]
        public void PieceTeamProperlySet()
        {
            _board.CreatePiece('K', (0, 1));
            _board.CreatePiece('k', (0, 2));
            _board.CreatePiece('Q', (1, 1));
            _board.CreatePiece('q', (1, 2));
            _board.CreatePiece('B', (2, 1));
            _board.CreatePiece('b', (2, 2));
            _board.CreatePiece('N', (3, 1));
            _board.CreatePiece('n', (3, 2));
            _board.CreatePiece('R', (4, 1));
            _board.CreatePiece('r', (4, 2));
            _board.CreatePiece('P', (5, 1));
            _board.CreatePiece('p', (5, 2));


            var whiteKing = _board.GetPiece((0, 1));
            var blackKing = _board.GetPiece((0, 2));
            var whiteQueen = _board.GetPiece((1, 1));
            var blackQueen = _board.GetPiece((1, 2));
            var whiteBishop = _board.GetPiece((2, 1));
            var blackBishop = _board.GetPiece((2, 2));
            var whiteKnight = _board.GetPiece((3, 1));
            var blackKnight = _board.GetPiece((3, 2));
            var whiteRook = _board.GetPiece((4, 1));
            var blackRook = _board.GetPiece((4, 2));
            var whitePawn = _board.GetPiece((5, 1));
            var blackPawn = _board.GetPiece((5, 2));


            Assert.IsNotNull(whiteKing);
            Assert.IsNotNull(blackKing);
            Assert.IsNotNull(whiteQueen);
            Assert.IsNotNull(blackQueen);
            Assert.IsNotNull(whiteBishop);
            Assert.IsNotNull(blackBishop);
            Assert.IsNotNull(whiteKnight);
            Assert.IsNotNull(blackKnight);
            Assert.IsNotNull(whiteRook);
            Assert.IsNotNull(blackRook);
            Assert.IsNotNull(whitePawn);
            Assert.IsNotNull(blackPawn);

            if (whiteKing != null)
            {
                Assert.AreEqual(Player.White, whiteKing.GetPlayer());
            }
            if (blackKing != null)
            {
                Assert.AreEqual(Player.Black, blackKing.GetPlayer());
            }
            if (whiteQueen != null)
            {
                Assert.AreEqual(Player.White, whiteQueen.GetPlayer());
            }
            if (blackQueen != null)
            {
                Assert.AreEqual(Player.Black, blackQueen.GetPlayer());
            }
            if (whiteBishop != null)
            {
                Assert.AreEqual(Player.White, whiteBishop.GetPlayer());
            }
            if (blackBishop != null)
            {
                Assert.AreEqual(Player.Black, blackBishop.GetPlayer());
            }
            if (whiteKnight != null)
            {
                Assert.AreEqual(Player.White, whiteKnight.GetPlayer());
            }
            if (blackKnight != null)
            {
                Assert.AreEqual(Player.Black, blackKnight.GetPlayer());
            }
            if (whiteRook != null)
            {
                Assert.AreEqual(Player.White, whiteRook.GetPlayer());
            }
            if (blackRook != null)
            {
                Assert.AreEqual(Player.Black, blackRook.GetPlayer());
            }
            if (whitePawn != null)
            {
                Assert.AreEqual(Player.White, whitePawn.GetPlayer());
            }
            if (blackPawn != null)
            {
                Assert.AreEqual(Player.Black, blackPawn.GetPlayer());
            }
        }

        /// <summary>
        /// If there is an empty spot on the board and we try and get a piece from there, return null.
        /// </summary>
        [Test]
        public void BoardReturnsNullPieceOnEmptySpotTest()
        {
            var piece = _board.GetPiece((5, 5));

            Assert.AreEqual(null, piece);
        }

        /// <summary>
        /// Check that the proper player is set to play
        /// </summary>
        [Test]
        public void PlayerToPlayTest()
        {
            _board.SetPlayerToPlay('w');
            Assert.AreEqual(Player.White, _board.m_playerToPlay);
            _board.SetPlayerToPlay('b');
            Assert.AreEqual(Player.Black, _board.m_playerToPlay);
        }

        /// <summary>
        /// If an invalid player, such as 'h' is set, we get an ArgumentException
        /// </summary>
        [Test]
        public void InvalidPlayingPlayerThrowsTest()
        {
            var exception = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.SetPlayerToPlay('h');
                });

            if (exception != null)
                Assert.AreEqual("Player \'h\' is not a valid player character.", exception.Message);
        }

        /// <summary>
        /// Ensure valid castling notations are handled
        /// </summary>
        [Test]
        public void CheckProperCastlingTest()
        {
            // Null castling
            _board.SetCastling("-");
            var castling = _board.GetCastling();
            Assert.AreEqual("-", castling);

            // Full castling
            _board.SetCastling("KQkq");
            castling = _board.GetCastling();
            Assert.AreEqual("KQkq", castling);

            // Partial castling
            _board.SetCastling("Qk");
            castling = _board.GetCastling();
            Assert.AreEqual("Qk", castling);

            // Misordered castling
            _board.SetCastling("qkQK");
            castling = _board.GetCastling();
            Assert.AreEqual("KQkq", castling);
        }

        /// <summary>
        /// Check if improper castling situations are handled
        /// </summary>
        [Test]
        public void CheckImproperCastlingTest()
        {
            // Improper character
            var exception1 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.SetCastling("h");
                });
            // Repeated character
            var exception2 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.SetCastling("QQ");
                });
            // Empty argument
            var exception3 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.SetCastling("");
                });
            // Appended hyphen
            var exception4 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.SetCastling("KQ-");
                });
            // Prepended hyphen
            var exception5 = Assert.Throws<ArgumentException>(
                delegate
                {
                    _board.SetCastling("-KQ");
                });

            if (exception1 != null)
                Assert.AreEqual("Invalid character \'h\' in FEN file.", exception1.Message);
            if (exception2 != null)
                Assert.AreEqual("Duplicate character \'Q\' in FEN file.", exception2.Message);
            if (exception3 != null)
                Assert.AreEqual("FEN file must include castling rights.", exception3.Message);
            if (exception4 != null)
                Assert.AreEqual("Character \'-\' must represent null castling rights in FEN file.", exception4.Message);
            if (exception5 != null)
                Assert.AreEqual("No castling rights must be represented by a single \'-\'.", exception5.Message);
        }
    }
}
