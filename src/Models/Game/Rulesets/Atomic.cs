using System;

namespace ChessBoom.Models.Game
{
    public class Atomic : Ruleset
    {
        public override void Capture(Piece attacker, Board board, string square)
        {
            throw new NotImplementedException();
        }

        public override bool IsInCheck(Player player, Board board)
        {
            throw new NotImplementedException();
        }

        public override bool CanCastle(Board board, Player player, Castling side)
        {
            throw new NotImplementedException();
        }

        public override void Castle(Board board, Player player, Castling side)
        {
            throw new NotImplementedException();
        }

        public override string GetInitialRookSquare(Player player, Castling side)
        {
            throw new NotImplementedException();
        }

        public override bool IsIllegalBoardState(Board board)
        {
            throw new NotImplementedException();
        }

        public override void AssessBoardState(Game game, Board board)
        {
            throw new NotImplementedException();
        }
    }
}