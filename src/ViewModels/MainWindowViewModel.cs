using System.Reactive;
using ReactiveUI;

namespace ChessBoom.ViewModels;

public class MainWindowViewModel : ReactiveObject, IScreen
{
    private readonly ReactiveCommand<Unit, IRoutableViewModel> _goHome;
    private readonly ReactiveCommand<Unit, IRoutableViewModel> _goTutorial;

    // The Router associated with this Screen.
    // Required by the IScreen interface.
    public RoutingState Router { get; } = new();

    // The command that navigates a user to the dashboard view model.
    internal ReactiveCommand<Unit, IRoutableViewModel> GoHome => _goHome;
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTutorial => _goTutorial;

    // The command that navigates a user back.
    public ReactiveCommand<Unit, Unit> GoBack => Router.NavigateBack;

    protected internal MainWindowViewModel()
    {
        // Manage the routing state. Use the Router.Navigate.Execute
        // command to navigate to different view models. 
        //
        // Note, that the Navigate.Execute method accepts an instance 
        // of a view model, this allows you to pass parameters to 
        // your view models, or to reuse existing view models.
        //
        _goHome = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new DashboardViewModel(this))
        );
        _goTutorial = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new TutorialViewModel(this))
        );
    }
}