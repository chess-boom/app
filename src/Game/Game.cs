using System;
using System.Collections.Generic;
using System.IO;

namespace ChessBoom.GameBoard
{
    enum Variant {
        Standard,
        Chess960,
        Atomic,
        Horde
    }

    public enum Player {
        White,
        Black
    }

    enum Castling {
        Kingside,
        Queenside
    }

    public class Game {
        private Variant m_variant = Variant.Standard;
        private Ruleset m_ruleset;
        private Board m_board;
        private Player m_playerToPlay {get; set;} = Player.White;
        private Dictionary<Player, List<Castling>> m_castling;
        private (int, int)? m_enPassant;
        private int m_halfmoveClock = 0;
        private int m_fullmoveCount = 0;

        public Game() {
            m_board = new Board();
            m_castling = new Dictionary<Player, List<Castling>>();
            InitializeBoard(m_variant);

            m_ruleset = new Standard();

            System.Console.WriteLine(CreateFENFromBoard());
        }

        /*public Game(Variant variant) {
            m_variant = variant;
            Game();
        }*/

        private void InitializeBoard(Variant variant) {
            string fen = "";

            // Note: Standard and Atomic use the default board. Chess960 and Horde use different initial configurations
            switch (variant) {
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

            CreateBoardFromFEN(fen);
        }

        private void CreateBoardFromFEN(string fen) {
            /* FEN files have 6 parts, delimited by ' ' characters:
            The first part is the piece placements, rows delimited by '/' characters starting on the top
            The second part denotes the next player to take their turn
            The third part denotes castling availability
            The fourth part denotes en passant availability
            The fifth part denotes the halfmove clock, useful for enforcing the fifty-move rule
            The sixth part denotes the fullmove number
            */
            string[] fenSplit = fen.Split(' ');

            // Create the pieces
            string[] pieceSplit = fenSplit[0].Split('/');
            for (int row = 0; row < GameHelpers.k_BoardHeight; row++) {
                int col = 0;

                foreach (char piece in pieceSplit[row]) {
                    if (Char.IsDigit(piece)) {
                        col += (int)Char.GetNumericValue(piece);
                    } else {
                        try {
                            m_board.CreatePiece(piece, row, col);
                        }
                        catch (ArgumentException)
                        {

                        }
                        col++;
                    }
                }
            }

            // Set the next player to play
            if (fenSplit[1].Length != 1) {
                throw new ArgumentException($"Player \"{fenSplit[1]}\" does not denote a single player.");
            }
            try {
                SetPlayerToPlay(fenSplit[1][0]);
            }
            catch (ArgumentException) {
                SetPlayerToPlay('w');
            }

            // Set castling availability
            try {
                SetCastling(fenSplit[2]);
            }
            catch (ArgumentException) {
                SetCastling("-");
            }

            // Set en passant capability
            if (fenSplit[3] != "-") {
                try {
                    m_enPassant = GameHelpers.GetCoordinateFromSquare(fenSplit[3]);
                }
                catch (ArgumentException) {
                    m_enPassant = null;
                }
            } else {
                m_enPassant = null;
            }

            // Set halfmove clock
            try {
                m_halfmoveClock = int.Parse(fenSplit[4]);
            }
            catch (FormatException) {
                m_halfmoveClock = 0;
            }

            // Set fullmove number
            try {
                m_fullmoveCount = int.Parse(fenSplit[5]);
            }
            catch (FormatException) {
                m_fullmoveCount = 0;
            }
        }

        private void SetPlayerToPlay(char player) {
            switch (player) {
                case 'w':
                    m_playerToPlay = Player.White;
                    break;
                case 'b':
                    m_playerToPlay = Player.Black;
                    break;
                default:
                    throw new ArgumentException($"Player \'{player}\' is not a valid player character.");
            }
        }

        private void SetCastling(string castling) {
            List<Castling> whiteCastling = new List<Castling>();
            List<Castling> blackCastling = new List<Castling>();
            foreach (char c in castling) {
                switch (c) {
                    case 'K':
                        whiteCastling.Add(Castling.Kingside);
                        break;
                    case 'k':
                        blackCastling.Add(Castling.Kingside);
                        break;
                    case 'Q':
                        whiteCastling.Add(Castling.Queenside);
                        break;
                    case 'q':
                        blackCastling.Add(Castling.Queenside);
                        break;
                    case '-':
                        break;
                    default:
                        throw new ArgumentException($"Invalid character {c} in FEN file.");
                }
            }
            
            m_castling.Add(Player.White, whiteCastling);
            m_castling.Add(Player.Black, blackCastling);
        }

        private string GetCastling() {
            string castling = "";
            if (m_castling[Player.White].Contains(Castling.Kingside)) {
                castling += "K";
            }
            if (m_castling[Player.White].Contains(Castling.Queenside)) {
                castling += "Q";
            }
            if (m_castling[Player.Black].Contains(Castling.Kingside)) {
                castling += "k";
            }
            if (m_castling[Player.Black].Contains(Castling.Queenside)) {
                castling += "q";
            }
            return castling;
        }

        private string CreateFENFromBoard() {
            string fen = "";

            // Retrieve the pieces
            for (int row = 0; row < GameHelpers.k_BoardHeight; row++) {
                int emptySquareCount = 0;
                for (int col = 0; col < GameHelpers.k_BoardWidth; col++) {
                    Piece? piece = m_board.GetPiece(row, col);
                    if (piece == null) {
                        emptySquareCount++;
                        continue;
                    } else {
                        // Append the number of empty squares and reset the count
                        if (emptySquareCount != 0) {
                            fen += emptySquareCount.ToString();
                            emptySquareCount = 0;
                        }

                        fen += piece.ToString();
                    }
                }

                if (emptySquareCount != 0) {
                    fen += emptySquareCount.ToString();
                    emptySquareCount = 0;
                }

                if (row != GameHelpers.k_BoardHeight - 1) {
                    fen += "/";
                }
            }

            //
            fen += " ";
            // Retrieve the next player
            fen += (m_playerToPlay == Player.White) ? "w" : "b";
            //
            fen += " ";
            // Retrieve castling availability
            fen += GetCastling();
            //
            fen += " ";
            // Retrieve en passant capability
            try {
                if (m_enPassant.HasValue) {
                    fen += GameHelpers.GetSquareFromCoordinate(m_enPassant.Value);
                } else {
                    fen += "-";
                }
            }
            catch (ArgumentException) {
                fen += "-";
            }
            //
            fen += " ";
            // Retrieve halfmove clock
            fen += m_halfmoveClock.ToString();
            //
            fen += " ";
            // Retrieve fullmove number
            fen += m_fullmoveCount.ToString();

            return fen;
        }
    }
}