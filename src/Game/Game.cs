using System;
using System.Collections.Generic;
using System.IO;

namespace ChessBoom.GameBoard
{
    enum Variant
    {
        Standard,
        Chess960,
        Atomic,
        Horde
    }

    public enum Player
    {
        White,
        Black
    }

    public enum Castling
    {
        Kingside,
        Queenside
    }

    public enum GameState
    {
        // TODO: Create a game loop
        Setup,
        InProgress,
        VictoryWhite,
        VictoryBlack,
        // TODO: Implement draw checks
        Draw
    }

    /// <summary>
    /// The Move struct represents a single move and all of its variations via a nested recursive list
    /// </summary>
    struct Move
    {
        /// <summary>
        /// The notation for kingside castling
        /// </summary>
        public const string k_kingsideCastleNotation = "O-O";
        /// <summary>
        /// The notation for kingside castling
        /// </summary>
        public const string k_queensideCastleNotation = "O-O-O";
        public Move(Piece piece, string square)
        {
            m_piece = piece;
            m_square = square;
            m_variations = null;
        }
        public Piece m_piece { get; }
        public string m_square { get; }
        public List<Move>? m_variations { get; set; }

        // TODO: Implement variations
        public void AddVariation(Move move)
        {
            if (m_variations == null)
            {
                m_variations = new List<Move>();
            }
            if (!m_variations.Contains(move))
            {
                m_variations.Add(move);
            }
            else
            {
                throw new ArgumentException($"Variation {move} already exists");
            }
        }

        public override string ToString()
        {
            string? pieceString = m_piece.ToString();
            if (pieceString == null)
            {
                throw new NullReferenceException($"Piece should not be null!");
            }
            return pieceString.ToLower() + m_square;
        }
    }

    /// <summary>
    /// The GameplayErrorException class is used for any case in which gameplay rules are broken
    /// </summary>
    public class GameplayErrorException : Exception
    {
        public GameplayErrorException()
        {
        }

        public GameplayErrorException(string message) : base(message)
        {
        }

        public GameplayErrorException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    /// <summary>
    /// The Game class handles the creation and playing of a game of any chess variant
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The chosen variant for this game
        /// </summary>
        private Variant m_variant = Variant.Standard;
        /// <summary>
        /// The chosen ruleset for this game
        /// </summary>
        public Ruleset m_ruleset { get; set; }
        /// <summary>
        /// The board created for this game
        /// </summary>
        public Board m_board { get; set; }
        /// <summary>
        /// The data structure for all moves and variations
        /// </summary>
        private List<Move> m_moveList;

        /// <summary>
        /// The present game state
        /// </summary>
        public GameState m_gameState { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Game()
        {
            m_board = InitializeBoard(m_variant);
            m_ruleset = Standard.Instance;
            m_moveList = new List<Move>();
            m_gameState = GameState.InProgress;
        }

        /*public Game(Variant variant)
        {
            m_variant = variant;
            Game();
        }*/

        /// <summary>
        /// The board object is created and initialized
        /// </summary>
        /// <param name="variant">The chosen variant for the board</param>
        private Board InitializeBoard(Variant variant)
        {
            string fen = "";

            // Note: Standard and Atomic use the default board. Chess960 and Horde use different initial configurations
            switch (variant)
            {
                case Variant.Chess960:
                    // TODO: CB-24
                    break;
                case Variant.Horde:
                    fen = File.ReadAllText("Resources/horde.fen");
                    break;
                case Variant.Standard:
                case Variant.Atomic:
                default:
                    fen = File.ReadAllText("Resources/default.fen");
                    break;
            }

            return CreateBoardFromFEN(this, fen);
        }

        /// <summary>
        /// Handle the capture that has occurred on a specific square
        /// </summary>
        /// <param name="attacker">The piece that initiated the capture</param>
        /// <param name="coordinate">The square on which the capture takes place</param>
        /// <exception cref="ArgumentException">Thrown the piece on the starting square can not be found or be moved, or the square can not be found</exception>
        public void MakeExplicitMove(string startingSquare, string destinationSquare)
        {
            try
            {
                Piece? piece = m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(startingSquare));
                if (piece == null)
                {
                    throw new ArgumentException($"Piece on square {startingSquare} not found!");
                }
                MakeMove(piece, destinationSquare);
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Attempt to move a piece as a player's move
        /// </summary>
        /// <param name="piece">The piece that will attempt to move</param>
        /// <param name="square">The square that the piece should move to</param>
        /// <exception cref="ArgumentException">Thrown the piece can not be found or be moved, or the square can not be found</exception>
        /// <exception cref="GameplayErrorException">Thrown if the wrong player attempts to make a move or if castling is attempted when illegal</exception>
        public void MakeMove(Piece piece, string square)
        {
            if (m_gameState != GameState.InProgress)
            {
                throw new GameplayErrorException($"Game is not in progress! Illegal move.");
            }
            if (piece.GetPlayer() != m_board.m_playerToPlay)
            {
                throw new GameplayErrorException($"Piece {piece} can not move because it is not {piece.GetPlayer()}\'s turn!");
            }
            m_board.m_halfmoveClock++;

            Board nextBoard = CreateBoardFromFEN(this, CreateFENFromBoard(m_board));
            Piece? nextPiece = nextBoard.GetPiece(piece.GetCoordinates());
            if (nextPiece == null)
            {
                // Should not occur, else the Board copy constructor failed to create a proper deep copy
                Console.WriteLine("Error! Copied piece not found!");
                return;
            }
            try
            {
                if (square == Move.k_kingsideCastleNotation || square == Move.k_queensideCastleNotation)
                {
                    m_ruleset.Castle(nextBoard, nextPiece.GetPlayer(), (square == Move.k_kingsideCastleNotation) ? Castling.Kingside : Castling.Queenside);
                }
                else
                {
                    nextPiece.MovePiece(GameHelpers.GetCoordinateFromSquare(square));
                }
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            catch (GameplayErrorException e)
            {
                throw e;
            }

            if (nextBoard.m_playerToPlay == Player.Black)
            {
                nextBoard.m_fullmoveCount++;
            }
            nextBoard.m_playerToPlay = GameHelpers.GetOpponent(nextBoard.m_playerToPlay);

            if (m_ruleset.IsIllegalBoardState(nextBoard))
            {
                throw new GameplayErrorException("Error! Illegal move!");
            }

            m_board = nextBoard;
            m_moveList.Add(new Move(piece, square));

            m_ruleset.AssessBoardState(this, m_board);
        }

        /// <summary>
        /// The board is created and populated from a .FEN file.
        /// FEN files have 6 parts, delimited by whitespace characters:
        ///     The first part is the piece placements, rows delimited by '/' characters starting on the top.
        ///     The second part denotes the next player to take their turn.
        ///     The third part denotes castling availability.
        ///     The fourth part denotes en passant availability.
        ///     The fifth part denotes the halfmove clock, useful for enforcing the fifty-move rule.
        ///     The sixth part denotes the fullmove number.
        /// </summary>
        /// <param name="fen">The contents of the .FEN file</param>
        public static Board CreateBoardFromFEN(Game game, string fen)
        {
            Board board = new Board(game);
            string[] fenSplit = fen.Split(' ');

            // Create the pieces
            string[] pieceSplit = fenSplit[0].Split('/');
            for (int row = 0; row < GameHelpers.k_BoardHeight; row++)
            {
                int col = 0;

                foreach (char piece in pieceSplit[row])
                {
                    if (Char.IsDigit(piece))
                    {
                        col += (int)Char.GetNumericValue(piece);
                    }
                    else
                    {
                        try
                        {
                            (int, int) coordinate = (col, (GameHelpers.k_BoardHeight - 1) - row);
                            board.CreatePiece(piece, coordinate);
                        }
                        catch (ArgumentException)
                        {

                        }
                        col++;
                    }
                }
            }

            // Set the next player to play
            if (fenSplit[1].Length != 1)
            {
                throw new ArgumentException($"Player \"{fenSplit[1]}\" does not denote a single player.");
            }
            try
            {
                board.SetPlayerToPlay(fenSplit[1][0]);
            }
            catch (ArgumentException)
            {
                board.SetPlayerToPlay('w');
            }

            // Set castling availability
            try
            {
                board.SetCastling(fenSplit[2]);
            }
            catch (ArgumentException)
            {
                board.SetCastling("-");
            }

            // Set en passant capability
            if (fenSplit[3] != "-")
            {
                try
                {
                    board.m_enPassant = GameHelpers.GetCoordinateFromSquare(fenSplit[3]);
                }
                catch (ArgumentException)
                {
                    board.m_enPassant = null;
                }
            }
            else
            {
                board.m_enPassant = null;
            }

            // Set halfmove clock
            try
            {
                board.m_halfmoveClock = int.Parse(fenSplit[4]);
            }
            catch (FormatException)
            {
                board.m_halfmoveClock = 0;
            }

            // Set fullmove number
            try
            {
                board.m_fullmoveCount = int.Parse(fenSplit[5]);
            }
            catch (FormatException)
            {
                board.m_fullmoveCount = 0;
            }

            return board;
        }

        /// <summary>
        /// Retrieve the board state as the contents of a .FEN file
        /// </summary>
        /// <returns>The board state as the contents of a .FEN file</returns>
        public static string CreateFENFromBoard(Board board)
        {
            string fen = "";

            // Retrieve the pieces
            for (int row = GameHelpers.k_BoardHeight - 1; row >= 0; row--)
            {
                int emptySquareCount = 0;
                for (int col = 0; col < GameHelpers.k_BoardWidth; col++)
                {
                    Piece? piece = board.GetPiece((col, row));
                    if (piece == null)
                    {
                        emptySquareCount++;
                        continue;
                    }
                    else
                    {
                        // Append the number of empty squares and reset the count
                        if (emptySquareCount != 0)
                        {
                            fen += emptySquareCount.ToString();
                            emptySquareCount = 0;
                        }

                        fen += piece.ToString();
                    }
                }

                if (emptySquareCount != 0)
                {
                    fen += emptySquareCount.ToString();
                    emptySquareCount = 0;
                }

                if (row != 0)
                {
                    fen += "/";
                }
            }

            //
            fen += " ";
            // Retrieve the next player
            fen += (board.m_playerToPlay == Player.White) ? "w" : "b";
            //
            fen += " ";
            // Retrieve castling availability
            fen += board.GetCastling();
            //
            fen += " ";
            // Retrieve en passant capability
            try
            {
                if (board.m_enPassant.HasValue)
                {
                    fen += GameHelpers.GetSquareFromCoordinate(board.m_enPassant.Value);
                }
                else
                {
                    fen += "-";
                }
            }
            catch (ArgumentException)
            {
                fen += "-";
            }
            //
            fen += " ";
            // Retrieve halfmove clock
            fen += board.m_halfmoveClock.ToString();
            //
            fen += " ";
            // Retrieve fullmove number
            fen += board.m_fullmoveCount.ToString();

            return fen;
        }
    }
}