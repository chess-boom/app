using System.Diagnostics.CodeAnalysis;
using System.IO;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class VariantMatchHistoryViewModel : MatchHistoryViewModel
{
    public VariantMatchHistoryViewModel(IScreen hostScreen) : base(hostScreen)
    {
        PlayerCollection =
            GetDummyPlayerData(Path.Combine(System.AppContext.BaseDirectory, "Resources/GameAnalysis_Variant.json"));
    }
}