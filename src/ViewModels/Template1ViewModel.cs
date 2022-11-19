using System;
using ReactiveUI;

namespace ChessBoom.ViewModels;

public class Template1ViewModel : ReactiveObject, IRoutableViewModel
{
    // Reference to IScreen that owns the routable view model.
    public IScreen HostScreen { get; }

    // Unique identifier for the routable view model.
    public string UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];

    public Template1ViewModel(IScreen screen) => HostScreen = screen;
}