using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class Chess960ViewModel : BaseViewModel
{
    public GameHandler GameHandler { get; }

    public Chess960ViewModel(IScreen hostScreen) : base(hostScreen)
    {
        GameHandler = new GameHandler(Variant.Chess960);
    }
}