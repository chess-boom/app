using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.Models.Game.Rulesets;

public class Standard : Ruleset
{
    private static readonly Standard _instance = new();

    private Standard()
    {
    }

    public static Standard Instance => _instance;

    public override List<Piece> Capture(Piece attacker, Board board, string square)
    {
        var capturedPiece = board.GetPiece(GameHelpers.GetCoordinateFromSquare(square));

        if (capturedPiece is null)
        {
            throw new ArgumentException("No piece to capture");
        }

        capturedPiece.Destroy();

        return new List<Piece> { capturedPiece };
    }

    public override bool IsInCheck(Player player, Board board)
    {
        // Get any Kings owned by the player (usually just 1 king)
        var playerKings = new List<Piece>();
        foreach (var piece in GameHelpers.GetPlayerPieces(player, board))
        {
            if (piece.GetType() == typeof(King))
            {
                playerKings.Add(piece);
            }
        }

        // Get the squares of all those kings
        var kingSquares = new List<(int, int)>();
        foreach (var king in playerKings)
        {
            kingSquares.Add(king.GetCoordinates());
        }

        // Check if any of the opponent's pieces can move to a square occupied by a king
        return kingSquares.Any(coordinate =>
            GameHelpers.IsSquareVisible(board, GameHelpers.GetOpponent(player), coordinate));
    }

    public override bool CanCastle(Board board, Player player, Castling side)
    {
        if (!board.CanCastleFromFEN(player, side))
        {
            return false;
        }

        Piece? king;
        Piece? rook;

        try
        {
            king = GetKing(board, player);
            rook = GetCastlingRook(board, player, side);
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

        var castlingVector = (side == Castling.Kingside) ? (1, 0) : (-1, 0);
        var intermediateSquares = new List<(int, int)>();
        GameHelpers.GetIntermediateSquares(
            ref intermediateSquares,
            king.GetCoordinates(),
            castlingVector,
            rook.GetCoordinates());

        if (intermediateSquares.Count == 0)
        {
            return false;
        }

        foreach (var coordinate in intermediateSquares)
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

        var playerRow = (player == Player.White) ? "1" : "8";
        var newKingCol = (side == Castling.Kingside) ? "g" : "c";
        var newRookCol = (side == Castling.Kingside) ? "f" : "d";
        Piece? king;
        Piece? rook;

        try
        {
            king = GetKing(board, player);
            rook = GetCastlingRook(board, player, side);
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

        king.CommandMovePiece(GameHelpers.GetCoordinateFromSquare(newKingCol + playerRow));
        rook.CommandMovePiece(GameHelpers.GetCoordinateFromSquare(newRookCol + playerRow));

        board.RemoveCastling(player, Castling.Kingside);
        board.RemoveCastling(player, Castling.Queenside);
    }

    public override string GetInitialRookSquare(Player player, Castling side)
    {
        var playerRow = (player == Player.White) ? "1" : "8";
        var rookCol = (side == Castling.Kingside) ? "h" : "a";
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

        var pieces = GameHelpers.GetPlayerPieces(board.m_playerToPlay, board);
        var legalMoveExists = false;

        foreach (var piece in pieces)
        {
            var pieceMoves = piece.GetMovementSquares();

            foreach (var move in pieceMoves)
            {
                var testGame = new Game();
                var testBoard = Game.CreateBoardFromFEN(testGame, Game.CreateFENFromBoard(board));

                try
                {
                    var testPiece = testBoard.GetPiece(piece.GetCoordinates());
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

        if (legalMoveExists) return;
        // Only the next player to play may be in checkmate, else an illegal move must have occurred
        if (!IsInCheck(board.m_playerToPlay, board))
        {
            game.m_gameState = GameState.Draw;
        }
        else
        {
            game.m_gameState = (board.m_playerToPlay == Player.Black) ? GameState.VictoryWhite : GameState.VictoryBlack;
        }
    }

    public override Piece GetKing(Board board, Player player)
    {
        foreach (var piece in GameHelpers.GetPlayerPieces(player, board))
        {
            if (piece.GetType() == typeof(King))
            {
                return piece;
            }
        }

        throw new GameplayErrorException($"King for player {player} not found!");
    }

    public override Piece? GetCastlingRook(Board board, Player player, Castling side)
    {
        var playerRow = (player == Player.White) ? "1" : "8";
        var rookCol = (side == Castling.Kingside) ? "h" : "a";

        Piece? rook = board.GetPiece(GameHelpers.GetCoordinateFromSquare(rookCol + playerRow));
        bool isCastlingRook = rook is not null && rook.GetType() == typeof(Rook) && rook.GetPlayer() == player;

        return (isCastlingRook) ? rook : null;
    }
}