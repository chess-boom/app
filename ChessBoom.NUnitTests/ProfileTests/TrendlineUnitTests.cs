using ChessBoom.Models.Profile;
using NUnit.Framework;

namespace ChessBoom.NUnitTests.ProfileTests;

public class TrendlineUnitTests
{
    private Trendline trendline = null!;
    private Trendline invalidTrendline = null!;

    [SetUp]
    public void Setup()
    {
        IList<int> yAxisValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        IList<int> xAxisValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        trendline = new Trendline(yAxisValues, xAxisValues);
        IList<int> invalidyAxisValues = new List<int> { 0, 0 };
        IList<int> invalidxAxisValues = new List<int> { 0, 0 };
        invalidTrendline = new Trendline(invalidyAxisValues, invalidxAxisValues);
    }

    /// <summary>
    /// Check the slope of the trendline is correct
    /// </summary>
    [Test]
    public void CheckSlopeTest()
    {
        var slope = trendline.Slope;
        Assert.AreEqual(1, slope);
    }

    /// <summary>
    /// Check the slope calculation catches division by zero
    /// </summary>
    [Test]
    public void CheckSlopeZeroTest()
    {
        var slope = invalidTrendline.Slope;
        Assert.AreEqual(0, slope);
    }

    /// <summary>
    /// Check the intercept of the trendline is correct
    /// </summary>
    [Test]
    public void CheckInterceptTest()
    {
        var intercept = trendline.Intercept;
        Assert.AreEqual(0, intercept);
    }

    /// <summary>
    /// Check the start of the trendline is correct
    /// </summary>
    [Test]
    public void CheckStartTest()
    {
        var start = trendline.Start;
        Assert.AreEqual(1, start);
    }
    /// <summary>
    /// Check the end of the trendline is correct
    /// </summary>
    [Test]
    public void CheckEndTest()
    {
        var end = trendline.End;
        Assert.AreEqual(10, end);
    }
}