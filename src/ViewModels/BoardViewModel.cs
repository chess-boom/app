using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class BoardViewModel : BaseViewModel
{
    public GameHandler GameHandler { get; set; }
    public string Title { get; set; }

    public BoardViewModel(IScreen hostScreen, Variant variant = Variant.Standard) : base(hostScreen)
    {
        Title = variant == Variant.Standard ? "Board" : $"Board: {variant}";
        GameHandler = new GameHandler(variant);
    }
}
