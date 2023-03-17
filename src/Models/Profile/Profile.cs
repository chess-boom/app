using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using ReactiveUI;

namespace ChessBoom.Models.Profile;

public class Profile : ReactiveObject
{
    public string name { get; set; }
    public string[][] elo { get; set; } // elo, difference, pixels, date    
    public Queue<string[]> eloQueue { get; set; }
    public int graphMaxElo { get; set; }
    public int graphMinElo { get; set; }
    public IList<Point> points { get; set; }
    public Point graphStartTrend { get; set; }
    public Point graphEndTrend { get; set; }
    double _winRateWhite;

    public string winRateWhite
    {
        get { return (_winRateWhite > minSize) ? ((int)Math.Ceiling(_winRateWhite * 100)).ToString() + "%" : ""; }
        set { _winRateWhite = double.Parse(value); }
    }

    double _winRateBlack;

    public string winRateBlack
    {
        get { return (_winRateBlack > minSize) ? ((int)Math.Ceiling(_winRateBlack * 100)).ToString() + "%" : ""; }
        set { _winRateBlack = double.Parse(value); }
    }

    double _lossRateWhite;

    public string lossRateWhite
    {
        get { return (_lossRateWhite > minSize) ? ((int)Math.Ceiling(_lossRateWhite * 100)).ToString() + "%" : ""; }
        set { _lossRateWhite = double.Parse(value); }
    }

    double _lossRateBlack;

    public string lossRateBlack
    {
        get { return (_lossRateBlack > minSize) ? ((int)Math.Ceiling(_lossRateBlack * 100)).ToString() + "%" : ""; }
        set { _lossRateBlack = double.Parse(value); }
    }

    double _drawRateWhite;

    public string drawRateWhite
    {
        get { return (_drawRateWhite > minSize) ? ((int)Math.Ceiling(_drawRateWhite * 100)).ToString() + "%" : ""; }
        set { _drawRateWhite = double.Parse(value); }
    }

    double _drawRateBlack;

    public string drawRateBlack
    {
        get { return (_drawRateBlack > minSize) ? ((int)Math.Ceiling(_drawRateBlack * 100)).ToString() + "%" : ""; }
        set { _drawRateBlack = double.Parse(value); }
    }

    public int[] barGraphData { get; set; }
    public List<Dictionary<string, string>> games { get; set; }
    public int _totalGames;

    public int TotalGames
    {
        get => _totalGames;
        set => this.RaiseAndSetIfChanged(ref _totalGames, value);
    }

    public int wins { get; set; }
    public int losses { get; set; }
    public int draws { get; set; }
    public int[] whiteBars { get; set; }
    public int[] blackBars { get; set; }
    Dictionary<string, int> openings;

    public string mostUsedOpening
    {
        get { return openings.OrderByDescending(x => x.Value).First().Key; }
        set { mostUsedOpening = value; }
    }

    public readonly int barWidth = 500;
    public readonly int barHeight = 200;
    public readonly int eloGraphHeight = 400;
    public readonly double minSize = 0.15;
    public readonly int lastEloGames = 10;

    public int maxGames
    {
        get { return Math.Min(lastEloGames, _totalGames); }
    }

    public Profile()
    {
        this.name = "Default";
        this.elo = new string[this.lastEloGames][];
        this.eloQueue = new Queue<string[]>();
        this.winRateWhite = "0";
        this.winRateBlack = "0";
        this.lossRateWhite = "0";
        this.lossRateBlack = "0";
        this.drawRateWhite = "0";
        this.drawRateBlack = "0";
        this._totalGames = 0;
        this.games = new List<Dictionary<string, string>>();
        this.barGraphData = new int[3];
        this.whiteBars = new int[3];
        this.blackBars = new int[3];
        this.points = new List<Point>();
        this.openings = new Dictionary<string, int>();
    }

    public Profile(string name)
    {
        this.name = name;
        this.elo = new string[this.lastEloGames][];
        this.eloQueue = new Queue<string[]>();
        this.winRateWhite = "0";
        this.winRateBlack = "0";
        this.lossRateWhite = "0";
        this.lossRateBlack = "0";
        this.drawRateWhite = "0";
        this.drawRateBlack = "0";
        this._totalGames = 0;
        this.games = new List<Dictionary<string, string>>();
        this.barGraphData = new int[3];
        this.whiteBars = new int[3];
        this.blackBars = new int[3];
        this.points = new List<Point>();
        this.openings = new Dictionary<string, int>();
    }

    public void addGame(Dictionary<string, string> game)
    {
        games.Add(game);
    }

    public void calculateStats(string variant = "")
    {
        int totalGames = 0;
        int whiteWins = 0;
        int whiteLosses = 0;
        int blackWins = 0;
        int blackLosses = 0;
        int whiteDraws = 0;
        int blackDraws = 0;
        int whiteGames = 0;
        int blackGames = 0;
        int wins = 0;
        int losses = 0;
        int draws = 0;
        foreach (Dictionary<string, string> game in games)
        {
            //calculate stats only for given variant, if none defined calculate for all games
            if (variant != "")
            {
                if (variant != game["Variant"]) continue;
            }

            string opening = game["Opening"];
            if (openings.ContainsKey(opening))
            {
                openings[opening]++;
            }
            else
            {
                openings.Add(opening, 0);
            }

            totalGames++;
            if (game["White"] == name)
            {
                if (game["Result"] == "1-0")
                {
                    whiteWins++;
                }
                else if (game["Result"] == "0-1")
                {
                    whiteLosses++;
                }
                else
                {
                    whiteDraws++;
                }

                addElo(game["WhiteElo"], game["Date"]);
                whiteGames++;
            }
            else
            {
                if (game["Result"] == "1-0")
                {
                    blackLosses++;
                }
                else if (game["Result"] == "0-1")
                {
                    blackWins++;
                }
                else
                {
                    blackDraws++;
                }

                addElo(game["BlackElo"], game["Date"]);
                blackGames++;
            }
        }

        //Total Games
        this.TotalGames = totalGames;

        // solves division by 0
        if (whiteGames == 0) whiteGames = 1;
        if (blackGames == 0) blackGames = 1;
        if (totalGames == 0) totalGames = 1;

        // Results %
        _winRateWhite = Math.Round((double)whiteWins / whiteGames, 2);
        _winRateBlack = Math.Round((double)blackWins / blackGames, 2);
        _lossRateWhite = Math.Round((double)whiteLosses / whiteGames, 2);
        _lossRateBlack = Math.Round((double)blackLosses / blackGames, 2);
        _drawRateWhite = Math.Round((double)whiteDraws / whiteGames, 2);
        _drawRateBlack = Math.Round((double)blackDraws / blackGames, 2);

        // Total Games
        this.TotalGames = totalGames;

        // Black/White Win Ratio Bars
        whiteBars[0] = (int)Math.Ceiling(_winRateWhite * barWidth);
        whiteBars[1] = (int)Math.Ceiling(_lossRateWhite * barWidth);
        whiteBars[2] = barWidth - whiteBars[0] - whiteBars[1];

        blackBars[0] = (int)Math.Ceiling(_winRateBlack * barWidth);
        blackBars[1] = (int)Math.Ceiling(_lossRateBlack * barWidth);
        blackBars[2] = barWidth - blackBars[0] - blackBars[1];

        // Results numbers
        wins = whiteWins + blackWins;
        this.wins = wins;
        losses = whiteLosses + blackLosses;
        this.losses = losses;
        draws = whiteDraws + blackDraws;
        this.draws = draws;

        // BarGraph Bars
        barGraphData[0] = (int)(((double)wins / totalGames) * barHeight);
        barGraphData[1] = (int)(((double)losses / totalGames) * barHeight);
        barGraphData[2] = barHeight - barGraphData[0] - barGraphData[1];

        // Elo Graph
        transformQueueToArray();
        calculateTrendLine();
    }

    public void calculateTrendLine()
    {
        var yAxisValues = new List<int>();
        var xAxisValues = new List<int>();

        for (int i = 0; i < this.lastEloGames; i++)
        {
            yAxisValues.Add(int.Parse(this.elo[i][0]));
            xAxisValues.Add(i * 50 + 50);
        }

        Trendline treadline = new Trendline(yAxisValues, xAxisValues);
        this.graphStartTrend = new Point(50, convertEloToPixels(treadline.Start));
        this.graphEndTrend = new Point(500, convertEloToPixels(treadline.End));
    }

    public void addElo(string elo, string date)
    {
        if (this.eloQueue.Count == this.lastEloGames)
        {
            this.eloQueue.Dequeue();
        }

        string[] info = { elo, date };
        this.eloQueue.Enqueue(info);
    }

    public void transformQueueToArray()
    {
        int difference = 0;
        string diff = "";
        string[] info;
        int currentElo = 0;
        int lastElo = 0;
        int max = 0;
        int min = int.MaxValue;
        int pixels = 0;

        for (int i = this.maxGames - 1; i >= 0; i--)
        {
            info = eloQueue.Dequeue();
            currentElo = int.Parse(info[0]);
            if (currentElo > max) max = currentElo;
            if (currentElo < min) min = currentElo;
            difference = (i == (this.maxGames - 1)) ? 0 : (currentElo - lastElo);
            diff = (difference > 0) ? "+" + difference.ToString() : difference.ToString();
            lastElo = currentElo;
            string[] eloInfo = { info[0], diff, "", info[1] };
            this.elo[i] = eloInfo;
        }

        this.graphMaxElo = (max + 150) / 100 * 100;
        this.graphMinElo = (min - 150) / 100 * 100;
        if (this.graphMaxElo < 0) this.graphMaxElo = 0;
        if (this.graphMinElo < 0) this.graphMinElo = 0;

        for (int i = 0; i < this.maxGames; i++)
        {
            pixels = convertEloToPixels(int.Parse(elo[i][0]));
            this.elo[i][2] = pixels.ToString();
            points.Add(new Point(i * 50 + 50, pixels + 5));
        }
    }

    public int convertEloToPixels(int elo)
    {
        double graphPosition = 0;
        int pixels = 0;
        int range = (this.graphMaxElo - this.graphMinElo);
        graphPosition = ((double)(elo - this.graphMinElo)) / range;
        pixels = eloGraphHeight - (int)(graphPosition * eloGraphHeight) + 30;
        return pixels;
    }

    public string getEloHistory()
    {
        string history = "";
        for (int i = 0; i < this.maxGames; i++)
        {
            history += "[" + elo[i][0] + "] ";
        }

        return history;
    }

    public void displayProfileStats()
    {
        string stats = "Name: " + name + "\n";
        stats += "Elo: " + getEloHistory() + "\n";
        stats += "Win Rate White: " + winRateWhite + "\n";
        stats += "Win Rate Black: " + winRateBlack + "\n";
        stats += "Loss Rate White: " + lossRateWhite + "\n";
        stats += "Loss Rate Black: " + lossRateBlack + "\n";
        stats += "Draw Rate White: " + drawRateWhite + "\n";
        stats += "Draw Rate Black: " + drawRateBlack + "\n";
        Console.WriteLine(stats);
    }
}