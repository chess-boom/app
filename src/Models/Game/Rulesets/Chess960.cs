using System;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.Models.Game.Rulesets;

public class Chess960 : Ruleset
{
    private static readonly Chess960 _instance = new();

    private Chess960()
    {
    }

    public static Chess960 Instance => _instance;

    public override void Capture(Piece attacker, Board board, string square)
    {
        Standard.Instance.Capture(attacker, board, square);
    }

    public override bool IsInCheck(Player player, Board board)
    {
        return Standard.Instance.IsInCheck(player, board);
    }

    public override bool CanCastle(Board board, Player player, Castling side)
    {
        // TODO
        return Standard.Instance.CanCastle(board, player, side);
    }

    public override void Castle(Board board, Player player, Castling side)
    {
        // TODO
        Standard.Instance.Castle(board, player, side);
    }

    public override string GetInitialRookSquare(Player player, Castling side)
    {
        // TODO
        return Standard.Instance.GetInitialRookSquare(player, side);
    }

    public override bool IsIllegalBoardState(Board board)
    {
        return Standard.Instance.IsIllegalBoardState(board);
    }

    public override void AssessBoardState(Game game, Board board)
    {
        Standard.Instance.AssessBoardState(game, board);
    }

    public override Piece GetKing(Board board, Player player)
    {
        return Standard.Instance.GetKing(board, player);
    }

    public static string GenerateRandomLegalFen(int? seed = null)
    {
        // TODO
        Random random;
        if (seed is null)
        {
            random = new Random();
        }
        else
        {
            random = new Random((int) seed);
        }

        // King must be within columns b-g, in between the rooks
        int kingCol = random.Next(1, 7);
        int leftRookCol = random.Next(0, kingCol);
        int rightRookCol = random.Next(kingCol + 1, 8);
        return "";
    }

    public static bool IsLegalFen()
    {
        // TODO
        return true;
    }
}