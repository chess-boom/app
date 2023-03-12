using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class AtomicViewModel : BaseViewModel
{
    public GameHandler GameHandler { get; }

    public AtomicViewModel(IScreen hostScreen) : base(hostScreen)
    {
        GameHandler = new GameHandler(Variant.Atomic);
    }
}
