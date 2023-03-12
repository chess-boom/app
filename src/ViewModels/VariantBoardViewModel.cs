using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class VariantBoardViewModel : BaseViewModel
{
    private GameHandler GameHandler { get; }
    private static Variant Variant => Variant.Chess960;

    public VariantBoardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        GameHandler = new GameHandler(Variant);
    }
}