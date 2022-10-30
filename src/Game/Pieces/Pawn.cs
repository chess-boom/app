using System.Collections.Generic;

namespace ChessBoom.GameBoard
{
    public class Pawn : Piece
    {
        public Pawn(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
        {
        }

        public override List<(int, int)> GetMovementSquares()
        {
            List<(int, int)> movementSquares = new List<(int, int)>();

            (int, int) coordinates = GetCoordinates();
            (int, int) standardMove;
            (int, int) doubleMove;
            (int, int) captureLeft;
            (int, int) captureRight;

            if (m_owner == Player.White)
            {
                standardMove = GameHelpers.AddVector(coordinates, (0, 1));
                doubleMove = GameHelpers.AddVector(coordinates, (0, 2));
                captureLeft = GameHelpers.AddVector(coordinates, (-1, 1));
                captureRight = GameHelpers.AddVector(coordinates, (1, 1));
            }
            else
            {
                standardMove = GameHelpers.AddVector(coordinates, (0, -1));
                doubleMove = GameHelpers.AddVector(coordinates, (0, -2));
                captureLeft = GameHelpers.AddVector(coordinates, (-1, -1));
                captureRight = GameHelpers.AddVector(coordinates, (1, -1));
            }

            if (GameHelpers.IsOnBoard(standardMove) && (m_board.GetPiece(standardMove) == null))
            {
                movementSquares.Add(standardMove);
            }
            if (GameHelpers.IsOnBoard(standardMove)
                && GameHelpers.IsOnBoard(doubleMove)
                && (m_board.GetPiece(standardMove) == null)
                && (m_board.GetPiece(doubleMove) == null))
            {
                if ((m_owner == Player.White && m_row < 2)
                    || (m_owner == Player.Black && m_row > 5))
                {
                    movementSquares.Add(doubleMove);
                }
            }
            Piece? occupant = m_board.GetPiece(captureLeft);
            if (GameHelpers.IsOnBoard(captureLeft)
                && ((occupant != null) && (occupant.GetPlayer() == GameHelpers.GetOpponent(m_owner)))
                    || (captureLeft == m_board.m_enPassant))
            {
                movementSquares.Add(captureLeft);
            }
            occupant = m_board.GetPiece(captureRight);
            if (GameHelpers.IsOnBoard(captureRight)
                && ((occupant != null) && (occupant.GetPlayer() == GameHelpers.GetOpponent(m_owner)))
                    || (captureRight == m_board.m_enPassant))
            {
                movementSquares.Add(captureRight);
            }

            return movementSquares;
        }

        public override string ToString()
        {
            return (m_owner == Player.White) ? "P" : "p";
        }
    }
}