using System;
using System.Collections.Generic;
using ChessBoom.Models.Game.Pieces;
using ChessBoom.Models.Game.Rulesets;

namespace ChessBoom.Models.Game;

/// <summary>
/// The class which interfaces with the user. Contains a Game and Board.
/// </summary>
public class GameHandler
{
    Game? m_game;
    Board? m_board;

    public GameHandler()
    {
        StartGame();
    }

    /// <summary>
    /// Reinitializes the game. Should always be used at the start of a session
    /// </summary>
    public void StartGame()
    {
        m_game = new Game();
        m_board = m_game.m_board;
    }

    /// <summary>
    /// Loads a game from a file
    /// </summary>
    /// <param name="path">The PGN file of the game to load</param>
    /// <exception cref="ArgumentException">Thrown if the PGN is invalid</exception>
    /// <exception cref="NullReferenceException">Thrown if the game has not yet started. Should not be thrown</exception>
    public void LoadGame(string path)
    {
        StartGame();
        if (m_game is null)
        {
            // Should never be thrown - StartGame() should guarantee m_game is not null
            throw new NullReferenceException();
        }
        try
        {
            foreach (string move in ExtractMovesFromPGN(path))
            {
                m_game.MakePGNMove(move);
            }
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Error! PGN contains invalid move!");
            throw;
        }
        Console.WriteLine("Game successfully loaded!");
    }

    /// <summary>
    /// Retrieves the list of moves from a game
    /// </summary>
    /// <param name="path">The PGN file of the game containing the moves</param>
    private List<string> ExtractMovesFromPGN(string path)
    {
        // TODO
        return new List<string>();
    }

    /// <summary>
    /// Make a move from two specified squares
    /// </summary>
    /// <param name="startingSquare">The square from which a piece moves</param>
    /// <param name="destinationSquare">The square to which a piece moves</param>
    /// <exception cref="ArgumentException">Thrown the piece on the starting square can not be found or be moved, or the square can not be found</exception>
    /// <exception cref="GameplayErrorException">Thrown if the attempted move is invalid as per gameplay rules</exception>
    /// <exception cref="NullReferenceException">Thrown if the game has not yet started</exception>
    public void MakeMove(string startingSquare, string destinationSquare)
    {
        if (m_game is null)
        {
            throw new NullReferenceException("Error! No game is in progress!");
        }
        try
        {
            m_game.MakeExplicitMove(startingSquare, destinationSquare);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (GameplayErrorException)
        {
            throw;
        }
    }

    /// <summary>
    /// Get the list of pieces
    /// </summary>
    /// <returns>The list of pieces</returns>
    public List<Piece> GetPieces()
    {
        if (m_game is null)
        {
            throw new NullReferenceException("Error! No game is in progress!");
        }
        return m_game.m_board.m_pieces;
    }
}