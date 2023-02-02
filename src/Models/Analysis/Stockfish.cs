using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ChessBoom.Models.Analysis;

/// <summary>
/// The GameplayErrorException class is used for any case in which gameplay rules are broken
/// </summary>
public class StockfishReadyException : Exception
{
    public StockfishReadyException() { }

    public StockfishReadyException(string message) : base(message) { }

    public StockfishReadyException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Stockfish engine class. Takes inspiration from the archived project https://github.com/Oremiro/Stockfish.NET
/// </summary>
public class Stockfish : IAnalysis
{
    protected string? m_fenPosition;
    protected string m_engineFilePath;
    protected Process m_stockfish;
    readonly int s_millisecondDelay = 100;
    readonly int s_maxRetries = 15;

    /// <summary>
    /// FEN position we wish to analyze. When setting the fen position, sends a command to Stockfish to set the position in the engine.
    /// </summary>
    public string? FenPosition
    {
        get { return m_fenPosition; }
        set
        {
            m_fenPosition = value;
            if (IsReady())
            {
                WriteCommand($"position fen {m_fenPosition}");
            }
        }
    }

    public Stockfish()
    {
        // Get stockfish engine location based on OS 
        string directoryString = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            directoryString = @".\AnalysisEngine\Windows\stockfish-windows-2022-x86-64-avx2.exe";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            directoryString = "./AnalysisEngine/MacOS/stockfish-arm64";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            directoryString = "./AnalysisEngine/Linux/stockfish-ubuntu-20.04-x86-64";
        }
        else
        {
            throw new NotSupportedException(
                "You are running an unsupported OS, Analysis will not function on your machine");
        }

        m_engineFilePath = directoryString;

        InitializeStockfishProcess();

        if (m_stockfish is null)
        {
            throw new ArgumentNullException(nameof(m_stockfish));
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
        ProcessStartInfo processStartInfo = new ProcessStartInfo(m_engineFilePath);

        processStartInfo.UseShellExecute = false;
        processStartInfo.RedirectStandardError = true;
        processStartInfo.RedirectStandardInput = true;
        processStartInfo.RedirectStandardOutput = true;

        m_stockfish = new Process();
        m_stockfish.StartInfo = processStartInfo;
    }

    /// <summary>
    /// Write a command to the stockfish engine
    /// </summary>
    /// <param name="command">Command to be written to engine</param>
    /// <exception cref="ArgumentNullException"></exception>
    public void WriteCommand(string command)
    {
        if (m_stockfish.StandardInput is null)
        {
            throw new ArgumentNullException(nameof(m_stockfish.StandardInput));
        }

        m_stockfish.StandardInput.WriteLine(command);
        m_stockfish.StandardInput.Flush();


        m_stockfish.WaitForExit(s_millisecondDelay);
    }

    /// <summary>
    /// Reads a line of output from Stockfish.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">If StandardOutput of our application (m_stockfish) is null, we throw this exception.</exception>
    public string ReadOutput()
    {
        if (m_stockfish.StandardOutput is null)
        {
            throw new ArgumentNullException(nameof(m_stockfish.StandardOutput));
        }

        return m_stockfish.StandardOutput.ReadLine()!;
    }

    /// <summary>
    /// Get confirmation Stockfish is ready
    /// </summary>
    /// <exception cref="StockfishReadyException"></exception>
    /// <returns>Returns if Stockfish is ready for inputs (i.e after we sent 'isready' stockfish responded with 'readyok' within the max retries limit.</returns>
    public bool IsReady()
    {
        WriteCommand("isready");
        int tries = 0;
        bool isReady = false;
        while (tries < s_maxRetries)
        {
            if (ReadOutput() == "readyok")
            {
                isReady = true;
                break;
            }
        }

        if (!isReady)
        {
            throw new StockfishReadyException("Stockfish is not ready for input! Is the application running properly?");
        }

        return isReady;
    }

    /// <summary>
    /// Tells Stockfish we are starting a new game.
    /// </summary>
    public void NewGame()
    {
        WriteCommand("ucinewgame");
        WriteCommand("position startpos");
    }

    /// <summary>
    /// Gets the static evaluation of the current position as a float, and which side the value is relative to (White or Black).
    /// </summary>
    /// <returns>Evaluation Object</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="StockfishReadyException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public Evaluation GetStaticEvaluation()
    {
        if (m_fenPosition != null)
        {
            if (IsReady())
            {
                WriteCommand("eval");

                string output = ReadOutput();
                while (!output.Contains("Final evaluation"))
                {
                    output = ReadOutput();
                }

                string[] splitOutput = output.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (!float.TryParse(splitOutput[2], out var evaluationNumber))
                    throw new ArgumentException($"Expected a float for evaluation number, got {splitOutput[3]}");

                string
                    side = splitOutput[3].Substring(1); // Normally gives (white or (black so we want to remove the "("

                char sideChar = side[0];

                return new Evaluation(evaluationNumber, sideChar);
            }

            throw new StockfishReadyException("Stockfish is not ready!");
        }

        throw new InvalidOperationException("Cannot get an evaluation with no fen position!");
    }

    /// <summary>
    /// Returns the N best moves based on your position.
    /// </summary>
    /// <param name="n">Number of moves you want to return. Value greater than 0. Ordered by descending cp value). Default: 3</param>
    /// <param name="depth">Depth you want the search to go to. Value greater than 0. Default: 10</param>
    /// <returns>List of (string, int) tuples, representing (move, cp value). Ordered from highest cp to lowest cp value moves</returns>
    public List<(string, int)> GetNBestMoves(int n = 3, int depth = 10)
    {
        if (n <= 0 || depth <= 0)
            throw new ArgumentOutOfRangeException("Depth and n must be greater than 0.");

        string[] bestMoves = new string[n];
        List<string> outputList = new List<string>();

        if (IsReady())
        {
            // Get Stockfish to return N best moves
            WriteCommand($"setoption name MultiPV value {n}");

            // Run analysis to specified depth
            WriteCommand($"go depth {depth}");

            // Keep reading output until analysis ends
            string output = ReadOutput();
            outputList.Add(output);
            while (!output.Contains("bestmove"))
            {
                output = ReadOutput();
                outputList.Add(output);
            }

            //Last N+1 lines are the N best moves and the bestmove line.
            List<(string, int)> moves = new List<(string, int)>();
            for (int i = outputList.Count - (n + 1); i < outputList.Count - 1; i++)
            {
                string[] splitOutput = outputList[i].Split();
                int cp_index =
                    Array.IndexOf(splitOutput,
                        "cp"); // Find the position of "cp" because the next line is the cp value (cp = centipawn)
                int move_index =
                    Array.IndexOf(splitOutput, "pv"); // Find the position of "pv" because the next line is the move;

                string move = "";
                int cp = int.MaxValue;
                if (cp_index != -1)
                    cp = int.Parse(splitOutput[cp_index + 1]);
                if (move_index != -1)
                    move = splitOutput[move_index + 1];

                moves.Add((move, cp));
            }

            moves.Sort((x, y) => (y.Item2).CompareTo(x.Item2)); // Sort in place, descending order

            return moves;
        }

        return null;
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