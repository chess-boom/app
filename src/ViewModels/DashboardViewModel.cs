using System;
using System.Reactive;
using ReactiveUI;

namespace ChessBoom.ViewModels;

public class DashboardViewModel : MainWindowViewModel, IRoutableViewModel
{
    // The Router associated with this Screen.
    // Required by the IScreen interface.

    public IScreen HostScreen { get; }
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];

    protected internal DashboardViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }
}