using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia;
using ReactiveUI;

namespace ChessBoom.Models.Profile;

public class Profile : ReactiveObject
{
    public string Name { get; set;}

    private readonly ObservableCollection<ObservableCollection<string>> _elo;

    // elo, difference, pixels, date 
    public ObservableCollection<ObservableCollection<string>> Elo
    {
        get => _elo;
    }

    public Queue<string[]> EloQueue { get; set; }

    private int _graphMaxElo;

    public int GraphMaxElo
    {
        get => _graphMaxElo;
        set => this.RaiseAndSetIfChanged(ref _graphMaxElo, value);
    }

    private int _graphMinElo;

    public int GraphMinElo
    {
        get => _graphMinElo;
        set => this.RaiseAndSetIfChanged(ref _graphMinElo, value);
    }

    private IList<Point> _points;

    public IList<Point> Points
    {
        get => _points;
        set => this.RaiseAndSetIfChanged(ref _points, value);
    }

    private Point _graphStartTrend;

    public Point GraphStartTrend
    {
        get => _graphStartTrend;
        set => this.RaiseAndSetIfChanged(ref _graphStartTrend, value);
    }

    private Point _graphEndTrend;

    public Point GraphEndTrend
    {
        get => _graphEndTrend;
        set => this.RaiseAndSetIfChanged(ref _graphEndTrend, value);
    }

    private double _winRateWhite;

    public string WinRateWhite
    {
        get => _winRateWhite > MinSize ? (int)Math.Ceiling(_winRateWhite * 100) + "%" : "";
        set => this.RaiseAndSetIfChanged(ref _winRateWhite, double.Parse(value));
    }

    private double _winRateBlack;

    public string WinRateBlack
    {
        get => _winRateBlack > MinSize ? (int)Math.Ceiling(_winRateBlack * 100) + "%" : "";
        set => this.RaiseAndSetIfChanged(ref _winRateBlack, double.Parse(value));
    }

    private double _lossRateWhite;

    public string LossRateWhite
    {
        get => _lossRateWhite > MinSize ? (int)Math.Ceiling(_lossRateWhite * 100) + "%" : "";
        set => this.RaiseAndSetIfChanged(ref _lossRateWhite, double.Parse(value));
    }

    private double _lossRateBlack;

    public string LossRateBlack
    {
        get => _lossRateBlack > MinSize ? (int)Math.Ceiling(_lossRateBlack * 100) + "%" : "";
        set => this.RaiseAndSetIfChanged(ref _lossRateBlack, double.Parse(value));
    }

    private double _drawRateWhite;

    public string DrawRateWhite
    {
        get => _drawRateWhite > MinSize ? (int)Math.Ceiling(_drawRateWhite * 100) + "%" : "";
        set => this.RaiseAndSetIfChanged(ref _drawRateWhite, double.Parse(value));
    }

    private double _drawRateBlack;

    public string DrawRateBlack
    {
        get => _drawRateBlack > MinSize ? (int)Math.Ceiling(_drawRateBlack * 100) + "%" : "";
        set => this.RaiseAndSetIfChanged(ref _drawRateBlack, double.Parse(value));
    }

    private ObservableCollection<int> _barGraphData;

    public ObservableCollection<int> BarGraphData
    {
        get => _barGraphData;
        set => this.RaiseAndSetIfChanged(ref _barGraphData, value);
    }

    private List<Dictionary<string, string>> Games { get; }
    private int _totalGames;

    public int TotalGames
    {
        get => _totalGames;
        set => this.RaiseAndSetIfChanged(ref _totalGames, value);
    }

    private int _wins;

    public int Wins
    {
        get => _wins;
        set => this.RaiseAndSetIfChanged(ref _wins, value);
    }

    private int _losses;

    public int Losses
    {
        get => _losses;
        set => this.RaiseAndSetIfChanged(ref _losses, value);
    }

    private int _draws;

    public int Draws
    {
        get => _draws;
        set => this.RaiseAndSetIfChanged(ref _draws, value);
    }

    private readonly ObservableCollection<int> _whiteBars;

    public ObservableCollection<int> WhiteBars
    {
        get => _whiteBars;
    }

    private readonly ObservableCollection<int> _blackBars;

    public ObservableCollection<int> BlackBars
    {
        get => _blackBars;
    }

    private Dictionary<string, int> Openings { get; set; }

    private string _mostUsedOpening;

    public string MostUsedOpening
    {
        get => _mostUsedOpening;
        set => this.RaiseAndSetIfChanged(ref _mostUsedOpening, value);
    }

    private const int BarWidth = 600;
    private const int BarHeight = 200;
    private const int EloGraphHeight = 400;
    private const double MinSize = 0.15;
    private const int LastEloGames = 10;

    private int MaxGames => Math.Min(LastEloGames, _totalGames);

    public Profile(string name)
    {
        Name = name;
        Games = new List<Dictionary<string, string>>();
        _elo = new ObservableCollection<ObservableCollection<string>>();
        for(int i = 0; i < 10; i++)
        {
            _elo.Add(new ObservableCollection<string> {"0","0","1000","0"});
        }
        EloQueue = new Queue<string[]>();
        _winRateWhite = 0;
        _winRateBlack = 0;
        _lossRateWhite = 0;
        _lossRateBlack = 0;
        _drawRateWhite = 0;
        _drawRateBlack = 0;
        _totalGames = 0;
        _barGraphData = new ObservableCollection<int>();
        _whiteBars = new ObservableCollection<int>();
        _blackBars = new ObservableCollection<int>();
        _points = new List<Point>();
        Openings = new Dictionary<string, int>();
        _mostUsedOpening = "";
        _wins = 0;
        _losses = 0;
        _draws = 0;
        _graphMaxElo = 0;
        _graphMinElo = 0;
    }

    private void ResetStats()
    {
        Elo.Clear();
        EloQueue = new Queue<string[]>();
        WinRateWhite = "0";
        WinRateBlack = "0";
        LossRateWhite = "0";
        LossRateBlack = "0";
        DrawRateWhite = "0";
        DrawRateBlack = "0";
        TotalGames = 0;
        BarGraphData.Clear();
        WhiteBars.Clear();
        BlackBars.Clear();
        Points = new List<Point>();
        Openings = new Dictionary<string, int>();
        MostUsedOpening = "";
        Wins = 0;
        Losses = 0;
        Draws = 0;
        GraphMaxElo = 0;
        GraphMinElo = 0;
    }

    public void AddGame(Dictionary<string, string> game)
    {
        Games.Add(game);
    }

    public void ClearGames()
    {
        Games.Clear();
    }

    public void CalculateStats(string variant = "")
    {
        ResetStats();
        var totalGames = 0;
        var whiteWins = 0;
        var whiteLosses = 0;
        var blackWins = 0;
        var blackLosses = 0;
        var whiteDraws = 0;
        var blackDraws = 0;
        var whiteGames = 0;
        var blackGames = 0;
        foreach (var game in Games)
        {
            // calculate stats only for given variant, if none defined calculate for all games
            if (variant != "" && variant != game["Variant"])
            {
                continue;
            }

            if (game["White"] == Name)
            {
                switch (game["Result"])
                {
                    case "1-0":
                        whiteWins++;
                        break;
                    case "0-1":
                        whiteLosses++;
                        break;
                    default:
                        whiteDraws++;
                        break;
                }

                AddElo(game["WhiteElo"], game["Date"]);
                whiteGames++;
            }
            else if (game["Black"] == Name)
            {
                switch (game["Result"])
                {
                    case "1-0":
                        blackLosses++;
                        break;
                    case "0-1":
                        blackWins++;
                        break;
                    default:
                        blackDraws++;
                        break;
                }

                AddElo(game["BlackElo"], game["Date"]);
                blackGames++;
            }
            else
            {
                // if game is not played by this profile, skip it
                continue;
            }

            var opening = game["Opening"];
            if (Openings.ContainsKey(opening))
            {
                Openings[opening]++;
            }
            else
            {
                Openings.Add(opening, 0);
            }
            totalGames++;
        }

        //Total Games
        TotalGames = totalGames;

        //Most Used Opening
        if (TotalGames > 0)
        {
            MostUsedOpening = Openings.MaxBy(x => x.Value).Key;
        }

        // solves division by 0
        if (whiteGames == 0) whiteGames = 1;
        if (blackGames == 0) blackGames = 1;
        if (totalGames == 0) totalGames = 1;

        // Results %
        WinRateWhite = Math.Round((double)whiteWins / whiteGames, 2).ToString(CultureInfo.InvariantCulture);
        WinRateBlack = Math.Round((double)blackWins / blackGames, 2).ToString(CultureInfo.InvariantCulture);
        LossRateWhite = Math.Round((double)whiteLosses / whiteGames, 2).ToString(CultureInfo.InvariantCulture);
        LossRateBlack = Math.Round((double)blackLosses / blackGames, 2).ToString(CultureInfo.InvariantCulture);
        DrawRateWhite = Math.Round((double)whiteDraws / whiteGames, 2).ToString(CultureInfo.InvariantCulture);
        DrawRateBlack = Math.Round((double)blackDraws / blackGames, 2).ToString(CultureInfo.InvariantCulture);

        // Black/White Win Ratio Bars
        WhiteBars.Add((int)Math.Ceiling(_winRateWhite * BarWidth));
        WhiteBars.Add((int)Math.Ceiling(_lossRateWhite * BarWidth));
        WhiteBars.Add((int)Math.Ceiling(_drawRateWhite * BarWidth));

        BlackBars.Add((int)Math.Ceiling(_winRateBlack * BarWidth));
        BlackBars.Add((int)Math.Ceiling(_lossRateBlack * BarWidth));
        BlackBars.Add((int)Math.Ceiling(_drawRateBlack * BarWidth));

        // Results numbers
        Wins = whiteWins + blackWins;
        Losses = whiteLosses + blackLosses;
        Draws = whiteDraws + blackDraws;

        // BarGraph Bars
        BarGraphData.Add((int)((double)_wins / totalGames * BarHeight));
        BarGraphData.Add((int)((double)_losses / totalGames * BarHeight));
        BarGraphData.Add((int)((double)_draws / totalGames * BarHeight));

        // Elo Graph
        TransformQueueToArray();
        CalculateTrendLine();
    }

    private void CalculateTrendLine()
    {
        var yAxisValues = new List<int>();
        var xAxisValues = new List<int>();

        for (var i = 0; i < MaxGames; i++)
        {
            yAxisValues.Add(int.Parse(Elo[i][0]));
            xAxisValues.Add(i * 50 + 50);
        }

        var trendline = new Trendline(yAxisValues, xAxisValues);
        GraphStartTrend = new Point(50, ConvertEloToPixels(trendline.Start));
        GraphEndTrend = new Point(MaxGames * 50, ConvertEloToPixels(trendline.End));
    }

    private void AddElo(string elo, string date)
    {
        if (EloQueue.Count == LastEloGames)
        {
            EloQueue.Dequeue();
        }

        string[] info = { elo, date };
        EloQueue.Enqueue(info);
    }

    private void TransformQueueToArray()
    {
        var lastElo = 0;
        var max = 0;
        var min = int.MaxValue;

        for (var i = LastEloGames; i >= 0; i--)
        {
            if (!EloQueue.Any())
            {
                // elo, difference, pixels, date
                Elo.Add(new ObservableCollection<string> { "", "", "1000", "" });
                continue;
            }

            var info = EloQueue.Dequeue();
            var currentElo = int.Parse(info[0]);
            if (currentElo > max) max = currentElo;
            if (currentElo < min) min = currentElo;
            var difference = i == MaxGames - 1 ? 0 : currentElo - lastElo;
            var diff = difference > 0 ? "+" + difference : difference.ToString();
            lastElo = currentElo;

            // elo, difference, pixels, date
            Elo.Insert(0, new ObservableCollection<string> { info[0], diff, "", info[1] });
        }
        if (min == int.MaxValue)
        {
            min = 150;
        }

        GraphMaxElo = (max + 150) / 100 * 100;
        GraphMinElo = (min - 150) / 100 * 100;
        if (GraphMaxElo < 0) GraphMaxElo = 0;
        if (GraphMinElo < 0) GraphMinElo = 0;

        for (var i = 0; i < MaxGames; i++)
        {
            var pixels = ConvertEloToPixels(int.Parse(Elo[i][0]));
            Elo[i][2] = pixels.ToString();
            Points.Add(new Point(i * 50 + 50, pixels + 5));
        }
    }

    private int ConvertEloToPixels(int elo)
    {
        var range = GraphMaxElo - GraphMinElo;
        var graphPosition = (double)(elo - GraphMinElo) / range;
        return EloGraphHeight - (int)(graphPosition * EloGraphHeight) + 30;
    }
}