using System.Collections.ObjectModel;
using Avalonia;
using ChessBoom.Models.Game;
using ChessBoom.Models.Profile;
using NUnit.Framework;

namespace ChessBoom.NUnitTests.ProfileTests;

public class ProfileUnitTests
{
    private Profile profile = null!;
    string[] files =
    {"../CBoom/game1.pgn",  //standard
    "../CBoom/game8.pgn",   //atomic
    "../CBoom/game11.pgn",  //horde
    "../CBoom/game17.pgn"}; //Chess960


    [SetUp]
    public void Setup()
    {
        string username = "MatteoGisondi";
        profile = new Profile(username);
        foreach (string file in files)
        {
            Dictionary<string, string> game = GameHandler.ReadPGN(file);
            profile.AddGame(game);
        }
    }

    /// <summary>
    /// Check the slope of the trendline is correct
    /// </summary>
    [Test]
    public void CheckCalculateStatsAllGamesTest()
    {
        profile.CalculateStats();
        Assert.AreEqual(4, profile.TotalGames);
        Assert.AreEqual(1700, profile.GraphMaxElo);
        Assert.AreEqual(1200, profile.GraphMinElo);
        Assert.AreEqual(new Point(50, 85), profile.GraphStartTrend);
        Assert.AreEqual(new Point(200, 270), profile.GraphEndTrend);
        Assert.AreEqual("100%", profile.WinRateWhite);
        Assert.AreEqual("", profile.WinRateBlack);
        Assert.AreEqual("", profile.LossRateWhite);
        Assert.AreEqual("100%", profile.LossRateBlack);
        Assert.AreEqual("", profile.DrawRateWhite);
        Assert.AreEqual("", profile.DrawRateBlack);
        Assert.AreEqual(2, profile.Wins);
        Assert.AreEqual(2, profile.Losses);
        Assert.AreEqual(0, profile.Draws);
        Assert.AreEqual("?", profile.MostUsedOpening);
    }
}