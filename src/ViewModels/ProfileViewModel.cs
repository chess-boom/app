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
    private ObservableCollection<int> _hi = new ObservableCollection<int>();

    public ObservableCollection<int> Hi
    {
        get => _hi;
        set => this.RaiseAndSetIfChanged(ref _hi, value);
    }

    public ReactiveCommand<Unit, Unit> ParseAtomicGames { get; set; }
    public ReactiveCommand<Unit, Unit> ParseStandardGames { get; set; }
    public ReactiveCommand<Unit, Unit> ParseHordeGames { get; set; }
    public ReactiveCommand<Unit, Unit> ParseChess360Games { get; set; }
    public ReactiveCommand<Unit, Unit> ParseAllGames { get; set; }

    public ProfileViewModel(IScreen hostScreen) : base(hostScreen)
    {
        var username = "MatteoGisondi";
        Hi.Add(0);
        Hi[0] = 0;
        _profile = new Profile(username);
        ParseGames();
        ParseAtomicGames = ReactiveCommand.Create(ParseAtomicGamesCommand);
        ParseStandardGames = ReactiveCommand.Create(ParseStandardGamesCommand);
        ParseHordeGames = ReactiveCommand.Create(ParseHordeGamesCommand);
        ParseChess360Games = ReactiveCommand.Create(ParseChess360GamesCommand);
        ParseAllGames = ReactiveCommand.Create(ParseAllGamesCommand);
        this.WhenAnyValue(x => x.Profile)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Profile)));
    }

    private void ParseGames()
    {
        DirectoryInfo cboom = new DirectoryInfo("../CBoom");

        //sorting files by creation time to get most recent games first
        FileInfo[] files = cboom.GetFiles("*.pgn").OrderByDescending(p => p.CreationTime).ToArray();

        foreach (FileInfo filePath in files)
        {
            String file = filePath.FullName;
            Dictionary<string, string> game = GameHandler.ReadPGN(file);
            Profile.addGame(game);
        }

        Profile.calculateStats();
    }
    private void ParseAllGamesCommand()
    {
        Profile.calculateStats();
        Hi[0] = 0;
    }
    private void ParseAtomicGamesCommand()
    {
        Profile.calculateStats("Atomic");
        Hi[0] = 1;
    }
    private void ParseStandardGamesCommand()
    {
        Profile.calculateStats("Standard");
        Hi[0] = 2;
    }
    private void ParseHordeGamesCommand()
    {
        Profile.calculateStats("Horde");
    }
    private void ParseChess360GamesCommand()
    {
        Profile.calculateStats("Chess360");
    }
}