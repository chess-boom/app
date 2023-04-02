using System.Reactive;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
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
    internal ReactiveCommand<Unit, IRoutableViewModel> GoStandardBoard { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoChess960Analysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoAtomicAnalysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoHordeAnalysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoMatchHistory { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoGameAnalysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoVariantMatchHistory { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoProfile { get; }

    // The command that navigates a user back.
    public ReactiveCommand<Unit, Unit> GoBack => Router.NavigateBack;

    /// <summary>
    /// Manage the routing state. Use the Router.Navigate.Execute
    /// command to navigate to different view models.
    ///
    /// Note, that the Navigate.Execute method accepts an instance
    /// of a view model, this allows you to pass parameters to
    /// your view models, or to reuse existing view models.
    /// </summary>
    protected internal MainWindowViewModel()
    {
        Router = new RoutingState();
        Router.NavigateAndReset.Execute(new DashboardViewModel(this));
        GoHome = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new DashboardViewModel(this))
        );
        GoTutorial = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new TutorialViewModel(this))
        );
        GoStandardBoard = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new BoardViewModel(this))
        );
        GoGameAnalysis = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new GameAnalysisViewModel(this))
        );
        GoChess960Analysis = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new GameAnalysisViewModel(this, Variant.Chess960))
        );
        GoAtomicAnalysis = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new GameAnalysisViewModel(this, Variant.Atomic))
        );
        GoHordeAnalysis = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new GameAnalysisViewModel(this, Variant.Horde))
        );
        GoMatchHistory = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new MatchHistoryViewModel(this))
        );
        GoVariantMatchHistory = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new VariantMatchHistoryViewModel(this))
        );
        GoProfile = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new ProfileViewModel(this))
        );
    }
}