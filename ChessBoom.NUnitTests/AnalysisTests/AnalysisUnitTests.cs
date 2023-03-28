using System.Runtime.InteropServices;
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

    [TearDown]
    public void TearDown()
    {
        _engine.Close();
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

        Assert.IsNotNull(staticEval);

        if (staticEval is not null) // Technically unecessary, but removes null dereference error
        {
            // If we are on windows or linux, we can expect a static evaluation of -0.07
            // If we are on mac, we can expect a static evaluation of -0.18
            // The mac version is more recent and uses a different evaluation function, so the values are different.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.AreEqual(-0.18f, staticEval.FinalEvaluation, "This value may change if we update Stockfish");
            }
            else
            {
                Assert.AreEqual(-0.07f, staticEval.FinalEvaluation, "This value may change if we update Stockfish");
            }

            Assert.AreEqual('w', staticEval.Side);
        }

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

        Assert.IsNotNull(topNMoves);

        if (topNMoves is not null)
        {
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

    /// <summary>
    /// Test that analysis engine can run on all supported variants.
    /// Does not use the Setup method, as we need to create a new engine for each variant.
    /// </summary>
    [Test]
    [TestCase("Chess960")]
    [TestCase("Horde")]
    [TestCase("Atomic")]
    public void AnalysisEngineCanRunOnAllSupportedVariants(string variant)
    {
        var engine = new Stockfish(variant);

        Assert.AreEqual(true, engine.IsRunning());

        engine.Close();
    }

    /// <summary>
    /// Test that analysis engine throws an ArgumentException when given an unsupported variant.
    /// Does not use the Setup method, as we need to create a new engine for each variant.
    /// </summary>
    [Test]
    [TestCase("Crazyhouse")]
    [TestCase("KingOfTheHill")]
    [TestCase("ThreeCheck")]
    public void AnalysisEngineThrowsArgumentExceptionOnUnsupportedVariant(string variant)
    {
        Assert.Throws<ArgumentException>(() => new Stockfish(variant));
    }

}
