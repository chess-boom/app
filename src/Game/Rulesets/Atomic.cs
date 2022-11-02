using System;

namespace ChessBoom.GameBoard
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

        public override bool CanCastle(Game game, Player player, Castling side)
        {
            throw new NotImplementedException();
        }

        public override void Castle(Game game, Player player, Castling side)
        {
            throw new NotImplementedException();
        }

        public override string GetInitialRookSquare(Player player, Castling side)
        {
            throw new NotImplementedException();
        }
    }
}