using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class GameAnalysisViewModel : BaseViewModel
{
    public class PlayerData
    {
        public string? Name { get; set; }
        public string? Opponent { get; set; }
        public string? Variant { get; set; }
        public string? MatchStatus { get; set; }
        public int? Moves { get; set; }
        public int? Blunders { get; set; }
        public int? MissedWins { get; set; }
        public string? Improvement { get; set; }

    }

    private ObservableCollection<PlayerData> _playerDataList = new();

    public ObservableCollection<PlayerData> PlayerDataList
    {
        get => _playerDataList;
        set => this.RaiseAndSetIfChanged(ref _playerDataList, value);
    }

    public GameAnalysisViewModel(IScreen hostScreen) : base(hostScreen)
    {
        PlayerDataList = new ObservableCollection<PlayerData>
        {
            new()
            {
                Name = "J4N3D03",
                Opponent = "J0ND03",
                Variant = "Atomic Chess",
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
                Variant = "Atomic Chess",
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
                Variant = "Atomic Chess",
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
                Variant = "Atomic Chess",
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
                Variant = "Atomic Chess",
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
                Variant = "Atomic Chess",
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
                Variant = "Atomic Chess",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            }
        };
    }
}