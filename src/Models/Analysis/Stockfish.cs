using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ChessBoom.Models.Analysis
{
    /// <summary>
    /// Stockfish engine class. Takes inspiration from the archived project https://github.com/Oremiro/Stockfish.NET
    /// </summary>
    class Stockfish : IAnalysis
    {
        protected string? m_fenPosition;
        protected string m_engineFilePath;
        protected Process m_stockfish;
        readonly int s_millisecondDelay = 100;
        readonly int s_maxRetries = 15;
        /// <summary>
        /// FEN position we wish to analyze.
        /// </summary>
        public string FenPosition
        {
            get { return m_fenPosition; }
            set { m_fenPosition = value; }
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
                directoryString = @".\AnalysisEngine\MacOS\stockfish-arm64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                directoryString = @".\AnalysisEngine\Linux\stockfish-ubuntu-20.04-x86-64";
            }
            else
            {
                throw new NotSupportedException("You are running an unsupported OS, Analysis will not function on your machine");
            }

            m_engineFilePath = directoryString;

            InitializeStockfishProcess();

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
        public void WriteCommand(string command)
        {
            if (m_stockfish.StandardInput == null)
            {
                throw new NullReferenceException();
            }

            m_stockfish.StandardInput.WriteLine(command);
            m_stockfish.StandardInput.Flush();


            m_stockfish.WaitForExit(s_millisecondDelay);
        }

        /// <summary>
        /// Reads a line of output from Stockfish.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">If StandardOutput of our application (m_stockfish) is null, we throw this exception.</exception>
        public string ReadOutput()
        {

            if (m_stockfish.StandardOutput == null)
            {
                throw new NullReferenceException("StandardOutput of m_stockfish is null!");
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
                throw new ApplicationException("Stockfish is not ready for input! Is the application running properly?");
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
        /// Gets the evaluation of the current position as a float, and which side the value is relative to (White or Black).
        /// </summary>
        /// <returns>Evaluation Object</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Evaluation GetEvaluation()
        {
            if (m_fenPosition != null)
            {
                if (IsReady())
                {
                    WriteCommand($"position fen {FenPosition}");
                    WriteCommand("eval");

                    string output = ReadOutput();
                    while (!output.Contains("Final evaluation"))
                    {
                        output = ReadOutput();
                    }

                    string[] splitOutput = output.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    float evaluation_number = float.MaxValue;
                    if (!float.TryParse(splitOutput[2], out evaluation_number))
                        throw new ArgumentException($"Expected a float for evaluation number, got {splitOutput[3]}");

                    string side = splitOutput[3].Substring(1); // Normally gives (white or (black so we want to remove the "("

                    char side_char = side[0];

                    return new Evaluation(evaluation_number, side_char);

                }
                throw new ApplicationException("Stockfish is not ready!");
            }
            throw new InvalidOperationException("Cannot get an evaluation with no fen position!");
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
        /// <summary>
        /// Temp, TODO remove. To get this to work in Visual Studio:
        /// 1. Go to Project --> ChessBoom Properties
        /// 2. Set Startup Object as MainClass
        /// 3. Set Output Type to Console Application
        /// </summary>
        //class MainClass
        //{
        //    static void Main(string[] args)
        //    {
        //        Stockfish stockfish = new Stockfish();
        //        stockfish.FenPosition = "rnbqkbnr/ppp1pppp/8/3pP3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2";
        //        Console.WriteLine(stockfish.GetEvaluation());
        //    }
        //}
}

