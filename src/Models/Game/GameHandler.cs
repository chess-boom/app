using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChessBoom.Models.Game.Pieces;

namespace ChessBoom.Models.Game;

/// <summary>
/// The class which interfaces with the user. Contains a Game and Board.
/// </summary>
public class GameHandler
{
    Game m_game;
    Board m_board;

    public GameHandler()
    {
        // Define the game and board as null to appease compiler
        // Game and board are defined in the StartGame() method
        m_game = null!;
        m_board = null!;
        StartGame();
    }
    public GameHandler(Variant variant)
    {
        // Define the game and board as null to appease compiler
        // Game and board are defined in the StartGame() method
        m_game = null!;
        m_board = null!;
        StartGame(variant);
    }

    /// <summary>
    /// Reinitializes the game. Should always be used at the start of a session
    /// </summary>
    public void StartGame(Variant variant = Variant.Standard)
    {
        m_game = new Game(variant);
        m_board = m_game.m_board;
    }

    /// <summary>
    /// Loads a game from a file
    /// </summary>
    /// <param name="path">The PGN file of the game to load</param>
    /// <exception cref="ArgumentException">Thrown if the PGN is invalid</exception>
    /// <exception cref="FileNotFoundException">Thrown if the PGN could not be found</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown if the PGN's directory could not be found</exception>
    public void LoadGame(string path)
    {
        StartGame();

        Dictionary<string, string> pgn;
        try
        {
            pgn = ReadPGN(path);
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException("Error! PGN file not found!");
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("Error! PGN file is invalid!");
        }
        catch (DirectoryNotFoundException)
        {
            throw new DirectoryNotFoundException("Error! PGN file's directory not found!");
        }

        try
        {
            foreach (string move in ExtractMovesFromPGN(pgn["Moves"]))
            {
                m_game.MakePGNMove(move);
            }
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("Error! PGN contains invalid move!");
        }
    }

    /// <summary>
    /// Processes a PGN file. Parses it for all relevant information and returns that information as a dictionary
    /// </summary>
    /// <param name="path">The PGN file</param>
    /// <returns>The contents of the PGN file, parsed into a dictionary</returns>
    public static Dictionary<string, string> ReadPGN(string path)
    {
        string pgn = File.ReadAllText(path);
        string[] splitPGN = pgn.Split("\n");

        Dictionary<string, string> pgnInfo = new Dictionary<string, string>();
        foreach (string line in splitPGN)
        {
            if (line == "" || line == "\r")
            {
                continue;
            }

            string editedLine = line;
            // Strip appended "\r"
            if (editedLine.Substring(editedLine.Length - 1) == "\r")
            {
                editedLine = editedLine.Substring(0, editedLine.Length - 1);
            }

            // Handle moves
            if (editedLine.Substring(0, 2) == "1.")
            {
                pgnInfo.Add("Moves", editedLine);
                continue;
            }

            // Strip square brackets
            if (editedLine[0] == '[')
            {
                editedLine = editedLine.Substring(1);
            }
            if (editedLine.Substring(editedLine.Length - 1) == "]")
            {
                editedLine = editedLine.Substring(0, editedLine.Length - 1);
            }
            string key = editedLine.Split(" ")[0];
            string dirtyValue = editedLine.Substring(key.Length + 2);
            string value = dirtyValue.Substring(0, dirtyValue.Length - 1);

            pgnInfo.Add(key, value);
        }
        return pgnInfo;
    }

    /// <summary>
    /// Retrieves the list of moves from a game
    /// </summary>
    /// <param name="moveList">List of moves in PGN format</param>
    /// <exception cref="ArgumentException">Thrown if the list of moves extracted from the PGN is invalid</exception>
    private static List<string> ExtractMovesFromPGN(string moveList)
    {
        string editedList = moveList;

        // TODO: Account for variations and comments
        int openParenthesisIndex;
        int closeParenthesisIndex;

        // Remove comments
        while (true)
        {
            openParenthesisIndex = editedList.IndexOf("{");
            if (openParenthesisIndex == -1)
            {
                break;
            }
            closeParenthesisIndex = editedList.IndexOf("}");
            if (closeParenthesisIndex == -1)
            {
                // There is an '{' without a '}'
                throw new ArgumentException("FEN move list is invalid!");
            }

            editedList = string.Concat(editedList.AsSpan(0, openParenthesisIndex), editedList.AsSpan(closeParenthesisIndex + 1));
        }

        // Remove variations
        while (true)
        {
            openParenthesisIndex = editedList.IndexOf("(");
            if (openParenthesisIndex == -1)
            {
                break;
            }
            closeParenthesisIndex = editedList.IndexOf(")");
            if (closeParenthesisIndex == -1)
            {
                // There is an '(' without a ')'
                throw new ArgumentException("FEN move list is invalid!");
            }

            editedList = string.Concat(editedList.AsSpan(0, openParenthesisIndex), editedList.AsSpan(closeParenthesisIndex + 1));
        }

        List<string> moves = editedList.Split().ToList();
        // foreach runs into iteration errors, therefore a simple for should be used
        int moveIndex = 0;
        while (moveIndex < moves.Count)
        {
            if (moves[moveIndex].Length == 0 ||
                (moves[moveIndex][moves[moveIndex].Length - 1] == '.'))
            {
                moves.RemoveAt(moveIndex);
            }
            else
            {
                moveIndex++;
            }
        }

        // Remove game result
        moves.RemoveAt(moves.Count - 1);
        return moves;
    }

    /// <summary>
    /// Make a move from two specified squares
    /// </summary>
    /// <param name="startingSquare">The square from which a piece moves</param>
    /// <param name="destinationSquare">The square to which a piece moves</param>
    /// <exception cref="ArgumentException">Thrown the piece on the starting square can not be found or be moved, or the square can not be found</exception>
    /// <exception cref="GameplayErrorException">Thrown if the attempted move is invalid as per gameplay rules</exception>
    public void MakeMove(string startingSquare, string destinationSquare)
    {
        m_game.MakeExplicitMove(startingSquare, destinationSquare);
    }

    /// <summary>
    /// Get the list of pieces
    /// </summary>
    /// <returns>The list of pieces</returns>
    public List<Piece> GetPieces()
    {
        return m_board.m_pieces;
    }

    /// <summary>
    /// Retrieves a piece from its coordinates
    /// </summary>
    /// <param name="coordinate">The 2-tuple containing the row and column coordinates (0-7, 0-7)</param>
    /// <returns>The piece found on the passed square. If none, returns null</returns>
    public Piece? GetPiece((int, int) coordinate)
    {
        return m_board.GetPiece(coordinate);
    }

    /// <summary>
    /// Retrieves a piece from its square
    /// </summary>
    /// <param name="square">The piece's square</param>
    /// <returns>The piece found on the passed square. If none, returns null</returns>
    public Piece? GetPiece(string square)
    {
        return m_board.GetPiece(GameHelpers.GetCoordinateFromSquare(square));
    }

    /// <summary>
    /// Get the legal moves from the piece on a square
    /// </summary>
    /// <param name="square">The name of the square</param>
    /// <returns>The list of legal moves</returns>
    /// <exception cref="ArgumentException">Thrown if the square parameter is not on the chess board</exception>
    public List<string> GetLegalMoves(string square)
    {
        return this.GetLegalMoves(GameHelpers.GetCoordinateFromSquare(square));
    }

    /// <summary>
    /// Get the legal moves from the piece on a square
    /// </summary>
    /// <param name="coordinate">The coordinate of the square</param>
    /// <returns>The list of legal moves</returns>
    public List<string> GetLegalMoves((int, int) coordinate)
    {
        Piece? piece = m_game.m_board.GetPiece(coordinate);
        if (piece is null)
        {
            Console.WriteLine("Error! No piece has been found!");
            return new List<string>();
        }
        return GetLegalMoves(piece);
    }

    /// <summary>
    /// Get the legal moves from a piece
    /// </summary>
    /// <param name="piece">The piece</param>
    /// <returns>The list of legal moves</returns>
    public List<string> GetLegalMoves(Piece piece)
    {
        return piece.GetLegalMoves();
    }

    /// <summary>
    /// Get the current position in FEN format
    /// </summary>
    /// <returns>The FEN of the game's current position</returns>
    /// <exception cref="GameplayErrorException">Thrown if no game is in progress</exception>
    public string GetCurrentFENPosition()
    {
        if (m_board is null)
        {
            throw new GameplayErrorException("Error! No game is in progress!");
        }
        return Game.CreateFENFromBoard(m_board);
    }

    /// <summary>
    /// Accessor for the game's current state
    /// </summary>
    /// <returns>The current game state</returns>
    /// <exception cref="GameplayErrorException">Thrown if no game is in progress</exception>
    public GameState GetCurrentGameState()
    {
        if (m_game is null)
        {
            throw new GameplayErrorException("Error! No game is in progress!");
        }
        return m_game.m_gameState;
    }

    /// <summary>
    /// Accessor for the board's current player to play
    /// </summary>
    /// <returns>The current player to play</returns>
    public Player GetPlayerToPlay()
    {
        return m_board.m_playerToPlay;
    }
}