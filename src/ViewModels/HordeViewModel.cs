using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class HordeViewModel : BaseViewModel
{
    public GameHandler GameHandler { get; }

    public HordeViewModel(IScreen hostScreen) : base(hostScreen)
    {
        GameHandler = new GameHandler(Variant.Horde);
    }
}
