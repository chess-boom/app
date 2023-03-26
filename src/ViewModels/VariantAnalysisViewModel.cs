using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Analysis;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class VariantAnalysisViewModel : GameAnalysisViewModel
{
    public VariantAnalysisViewModel(IScreen hostScreen) : base(hostScreen)
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
                Variant = "Atomic",
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
                Variant = "Chess960",
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
                Variant = "Atomic",
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
                Variant = "Horde",
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
                Variant = "Horde",
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
                Variant = "Atomic",
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
                Variant = "Chess960",
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
                Variant = "Atomic",
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
                Variant = "Horde",
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
                Variant = "Horde",
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
                Variant = "Horde",
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
                Variant = "Atomic",
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
                Variant = "Atomic",
                MatchStatus = "Win!",
                Moves = 32,
                Blunders = 2,
                MissedWins = 0,
                Improvement = "++"
            }
        };
    }
}