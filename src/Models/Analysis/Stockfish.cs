using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ChessBoom.Models.Analysis;

/// <summary>
/// Stockfish engine class. Takes inspiration from the archived project https://github.com/Oremiro/Stockfish.NET
/// </summary>
public class Stockfish : IAnalysis
{
    private string? m_FENPosition;
    private readonly string m_engineFilePath;
    private Process m_stockfish;
    private const int s_millisecondDelay = 100;
    private const int s_maxRetries = 15;

    /// <summary>
    /// FEN position we wish to analyze. When setting the fen position, sends a command to Stockfish to set the position in the engine.
    /// </summary>
    public string? FenPosition
    {
        get => m_FENPosition;
        set
        {
            m_FENPosition = value;
            if (IsReady())
            {
                WriteCommand($"position fen {m_FENPosition}");
            }
        }
    }

    public Game.Variant? Variant { get; set; }

    public Stockfish(Game.Variant variant = Game.Variant.Standard)
    {
        // Set variant to Standard if not specified, otherwise set to variant
        Variant = variant;

        // Get stockfish engine location based on OS 
        string directoryString;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Change directory string if variant isn't Standard, use ternary operator
            directoryString = variant == Game.Variant.Standard
                ? @".\AnalysisEngine\Windows\stockfish-windows-2022-x86-64-avx2.exe"
                : @"./AnalysisEngine/Windows/fairy-stockfish-largeboard_x86-64.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            if (RuntimeInformation.ProcessArchitecture ==
                Architecture.X64) // If we are running on x64 architecture and not the new M series chips
            {
                directoryString = "./AnalysisEngine/MacOS/Fairy-Stockfish-14-LargeBoard_Mac_x86-64";
            }
            else
            {
                directoryString = variant == Game.Variant.Standard
                    ? "./AnalysisEngine/MacOS/stockfish"
                    : "./AnalysisEngine/MacOS/Fairy-Stockfish-14-LargeBoard_Mac_Apple_Silicon";
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            directoryString = variant == Game.Variant.Standard
                ? "./AnalysisEngine/Linux/stockfish-ubuntu-20.04-x86-64"
                : "./AnalysisEngine/Linux/fairy-stockfish-largeboard_x86-64";
        }
        else
        {
            throw new NotSupportedException(
                "You are running an unsupported OS, Analysis will not function on your machine");
        }

        m_engineFilePath = directoryString;

        InitializeStockfishProcess();

        if (m_stockfish == null)
        {
            throw new InvalidOperationException("m_stockfish is null!");
        }

        m_stockfish.Start();
        Thread.Sleep(500);
        ReadOutput(); // Read initial output

        // Start of new game
        NewGame();
    }

    /// <summary>
    /// Initializes the Stockfish process so we can perform analysis in Chess Boom
    /// </summary>
    private void InitializeStockfishProcess()
    {
        var processStartInfo = new ProcessStartInfo(m_engineFilePath)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        m_stockfish = new Process();
        m_stockfish.StartInfo = processStartInfo;
    }

    /// <summary>
    /// Write a command to the stockfish engine
    /// </summary>
    /// <param name="command">Command to be written to engine</param>
    public void WriteCommand(string command)
    {
        if (m_stockfish.StandardInput == null)
        {
            throw new InvalidOperationException("StandardInput of m_stockfish is null!");
        }

        m_stockfish.StandardInput.WriteLine(command);
        m_stockfish.StandardInput.Flush();

        m_stockfish.WaitForExit(s_millisecondDelay);
    }

    /// <summary>
    /// Reads a line of output from Stockfish.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">If StandardOutput of our application (m_stockfish) is null, we throw this exception.</exception>
    public string? ReadOutput()
    {
        if (m_stockfish.StandardOutput == null)
        {
            throw new InvalidOperationException("StandardOutput of m_stockfish is null!");
        }

        return m_stockfish.StandardOutput.ReadLine();
    }

    /// <summary>
    /// Get confirmation Stockfish is ready
    /// </summary>
    /// <returns>Returns if Stockfish is ready for inputs (i.e after we sent 'isready' stockfish responded with 'readyok' within the max retries limit.</returns>
    public bool IsReady()
    {
        WriteCommand("isready");
        var tries = 0;
        var isReady = false;
        while (tries < s_maxRetries)
        {
            if (ReadOutput() == "readyok")
            {
                isReady = true;
                break;
            }

            tries++;
        }

        if (!isReady)
        {
            throw new InvalidOperationException(
                "Stockfish is not ready for input! Is the application running properly?");
        }

        return isReady;
    }

    /// <summary>
    /// Tells Stockfish we are starting a new game.
    /// </summary>
    public void NewGame()
    {
        WriteCommand("ucinewgame");
        if (Variant != Game.Variant.Standard) // If variant is not standard, set variant in fairy-stockfish
        {
            SetVariant();
        }

        WriteCommand("position startpos");
    }

    /// <summary>
    /// Sets the variant of the engine to the variant specified in the constructor.
    /// </summary>
    private void SetVariant()
    {
        switch (Variant)
        {
            case Game.Variant.Chess960:
                WriteCommand("setoption name UCI_Variant value chess960");
                break;
            case Game.Variant.Horde:
                WriteCommand("setoption name UCI_Variant value horde");
                break;
            case Game.Variant.Atomic:
                WriteCommand("setoption name UCI_Variant value atomic");
                break;
            default:
                throw new ArgumentException("Variant is not supported!");
        }
    }

    /// <summary>
    /// Gets the static evaluation of the current position as a float, and which side the value is relative to (White or Black).
    /// </summary>
    /// <returns>Evaluation Object. Null if output does not return an evaluation due to an error.</returns>
    public Evaluation? GetStaticEvaluation()
    {
        if (m_FENPosition == null)
            throw new InvalidOperationException("Cannot get an evaluation with no fen position!");
        if (!IsReady()) throw new InvalidOperationException("Stockfish is not ready!");
        WriteCommand("eval");

        var output = ReadOutput();

        if (output is null)
        {
            return null;
        }

        while (!output.Contains("Final evaluation"))
        {
            output = ReadOutput();

            if (output is null)
            {
                break;
            }
        }

        if (output is not null)
        {
            var splitOutput = output.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (!float.TryParse(splitOutput[2], out var evaluationNumber))
                throw new ArgumentException($"Expected a float for evaluation number, got {splitOutput[2]}");

            var
                side = splitOutput[3]
                    .Substring(1); // Normally gives (white or (black so we want to remove the "("

            var sideChar = side[0];

            return new Evaluation(evaluationNumber, sideChar);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the N best moves based on your position.
    /// </summary>
    /// <param name="n">Number of moves you want to return. Value greater than 0. Ordered by descending cp value). Default: 3</param>
    /// <param name="depth">Depth you want the search to go to. Value greater than 0. Default: 10</param>
    /// <returns>List of MoveEvaluations, representing (move, cp value). Ordered from highest cp to lowest cp value moves</returns>
    public List<MoveEvaluation> GetNBestMoves(int n = 3, int depth = 10)
    {
        if (n <= 0)
            throw new ArgumentOutOfRangeException(nameof(n), "n must be > 0");
        if (depth <= 0)
            throw new ArgumentOutOfRangeException(nameof(depth), "depth must be > 0");

        var outputList = new List<string>();

        if (!IsReady()) return new List<MoveEvaluation>();

        // Get Stockfish to return N best moves
        WriteCommand($"setoption name MultiPV value {n}");

        // Run analysis to specified depth
        WriteCommand($"go depth {depth}");

        // Keep reading output until analysis ends
        var output = ReadOutput();

        if (output is null)
        {
            return new List<MoveEvaluation>();
        }

        outputList.Add(output);
        while (!output.Contains("bestmove"))
        {
            output = ReadOutput();
            if (output is not null)
            {
                outputList.Add(output);
            }
            else
            {
                break;
            }
        }

        // Last N+1 lines are the N best moves and the bestmove line.
        var moves = new List<MoveEvaluation>();
        for (var i = outputList.Count - (n + 1); i < outputList.Count - 1; i++)
        {
            var splitOutput = outputList[i].Split();

            // Find the position of "cp" because the next line is the cp value (cp = centipawn)
            var cpIndex = Array.IndexOf(splitOutput, "cp");

            // Find the position of "pv" because the next line is the move;
            var moveIndex = Array.IndexOf(splitOutput, "pv");

            var move = "";
            var cp = int.MaxValue;
            if (cpIndex != -1)
                cp = int.Parse(splitOutput[cpIndex + 1]);
            if (moveIndex != -1)
                move = splitOutput[moveIndex + 1];

            moves.Add(new MoveEvaluation(move, cp));
        }

        moves.Sort((move1, move2) => (move2.Evaluation).CompareTo(move1.Evaluation)); // Sort in place, descending order

        return moves;
    }

    /// <summary>
    /// Close the Stockfish process
    /// </summary>
    public void Close()
    {
        m_stockfish.Close();
    }

    /// <summary>
    /// Checks if the stockfish process is running (responding)
    /// </summary>
    /// <returns>Boolean for whether or not Stockfish is running</returns>
    public bool IsRunning()
    {
        return m_stockfish.Responding;
    }

    /// <summary>
    /// Restart the Stockfish process
    /// </summary>
    public void Restart()
    {
        m_stockfish.Close();
        m_stockfish.Start();
    }
}