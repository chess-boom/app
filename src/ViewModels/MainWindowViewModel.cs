using System.Reactive;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls;
using ChessBoom.Models.Game;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class MainWindowViewModel : ReactiveObject, IScreen
{
    // The Router associated with this Screen.
    // Required by the IScreen interface.
    public RoutingState Router { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTutorial { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTemplate { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoChess960Board { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoBoard { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoAtomicBoard { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoHordeBoard { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoAnalysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoVariant { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoProfile { get; }

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
        Router = new RoutingState();
        Router.NavigateAndReset.Execute(new DashboardViewModel(this));
        GoHome = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new DashboardViewModel(this))
        );
        GoTutorial = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new TutorialViewModel(this))
        );
        GoChess960Board = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new BoardViewModel(this, Variant.Chess960))
        );
        GoBoard = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new BoardViewModel(this))
        );
        GoAtomicBoard = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new BoardViewModel(this, Variant.Atomic))
        );
        GoHordeBoard = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new BoardViewModel(this, Variant.Horde))
        );
        GoTemplate = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new TemplateViewModel(this))
        );
        GoAnalysis = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new GameAnalysisViewModel(this))
        );
        GoVariant = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new VariantAnalysisViewModel(this))
        );
        GoProfile = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new TemplateViewModel(this))
        );
    }
}
