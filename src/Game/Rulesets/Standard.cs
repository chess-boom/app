using System;

namespace ChessBoom.GameBoard
{
    public class Standard : Ruleset
    {
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
            catch (ArgumentException e)
            {
                throw e;
            }
        }
    }
}