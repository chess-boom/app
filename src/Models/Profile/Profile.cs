using System;
using System.Collections.Generic;
using ReactiveUI;

namespace ChessBoom.Models.Profile;

public class Profile{
    public BarGraph barGraph { get; set; }
    public string name { get; set; }
    public List<int> elo { get; set; }
    public double winRateWhite { get; set; }
    public double winRateBlack { get; set; }
    public double lossRateWhite { get; set; }
    public double lossRateBlack { get; set; }
    public double drawRateWhite { get; set; }
    public double drawRateBlack { get; set; }
    public List<int> barGraphData { get; set; }
    public List<Dictionary<string, string>> games { get; set; }
    public int totalGames { get; set; }
    public Profile(){
        this.name = "Default";
        this.elo = new List<int>();
        this.winRateWhite = 0;
        this.winRateBlack = 0;
        this.lossRateWhite = 0;
        this.lossRateBlack = 0;
        this.drawRateWhite = 0;
        this.drawRateBlack = 0;
        this.totalGames = 0;
        this.games = new List<Dictionary<string, string>>();
        this.barGraphData = new List<int>();
        this.barGraph = new BarGraph();
    }
    public Profile(string name){
        this.name = name;
        this.elo = new List<int>();
        this.winRateWhite = 0;
        this.winRateBlack = 0;
        this.lossRateWhite = 0;
        this.lossRateBlack = 0;
        this.drawRateWhite = 0;
        this.drawRateBlack = 0;
        this.totalGames = 110;
        this.games = new List<Dictionary<string, string>>();
        this.barGraphData = new List<int>();
        this.barGraph = new BarGraph();
    }

    public void addGame(Dictionary<string, string> game){
        games.Add(game);
    }

    public void calculateStats(string variant = ""){
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
        foreach (Dictionary<string, string> game in games){
            //calculate stats only for given variant, if none defined calculate for all games
            if(variant != ""){
                if(variant != game["variant"]) continue;
            }
            totalGames++;
            if(game["White"] == name){
                if (game["Result"] == "1-0"){
                    whiteWins++;
                } else if (game["Result"] == "0-1"){
                    whiteLosses++;
                } else {
                    whiteDraws++;
                }
                addElo(int.Parse(game["WhiteElo"]));
                whiteGames++;
            } else {
                if (game["Result"] == "1-0"){
                    blackLosses++;
                } else if (game["Result"] == "0-1"){
                    blackWins++;
                } else {
                    blackDraws++;
                }
                addElo(int.Parse(game["BlackElo"]));
                blackGames++;
            }
        }
        //solves division by 0
        if(whiteGames == 0) whiteGames = 1;
        if(blackGames == 0) blackGames = 1;

        winRateWhite = Math.Round((double)whiteWins / whiteGames, 2);
        winRateBlack = Math.Round((double)blackWins / blackGames, 2);
        lossRateWhite = Math.Round((double)whiteLosses / whiteGames, 2);
        lossRateBlack = Math.Round((double)blackLosses / blackGames, 2);
        drawRateWhite = Math.Round((double)whiteDraws / whiteGames, 2);
        drawRateBlack = Math.Round((double)blackDraws / blackGames, 2);
        wins = whiteWins + blackWins;
        losses = whiteLosses + blackLosses;
        draws = whiteDraws + blackDraws;
        this.totalGames = totalGames;
    }

    public void addElo(int elo){
        this.elo.Add(elo);
    }

    public string getEloHistory(){
        string history = "";
        foreach (int e in elo){
            history += "[" + e + "] ";
        }
        return history;
    }

    public void displayProfileStats(){
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