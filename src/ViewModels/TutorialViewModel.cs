using System;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class TutorialViewModel : ReactiveObject, IRoutableViewModel
{
    // Reference to IScreen that owns the routable view model.
    public IScreen HostScreen { get; }

    // Unique identifier for the routable view model.
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];

    public TutorialViewModel(IScreen screen) => HostScreen = screen;
}
