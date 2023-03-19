using System;
using System.Collections.Generic;

namespace ChessBoom.Models.Game.Pieces;

public class King : Piece
{
    /// <summary>
    /// Flag to keep track of if the king has moved
    /// </summary>
    private bool m_hasMoved;

    public King(Board board, Player player, (int, int) coordinate) : base(board, player, coordinate)
    {
        m_hasMoved = false;
    }

    public override List<(int, int)> GetMovementSquares()
    {
        var movementSquares = new List<(int, int)>();

        var tryCoordinates = new List<(int, int)>();
        var coordinates = GetCoordinates();
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, 0)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (-1, 1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (0, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (0, 1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, -1)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, 0)));
        tryCoordinates.Add(GameHelpers.AddVector(coordinates, (1, 1)));

        foreach (var coordinate in tryCoordinates)
        {
            if (!GameHelpers.IsOnBoard(coordinate))
            {
                continue;
            }

            var occupant = m_board.GetPiece(coordinate);
            if (occupant is not null && occupant.GetPlayer() == m_owner)
            {
                continue;
            }

            movementSquares.Add(coordinate);
        }

        return movementSquares;
    }

    public override void MovePiece((int, int) coordinate, char? promotionPiece = null)
    {
        base.MovePiece(coordinate, promotionPiece);
        if (!m_hasMoved)
        {
            m_board.RemoveCastling(m_owner, Castling.Kingside);
            m_board.RemoveCastling(m_owner, Castling.Queenside);
            m_hasMoved = true;
        }
    }

    public override List<string> GetLegalMoves()
    {
        List<string> legalSquares = new List<string>();
        if (m_board.m_game is null)
        {
            Console.WriteLine($"Game is null! Legal moves not found");
            return legalSquares;
        }

        foreach ((int, int) square in GetMovementSquares())
        {
            Board newBoard = Game.CreateBoardFromFEN(m_board.m_game, Game.CreateFENFromBoard(m_board));
            string squareName = GameHelpers.GetSquareFromCoordinate(square);
            try
            {
                Piece? king = newBoard.GetPiece(this.GetCoordinates());
                if (king is not null)
                {
                    newBoard.MovePiece(king, squareName);
                    newBoard.m_playerToPlay = GameHelpers.GetOpponent(m_owner);
                }
            }
            catch (ArgumentException)
            {
                continue;
            }

            if (!newBoard.GetRuleset().IsIllegalBoardState(newBoard))
            {
                legalSquares.Add(squareName);
            }
        }

        Board castlingBoard = Game.CreateBoardFromFEN(m_board.m_game, Game.CreateFENFromBoard(m_board));
        try
        {
            castlingBoard.GetRuleset().Castle(castlingBoard, m_owner, Castling.Kingside);
            legalSquares.Add(Move.k_kingsideCastleNotation);
        }
        catch (GameplayErrorException)
        {
        }
        castlingBoard = Game.CreateBoardFromFEN(m_board.m_game, Game.CreateFENFromBoard(m_board));
        try
        {
            castlingBoard.GetRuleset().Castle(castlingBoard, m_owner, Castling.Queenside);
            legalSquares.Add(Move.k_queensideCastleNotation);
        }
        catch (GameplayErrorException)
        {
        }

        return legalSquares;
    }

    public override string ToString()
    {
        return (m_owner == Player.White) ? "K" : "k";
    }
}