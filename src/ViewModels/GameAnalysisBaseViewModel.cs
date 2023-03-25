using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class GameAnalysisBaseViewModel : BoardViewModel
{
    public string Player1 { get; set; }
    public string Player2 { get; set; }
    public List<string> Moves { get; set; }
    public List<string> Pieces { get; set; }
    public List<string> LostPieces { get; set; }
    public List<string> CapturedPieces { get; set; }

    public GameAnalysisBaseViewModel(IScreen hostScreen, Variant variant = Variant.Standard) : base(hostScreen)
    {
        Title = variant == Variant.Standard ? "Board" : $"Board: {variant}";
        GameHandler = new GameHandler(variant);
    }
}