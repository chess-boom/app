using System;
using System.Collections.Generic;

namespace ChessBoom.Models.Game
{
    public class Standard : Ruleset
    {
        private static readonly Standard instance = new Standard();
        private Standard()
        {
        }

        public static Standard Instance
        {
            get
            {
                return instance;
            }
        }

        public override void Capture(Piece attacker, Board board, string square)
        {
            try
            {
                Piece? capturedPiece = board.GetPiece(GameHelpers.GetCoordinateFromSquare(square));
                if (capturedPiece != null)
                {
                    capturedPiece.Destroy();
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
        }

        public override bool IsInCheck(Player player, Board board)
        {
            // Get any Kings owned by the player (usually just 1 king)
            List<Piece> playerKings = new List<Piece>();
            foreach (Piece piece in GameHelpers.GetPlayerPieces(player, board))
            {
                if (piece.GetType() == typeof(King))
                {
                    playerKings.Add(piece);
                }
            }

            // Get the squares of all those kings
            List<(int, int)> kingSquares = new List<(int, int)>();
            foreach (Piece king in playerKings)
            {
                kingSquares.Add(king.GetCoordinates());
            }

            // Check if any of the opponent's pieces can move to a square occupied by a king
            foreach ((int, int) coordinate in kingSquares)
            {
                if (GameHelpers.IsSquareVisible(board, GameHelpers.GetOpponent(player), coordinate))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CanCastle(Board board, Player player, Castling side)
        {
            if (!board.CanCastleFromFEN(player, side))
            {
                return false;
            }
            string playerRow = (player == Player.White) ? "1" : "8";
            string rookCol = (side == Castling.Kingside) ? "h" : "a";
            Piece? king;
            Piece? rook;

            (int, int) kingCoordinate;

            try
            {
                kingCoordinate = GameHelpers.GetCoordinateFromSquare("e" + playerRow);
                king = board.GetPiece(kingCoordinate);
                rook = board.GetPiece(GameHelpers.GetCoordinateFromSquare(rookCol + playerRow));
            }
            catch (ArgumentException)
            {
                // Caught when coordinates are illegal
                return false;
            }

            if (king == null
                || rook == null
                || king.GetType() != typeof(King)
                || rook.GetType() != typeof(Rook))
            {
                // Pieces were not found or were of the wrong type
                return false;
            }

            if (IsInCheck(player, board))
            {
                // King is currently in check
                return false;
            }

            (int, int) castlingVector = (side == Castling.Kingside) ? (1, 0) : (-1, 0);
            List<(int, int)> intermediateSquares = new List<(int, int)>();
            GameHelpers.GetVectorMovementSquares(
                ref intermediateSquares,
                board,
                player,
                kingCoordinate,
                castlingVector);

            foreach ((int, int) coordinate in intermediateSquares)
            {
                if (GameHelpers.IsSquareVisible(board, GameHelpers.GetOpponent(player), coordinate))
                {
                    // An opponent's piece can see a square between the king and the rook (castling through check)
                    return false;
                }
                if (board.GetPiece(coordinate) != null)
                {
                    // A piece exists between the king and the rook
                    return false;
                }
            }

            return true;
        }

        public override void Castle(Board board, Player player, Castling side)
        {
            if (!CanCastle(board, player, side))
            {
                throw new GameplayErrorException("Castling is illegal in this situation!");
            }

            string playerRow = (player == Player.White) ? "1" : "8";
            string rookCol = (side == Castling.Kingside) ? "h" : "a";
            string newKingCol = (side == Castling.Kingside) ? "g" : "c";
            string newRookCol = (side == Castling.Kingside) ? "f" : "d";
            Piece? king;
            Piece? rook;

            (int, int) kingCoordinate;

            try
            {
                kingCoordinate = GameHelpers.GetCoordinateFromSquare("e" + playerRow);
                king = board.GetPiece(kingCoordinate);
                rook = board.GetPiece(GameHelpers.GetCoordinateFromSquare(rookCol + playerRow));
            }
            catch (ArgumentException)
            {
                // Caught when coordinates are illegal. Should never occur since CanCastle must return true.
                throw new GameplayErrorException("Castling is illegal in this situation!");
            }

            if (king == null
                || rook == null)
            {
                // Pieces were not found. Should never occur since CanCastle must return true.
                throw new GameplayErrorException("Castling is illegal in this situation!");
            }

            try
            {
                king.CommandMovePiece(GameHelpers.GetCoordinateFromSquare(newKingCol + playerRow));
                rook.CommandMovePiece(GameHelpers.GetCoordinateFromSquare(newRookCol + playerRow));
            }
            catch (ArgumentException)
            {
                throw;
            }

            board.RemoveCastling(player, Castling.Kingside);
            board.RemoveCastling(player, Castling.Queenside);
        }

        public override string GetInitialRookSquare(Player player, Castling side)
        {
            string playerRow = (player == Player.White) ? "1" : "8";
            string rookCol = (side == Castling.Kingside) ? "h" : "a";
            return rookCol + playerRow;
        }

        public override bool IsIllegalBoardState(Board board)
        {
            return IsInCheck(GameHelpers.GetOpponent(board.m_playerToPlay), board);
        }

        public override void AssessBoardState(Game game, Board board)
        {
            // Only the next player to play may be in checkmate, else an illegal move must have occurred
            if (!IsInCheck(board.m_playerToPlay, board))
            {
                // TODO: Assess for stalemate
                return;
            }

            List<Piece> pieces = GameHelpers.GetPlayerPieces(board.m_playerToPlay, board);
            Board testBoard;
            bool legalMoveExists = false;

            foreach (Piece piece in pieces)
            {
                List<(int, int)> pieceMoves = piece.GetMovementSquares();

                foreach ((int, int) move in pieceMoves)
                {
                    testBoard = Game.CreateBoardFromFEN(game, Game.CreateFENFromBoard(board));

                    try
                    {
                        Piece? testPiece = testBoard.GetPiece(piece.GetCoordinates());
                        if (testPiece != null)
                        {
                            testPiece.MovePiece(move);
                        }
                        else
                        {
                            Console.WriteLine("Error! Test piece from duplicated board not found!");
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"Error! {e}");
                    }
                    catch (GameplayErrorException e)
                    {
                        Console.WriteLine($"Error! {e}");
                    }

                    testBoard.m_playerToPlay = GameHelpers.GetOpponent(testBoard.m_playerToPlay);
                    if (!IsIllegalBoardState(testBoard))
                    {
                        legalMoveExists = true;
                        break;
                    }
                }
                if (legalMoveExists)
                {
                    break;
                }
            }

            if (!legalMoveExists)
            {
                game.m_gameState = (board.m_playerToPlay == Player.Black) ? GameState.VictoryWhite : GameState.VictoryBlack;
            }
        }
    }
}