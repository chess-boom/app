using System;
using System.Collections.Generic;

namespace ChessBoom.Models;

public class Profile{
    public string name { get; set; }
    public List<int> elo { get; set; }
    public double winRateWhite { get; set; }
    public double winRateBlack { get; set; }
    public double lossRateWhite { get; set; }
    public double lossRateBlack { get; set; }
    public double drawRateWhite { get; set; }
    public double drawRateBlack { get; set; }
    public List<Dictionary<string, string>> games { get; set; }
    public Profile(){
        this.name = "Default";
        this.elo = new List<int>();
        this.winRateWhite = 0;
        this.winRateBlack = 0;
        this.lossRateWhite = 0;
        this.lossRateBlack = 0;
        this.drawRateWhite = 0;
        this.drawRateBlack = 0;
        this.games = new List<Dictionary<string, string>>();
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
        this.games = new List<Dictionary<string, string>>();
    }

    public void addGame(Dictionary<string, string> game){
        games.Add(game);
    }

    public void calculateStats(){
        int whiteWins = 0;
        int whiteLosses = 0;
        int blackWins = 0;
        int blackLosses = 0;
        int whiteDraws = 0;
        int blackDraws = 0;
        int whiteGames = 0;
        int blackGames = 0;
        foreach (Dictionary<string, string> game in games){
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
        winRateWhite = Math.Round((double)whiteWins / whiteGames, 2);
        winRateBlack = Math.Round((double)blackWins / blackGames, 2);
        lossRateWhite = Math.Round((double)whiteLosses / whiteGames, 2);
        lossRateBlack = Math.Round((double)blackLosses / blackGames, 2);
        drawRateWhite = Math.Round((double)whiteDraws / whiteGames, 2);
        drawRateBlack = Math.Round((double)blackDraws / blackGames, 2);
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