using System;
using System.Collections.Generic;

namespace ChessBoom.Models.Game
{
    public class Rook : Piece
    {
        /// <summary>
        /// Flag to keep track of if the king has moved
        /// </summary>
        private bool m_hasMoved;
        public Rook(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
        {
            m_hasMoved = false;
        }

        public override void Destroy()
        {
            try
            {
                string kingsideRookSquare = m_board.GetRuleset().GetInitialRookSquare(m_owner, Castling.Kingside);
                if (m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(kingsideRookSquare)) == this)
                {
                    m_board.RemoveCastling(m_owner, Castling.Kingside);
                }
                string queensideRookSquare = m_board.GetRuleset().GetInitialRookSquare(m_owner, Castling.Queenside);
                if (m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(queensideRookSquare)) == this)
                {
                    m_board.RemoveCastling(m_owner, Castling.Queenside);
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Error! Castling rights removal failed.");
                return;
            }
            base.Destroy();
        }

        public override List<(int, int)> GetMovementSquares()
        {
            List<(int, int)> movementSquares = new List<(int, int)>();
            (int, int) movementVector;
            (int, int) position = GetCoordinates();

            // Top
            movementVector = (0, 1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Left
            movementVector = (-1, 0);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Right
            movementVector = (1, 0);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);
            // Bottom
            movementVector = (0, -1);
            GameHelpers.GetVectorMovementSquares(ref movementSquares, m_board, m_owner, position, movementVector);

            return movementSquares;
        }

        public override void MovePiece((int, int) coordinate)
        {
            base.MovePiece(coordinate);
            if (!m_hasMoved)
            {
                if (m_board.m_game == null)
                {
                    return;
                }
                try
                {
                    string kingsideRookSquare = m_board.GetRuleset().GetInitialRookSquare(m_owner, Castling.Kingside);
                    if (m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(kingsideRookSquare)) == this)
                    {
                        m_board.RemoveCastling(m_owner, Castling.Kingside);
                    }
                    string queensideRookSquare = m_board.GetRuleset().GetInitialRookSquare(m_owner, Castling.Queenside);
                    if (m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(queensideRookSquare)) == this)
                    {
                        m_board.RemoveCastling(m_owner, Castling.Queenside);
                    }
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Error! Castling rights removal failed.");
                    return;
                }

                m_hasMoved = true;
            }
        }

        public override string ToString()
        {
            return (m_owner == Player.White) ? "R" : "r";
        }
    }
}