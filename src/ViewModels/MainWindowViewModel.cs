using System.Reactive;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class MainWindowViewModel : ReactiveObject, IScreen
{
    // The Router associated with this Screen.
    // Required by the IScreen interface.
    public RoutingState Router { get; } = new();
    internal ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTutorial { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTemplate { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoBoard { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoAnalysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoVariant { get; }

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
        GoHome = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new DashboardViewModel(this))
        );
        GoTutorial = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new TutorialViewModel(this))
        );
        GoBoard = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new BoardViewModel(this))
        );
        GoTemplate = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new ProfileViewModel(this))
        );
        GoAnalysis = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new GameAnalysisViewModel(this))
        );
        GoVariant = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new VariantAnalysisViewModel(this))
        );
    }
}