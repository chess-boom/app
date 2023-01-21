using ChessBoom.Models.Analysis;
using NUnit.Framework;

namespace ChessBoom.NUnitTests.AnalysisTests;

public class AnalysisUnitTests
{
    private IAnalysis _engine = null!;

    [SetUp]
    public void Setup()
    {
        _engine = new Stockfish();
    }
    /// <summary>
    /// Test that the engine can successfully run.
    /// </summary>
    [Test]
    public void AnalysisEngineIsRunning()
    {
        Assert.AreEqual(true, _engine.IsRunning());
    }

    /// <summary>
    /// Test that the analysis engine returns a valid static evaluation for a given FEN
    /// </summary>
    [Test]
    public void AnalysisEngineReturnsStaticEvaluationForFen()
    {
        _engine.FenPosition = "rnbqkbnr/ppp1pppp/8/3pP3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2";

        var staticEval = _engine.GetStaticEvaluation();

        Assert.AreEqual(-0.07f, staticEval.FinalEvaluation);

        Assert.AreEqual('w', staticEval.Side);
    }
    /// <summary>
    /// Test that the analysis engine can return the top N moves, ordered from greatest to lowest CP.
    /// </summary>
    [Test]
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(5)]
    public void AnalysisEngineReturnsTopNMoves(int n)
    {
        _engine.FenPosition = "rnbqkbnr/ppp1pppp/8/3pP3/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 2";

        var topNMoves = _engine.GetNBestMoves(n);
        foreach (var move in topNMoves)
        {
            Console.WriteLine($"Move {move.Item1} CP: {move.Item2}");
        }

        Assert.AreEqual(n, topNMoves.Count);

        if (topNMoves.Count > 1)
        {
            // Ensure the CP values are ordered greatest --> smallest.
            for (int i = 0; i < topNMoves.Count - 1; i++)
            {
                Assert.GreaterOrEqual(topNMoves[i].Item2, topNMoves[i + 1].Item2);
            }
        }
    }

}
