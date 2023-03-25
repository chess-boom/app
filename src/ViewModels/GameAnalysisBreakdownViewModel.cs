using System.Diagnostics.CodeAnalysis;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class GameAnalysisBreakdownViewModel : GameAnalysisBaseViewModel
{
    public GameAnalysisBreakdownViewModel(IScreen hostScreen) : base(hostScreen)
    {
        Title = "";
        Player1 = "Daniel";
    }
}