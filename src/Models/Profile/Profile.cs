using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using ReactiveUI;

namespace ChessBoom.Models.Profile;

public class Profile : ReactiveObject
{
    public string name { get; set; }
    public ObservableCollection<ObservableCollection<string>> _elo;
    public ObservableCollection<ObservableCollection<string>> Elo // elo, difference, pixels, date 
    {
        get => _elo;
        set => this.RaiseAndSetIfChanged(ref _elo, value);
    }
    public Queue<string[]> eloQueue { get; set; }
    public int _graphMaxElo;
    public int GraphMaxElo
    {
        get => _graphMaxElo;
        set => this.RaiseAndSetIfChanged(ref _graphMaxElo, value);
    }
    public int _graphMinElo;
    public int GraphMinElo
    {
        get => _graphMinElo;
        set => this.RaiseAndSetIfChanged(ref _graphMinElo, value);
    }
    public IList<Point> _points;
    public IList<Point> Points
    {
        get => _points;
        set => this.RaiseAndSetIfChanged(ref _points, value);
    }
    public Point _graphStartTrend;
    public Point GraphStartTrend
    {
        get => _graphStartTrend;
        set => this.RaiseAndSetIfChanged(ref _graphStartTrend, value);
    }
    public Point _graphEndTrend;
    public Point GraphEndTrend
    {
        get => _graphEndTrend;
        set => this.RaiseAndSetIfChanged(ref _graphEndTrend, value);
    }
    double _winRateWhite;
    public string WinRateWhite
    {
        get { return (_winRateWhite > minSize) ? ((int)Math.Ceiling(_winRateWhite * 100)).ToString() + "%" : ""; }
        set => this.RaiseAndSetIfChanged(ref _winRateWhite, double.Parse(value));
    }
    double _winRateBlack;
    public string WinRateBlack
    {
        get { return (_winRateBlack > minSize) ? ((int)Math.Ceiling(_winRateBlack * 100)).ToString() + "%" : ""; }
        set => this.RaiseAndSetIfChanged(ref _winRateBlack, double.Parse(value));
    }
    double _lossRateWhite;
    public string LossRateWhite
    {
        get { return (_lossRateWhite > minSize) ? ((int)Math.Ceiling(_lossRateWhite * 100)).ToString() + "%" : ""; }
        set => this.RaiseAndSetIfChanged(ref _lossRateWhite, double.Parse(value));
    }
    double _lossRateBlack;
    public string LossRateBlack
    {
        get { return (_lossRateBlack > minSize) ? ((int)Math.Ceiling(_lossRateBlack * 100)).ToString() + "%" : ""; }
        set => this.RaiseAndSetIfChanged(ref _lossRateBlack, double.Parse(value));
    }
    double _drawRateWhite;
    public string DrawRateWhite
    {
        get { return (_drawRateWhite > minSize) ? ((int)Math.Ceiling(_drawRateWhite * 100)).ToString() + "%" : ""; }
        set => this.RaiseAndSetIfChanged(ref _drawRateWhite, double.Parse(value));
    }
    public double _drawRateBlack;
    public string DrawRateBlack
    {
        get { return (_drawRateBlack > minSize) ? ((int)Math.Ceiling(_drawRateBlack * 100)).ToString() + "%" : ""; }
        set => this.RaiseAndSetIfChanged(ref _drawRateBlack, double.Parse(value));
    }
    public ObservableCollection<int> _barGraphData;
    public ObservableCollection<int> BarGraphData
    {
        get => _barGraphData;
        set => this.RaiseAndSetIfChanged(ref _barGraphData, value);
    }
    public List<Dictionary<string, string>> games { get; set; }
    public int _totalGames;
    public int TotalGames
    {
        get => _totalGames;
        set => this.RaiseAndSetIfChanged(ref _totalGames, value);
    }
    public int _wins;
    public int Wins
    {
        get => _wins;
        set => this.RaiseAndSetIfChanged(ref _wins, value);
    }
    public int _losses;
    public int Losses
    {
        get => _losses;
        set => this.RaiseAndSetIfChanged(ref _losses, value);
    }
    public int _draws;
    public int Draws
    {
        get => _draws;
        set => this.RaiseAndSetIfChanged(ref _draws, value);
    }
    public ObservableCollection<int> _whiteBars;
    public ObservableCollection<int> WhiteBars
    {
        get => _whiteBars;
        set => this.RaiseAndSetIfChanged(ref _whiteBars, value);
    }
    public ObservableCollection<int> _blackBars;
    public ObservableCollection<int> BlackBars
    {
        get => _blackBars;
        set => this.RaiseAndSetIfChanged(ref _blackBars, value);
    }
    public Dictionary<string, int> openings {get; set;}
    public string _mostUsedOpening;
    public string MostUsedOpening
    {
        get => _mostUsedOpening;
        set => this.RaiseAndSetIfChanged(ref _mostUsedOpening, value);
    }

    public readonly int barWidth = 600;
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
        this.games = new List<Dictionary<string, string>>();
        this._elo = new ObservableCollection<ObservableCollection<string>>();
        this.eloQueue = new Queue<string[]>();
        this._winRateWhite = 0;
        this._winRateBlack = 0;
        this._lossRateWhite = 0;
        this._lossRateBlack = 0;
        this._drawRateWhite = 0;
        this._drawRateBlack = 0;
        this._totalGames = 0;
        this._barGraphData = new ObservableCollection<int>();
        this._whiteBars = new ObservableCollection<int>();
        this._blackBars = new ObservableCollection<int>();
        this._points = new List<Point>();
        this.openings = new Dictionary<string, int>();
        this._mostUsedOpening = "";
        this._wins=0;
        this._losses=0;
        this._draws=0;
        this._graphMaxElo=0;
        this._graphMinElo=0;
    }

    public Profile(string name)
    {
        this.name = name;
        this.games = new List<Dictionary<string, string>>();
        this._elo = new ObservableCollection<ObservableCollection<string>>();
        this.eloQueue = new Queue<string[]>();
        this._winRateWhite = 0;
        this._winRateBlack = 0;
        this._lossRateWhite = 0;
        this._lossRateBlack = 0;
        this._drawRateWhite = 0;
        this._drawRateBlack = 0;
        this._totalGames = 0;
        this._barGraphData = new ObservableCollection<int>();
        this._whiteBars = new ObservableCollection<int>();
        this._blackBars = new ObservableCollection<int>();
        this._points = new List<Point>();
        this.openings = new Dictionary<string, int>();
        this._mostUsedOpening = "";
        this._wins=0;
        this._losses=0;
        this._draws=0;
        this._graphMaxElo=0;
        this._graphMinElo=0;
    }

    public void ResetStats()
    {        
        this.Elo.Clear();
        this.eloQueue = new Queue<string[]>();
        this.WinRateWhite = "0";
        this.WinRateBlack = "0";
        this.LossRateWhite = "0";
        this.LossRateBlack = "0";
        this.DrawRateWhite = "0";
        this.DrawRateBlack = "0";
        this.TotalGames = 0;
        this.BarGraphData.Clear();
        this.WhiteBars.Clear();
        this.BlackBars.Clear();
        this.Points = new List<Point>();
        this.openings = new Dictionary<string, int>();
        this.MostUsedOpening = "";
        this.Wins=0;
        this.Losses=0;
        this.Draws=0;
        this.GraphMaxElo=0;
        this.GraphMinElo=0;
    }

    public void addGame(Dictionary<string, string> game)
    {
        games.Add(game);
    }

    public void calculateStats(string variant = "")
    {
        ResetStats();
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

        //Most Used Opening
        this.MostUsedOpening = openings.OrderByDescending(x => x.Value).First().Key;

        // solves division by 0
        if (whiteGames == 0) whiteGames = 1;
        if (blackGames == 0) blackGames = 1;
        if (totalGames == 0) totalGames = 1;

        // Results %
        WinRateWhite = Math.Round((double)whiteWins / whiteGames, 2).ToString();
        WinRateBlack = Math.Round((double)blackWins / blackGames, 2).ToString();
        LossRateWhite = Math.Round((double)whiteLosses / whiteGames, 2).ToString();
        LossRateBlack = Math.Round((double)blackLosses / blackGames, 2).ToString();
        DrawRateWhite = Math.Round((double)whiteDraws / whiteGames, 2).ToString();
        DrawRateBlack = Math.Round((double)blackDraws / blackGames, 2).ToString();

        // Black/White Win Ratio Bars
        WhiteBars.Add((int)Math.Ceiling(_winRateWhite * barWidth));
        WhiteBars.Add((int)Math.Ceiling(_lossRateWhite * barWidth));
        WhiteBars.Add(barWidth - WhiteBars[0] - WhiteBars[1]);

        BlackBars.Add((int)Math.Ceiling(_winRateBlack * barWidth));
        BlackBars.Add((int)Math.Ceiling(_lossRateBlack * barWidth));
        BlackBars.Add(barWidth - BlackBars[0] - BlackBars[1]);

        // Results numbers
        wins = whiteWins + blackWins;
        this.Wins = wins;
        losses = whiteLosses + blackLosses;
        this.Losses = losses;
        draws = whiteDraws + blackDraws;
        this.Draws = draws;

        // BarGraph Bars
        BarGraphData.Add((int)(((double)wins / totalGames) * barHeight));
        BarGraphData.Add((int)(((double)losses / totalGames) * barHeight));
        BarGraphData.Add(barHeight - BarGraphData[0] - BarGraphData[1]);

        // Elo Graph
        transformQueueToArray();
        calculateTrendLine();
    }

    public void calculateTrendLine()
    {
        var yAxisValues = new List<int>();
        var xAxisValues = new List<int>();

        for (int i = 0; i < this.maxGames; i++)
        {
            yAxisValues.Add(int.Parse(this.Elo[i][0]));
            xAxisValues.Add(i * 50 + 50);
        }

        Trendline treadline = new Trendline(yAxisValues, xAxisValues);
        this.GraphStartTrend = new Point(50, convertEloToPixels(treadline.Start));
        this.GraphEndTrend = new Point(this.maxGames*50, convertEloToPixels(treadline.End));
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

        for (int i = this.lastEloGames; i >= 0; i--)
        {
            ObservableCollection<string> eloInfo;
            if(eloQueue.Count() == 0){
                eloInfo = new ObservableCollection<string>{"", "", "1000", ""};
                this.Elo.Add(eloInfo);
                continue;
            }
            info = eloQueue.Dequeue();
            currentElo = int.Parse(info[0]);
            if (currentElo > max) max = currentElo;
            if (currentElo < min) min = currentElo;
            difference = (i == (this.maxGames - 1)) ? 0 : (currentElo - lastElo);
            diff = (difference > 0) ? "+" + difference.ToString() : difference.ToString();
            lastElo = currentElo;
            
            eloInfo = new ObservableCollection<string>{info[0], diff, "", info[1]};
            this.Elo.Insert(0, eloInfo);
        }

        this.GraphMaxElo = (max + 150) / 100 * 100;
        this.GraphMinElo = (min - 150) / 100 * 100;
        if (this.GraphMaxElo < 0) this.GraphMaxElo = 0;
        if (this.GraphMinElo < 0) this.GraphMinElo = 0;

        for (int i = 0; i < this.maxGames; i++)
        {
            pixels = convertEloToPixels(int.Parse(Elo[i][0]));
            this.Elo[i][2] = pixels.ToString();
            Points.Add(new Point(i * 50 + 50, pixels + 5));
        }
    }

    public int convertEloToPixels(int elo)
    {
        double graphPosition = 0;
        int pixels = 0;
        int range = (this.GraphMaxElo - this.GraphMinElo);
        graphPosition = ((double)(elo - this.GraphMinElo)) / range;
        pixels = eloGraphHeight - (int)(graphPosition * eloGraphHeight) + 30;
        return pixels;
    }
}