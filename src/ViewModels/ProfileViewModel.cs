using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Profile;
using ReactiveUI;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using ChessBoom.Models.Game;
using System.Reactive;
using System.Collections.ObjectModel;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class ProfileViewModel : BaseViewModel
{
    private Profile _profile;

    public Profile Profile
    {
        get => _profile;
        set => this.RaiseAndSetIfChanged(ref _profile, value);
    }

    private string _username;
    public string Username
    {
    get => _username;
    set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public ReactiveCommand<Unit, Unit> ParseAtomicGames { get; set; }
    public ReactiveCommand<Unit, Unit> ParseStandardGames { get; set; }
    public ReactiveCommand<Unit, Unit> ParseHordeGames { get; set; }
    public ReactiveCommand<Unit, Unit> ParseChess960Games { get; set; }
    public ReactiveCommand<Unit, Unit> ParseAllGames { get; set; }

    public ProfileViewModel(IScreen hostScreen) : base(hostScreen)
    {
        _username = "";
        _profile = new Profile(Username);
        ParseAtomicGames = ReactiveCommand.Create(ParseAtomicGamesCommand);
        ParseStandardGames = ReactiveCommand.Create(ParseStandardGamesCommand);
        ParseHordeGames = ReactiveCommand.Create(ParseHordeGamesCommand);
        ParseChess960Games = ReactiveCommand.Create(ParseChess960GamesCommand);
        ParseAllGames = ReactiveCommand.Create(ParseAllGamesCommand);
        this.WhenAnyValue(x => x.Profile)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Profile)));
    }

    public void InitializeProfile()
    {
        Profile.Name = Username;
        Profile.ClearGames();
        ParseGames();
    }

    private void ParseGames()
    {
        DirectoryInfo cboom = new DirectoryInfo(Path.Combine(System.AppContext.BaseDirectory, "CBoom"));
        if (!cboom.Exists)
        {
            cboom.Create();
        }
        //sorting files by creation time to get most recent games first
        FileInfo[] files = cboom.GetFiles("*.pgn").OrderByDescending(p => p.CreationTime).ToArray();

        foreach (String file in files.Select(filePath => filePath.FullName))
        {
            Dictionary<string, string> game = GameHandler.ReadPGN(file);
            Profile.AddGame(game);
        }

        Profile.CalculateStats();
    }
    private void ParseAllGamesCommand()
    {
        Profile.CalculateStats();
    }
    private void ParseAtomicGamesCommand()
    {
        Profile.CalculateStats("Atomic");
    }
    private void ParseStandardGamesCommand()
    {
        Profile.CalculateStats("Standard");
    }
    private void ParseHordeGamesCommand()
    {
        Profile.CalculateStats("Horde");
    }
    private void ParseChess960GamesCommand()
    {
        Profile.CalculateStats("Chess960");
    }
}