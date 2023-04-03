using System.Diagnostics.CodeAnalysis;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class VariantMatchHistoryViewModel : MatchHistoryViewModel
{
    public VariantMatchHistoryViewModel(IScreen hostScreen) : base(hostScreen)
    {
        PlayerCollection = GetDummyPlayerData("Resources/GameAnalysis_Variant.json");
    }
}