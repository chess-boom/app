using System.Diagnostics.CodeAnalysis;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class VariantAnalysisViewModel : GameAnalysisViewModel
{
    public VariantAnalysisViewModel(IScreen hostScreen) : base(hostScreen)
    {
        PlayerCollection = GetDummyPlayerData("Resources/GameAnalysis_Variant.json");
    }
}