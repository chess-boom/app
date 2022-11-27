using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace ChessBoom.ViewModels;

public abstract class BaseViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
{
    public ViewModelActivator Activator { get; }

    public IScreen HostScreen { get; }

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];

    protected BaseViewModel(IScreen hostScreen)
    {
        HostScreen = hostScreen;
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            /* handle activation */
            Disposable
                .Create(() =>
                {
                    /* handle deactivation */
                })
                .DisposeWith(disposables);
        });
    }
}
