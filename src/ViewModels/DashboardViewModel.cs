using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class DashboardViewModel : BaseViewModel, IScreen
{
    public RoutingState Router { get; } = new();
    internal ReactiveCommand<Unit, IRoutableViewModel> GoAnalysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoVariant { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTutorial { get; }

    public string Greeting { get; set; } = "Welcome to Chess Boom!";

    public string GameAnalysisContent { get; set; } =
        "\n\nReview your chess games\nin more detail and learn how to\nimprove your gameplay\n\n\n\n\n";

    public string VariantGameAnalysisContent { get; set; } =
        "\n\nReview your variant games\nof chess using a similar feature\nof Game Analysis, but specialized\nfor each chess variant\n\n\n\n";

    public string TutorialContent { get; set; } =
        "\n\nDive into basic and advanced chess\ntactics and learn skills that will\ngive you the upper hand against \nyour opponents\n\n\n\n";

    public DashboardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        GoAnalysis = ReactiveCommand.CreateFromObservable(
            () => hostScreen.Router.Navigate.Execute(new MatchHistoryViewModel(this)));
        GoVariant = ReactiveCommand.CreateFromObservable(
            () => hostScreen.Router.Navigate.Execute(new VariantMatchHistoryViewModel(this)));
        GoTutorial = ReactiveCommand.CreateFromObservable(
            () => hostScreen.Router.Navigate.Execute(new TutorialViewModel(this)));
    }
}