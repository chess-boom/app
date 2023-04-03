using System;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.Models.Game.Rulesets;

public class Horde : Ruleset
{
    private static readonly Horde _instance = new();

    private Horde()
    {
    }

    public static Horde Instance => _instance;

    public override void Capture(Piece attacker, Board board, string square)
    {
        Standard.Instance.Capture(attacker, board, square);
    }

    public override bool IsInCheck(Player player, Board board)
    {
        if (player == Player.White)
        {
            return false;
        }
        return Standard.Instance.IsInCheck(player, board);
    }

    public override bool CanCastle(Board board, Player player, Castling side)
    {
        if (player == Player.White)
        {
            return false;
        }
        return Standard.Instance.CanCastle(board, player, side);
    }

    public override void Castle(Board board, Player player, Castling side)
    {
        if (player == Player.White)
        {
            return;
        }
        Standard.Instance.Castle(board, player, side);
    }

    public override string GetInitialRookSquare(Player player, Castling side)
    {
        if (player == Player.White)
        {
            return "";
        }
        return Standard.Instance.GetInitialRookSquare(player, side);
    }

    public override bool IsIllegalBoardState(Board board)
    {
        return Standard.Instance.IsIllegalBoardState(board);
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
        if (GameHelpers.GetPlayerPieces(Player.White, board).Count == 0)
        {
            game.m_gameState = GameState.VictoryBlack;
            return;
        }
        if (!IsInCheck(board.m_playerToPlay, board))
        {
            game.m_gameState = GameState.Draw;
        }
        else
        {
            game.m_gameState = GameState.VictoryWhite;
        }
    }

    public override Piece GetKing(Board board, Player player)
    {
        return Standard.Instance.GetKing(board, player);
    }

    public override Piece? GetCastlingRook(Board board, Player player, Castling side)
    {
        return (player == Player.Black) ? Standard.Instance.GetCastlingRook(board, player, side) : null;
    }
}