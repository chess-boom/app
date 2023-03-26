using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Analysis;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class GameAnalysisViewModel : BaseViewModel
{
    private ObservableCollection<Player> _playerCollection = new();

    public ObservableCollection<Player> PlayerCollection
    {
        get => _playerCollection;
        set => this.RaiseAndSetIfChanged(ref _playerCollection, value);
    }

    public GameAnalysisViewModel(IScreen hostScreen) : base(hostScreen)
    {
        PlayerCollection = GetDummyPlayerData();
    }

    private static ObservableCollection<Player> GetDummyPlayerData()
    {
        return new ObservableCollection<Player>
        {
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Loss!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Draw!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Loss!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Draw!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Draw!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            },
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Standard",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            }
        };
    }
}