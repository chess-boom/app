using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models.Game;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class DashboardViewModel : BaseViewModel
{
    public DashboardViewModel(IScreen hostScreen) : base(hostScreen)
    {
        
    }

    protected static string m_greeting => "Welcome to Chess Boom!";

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