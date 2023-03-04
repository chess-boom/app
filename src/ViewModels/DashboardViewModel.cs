using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class DashboardViewModel : BaseViewModel, IScreen
{
    public RoutingState Router { get; } = new();
    internal ReactiveCommand<Unit, IRoutableViewModel> GoAnalysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoVariant { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTutorial { get; }

    public string m_greeting { get; set; }

    public DashboardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        m_greeting = "Welcome to Chess Boom!";
        GoAnalysis = ReactiveCommand.CreateFromObservable(
            () => hostScreen.Router.Navigate.Execute(new GameAnalysisViewModel(this)));
        GoVariant = ReactiveCommand.CreateFromObservable(
            () => hostScreen.Router.Navigate.Execute(new VariantAnalysisViewModel(this)));
        GoTutorial = ReactiveCommand.CreateFromObservable(
            () => hostScreen.Router.Navigate.Execute(new TutorialViewModel(this)));
    }
    public static void OnClickGameAnalysis()
    {
        System.Console.WriteLine("Game Analysis clicked");
        var game = new Game();
        System.Console.WriteLine("Game Analysis done");
    }

    public static void OnClickVariantGameAnalysis()
    {
        System.Console.WriteLine("Variant Game Analysis clicked");
        System.Console.WriteLine("variant Game Analysis done");
    }

    public static void OnClickTutorial()
    {
        System.Console.WriteLine("Tutorial clicked");
        System.Console.WriteLine("Tutorial done");
    }
}