using System;
using System.Collections.Generic;

namespace ChessBoom.Models.Game;

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

    /// <summary>
    /// The limiting number of moves that amount to "no progress" before a game ends in a draw
    /// </summary>
    private const int k_progressMoveLimit = 50;

    public override void Capture(Piece attacker, Board board, string square)
    {
        try
        {
            Piece? capturedPiece = board.GetPiece(GameHelpers.GetCoordinateFromSquare(square));
            if (capturedPiece is not null)
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

        if (king is null
            || rook is null
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
            if (board.GetPiece(coordinate) is not null)
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

        if (king is null
            || rook is null)
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
        // 50-move rule
        if (board.m_halfmoveClock >= (2 * k_progressMoveLimit))
        {
            game.m_gameState = GameState.Draw;
            return;
        }

        // Threefold repetition
        if (game.HasThreefoldRepetition())
        {
            game.m_gameState = GameState.Draw;
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
                Game testGame = new Game();
                testBoard = Game.CreateBoardFromFEN(testGame, Game.CreateFENFromBoard(board));

                try
                {
                    Piece? testPiece = testBoard.GetPiece(piece.GetCoordinates());
                    if (testPiece is not null)
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
            // Only the next player to play may be in checkmate, else an illegal move must have occurred
            if (!IsInCheck(board.m_playerToPlay, board))
            {
                game.m_gameState = GameState.Draw;
                return;
            }
            else
            {
                game.m_gameState = (board.m_playerToPlay == Player.Black) ? GameState.VictoryWhite : GameState.VictoryBlack;
            }
        }
    }
}