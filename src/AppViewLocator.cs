using System;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;
using ChessBoom.ViewModels;
using ChessBoom.Views;

namespace ChessBoom;

[ExcludeFromCodeCoverage]
public class AppViewLocator : IViewLocator
{
    public IViewFor ResolveView<T>(T viewModel, string? contract = null) => viewModel switch
    {
        DashboardViewModel context => new DashboardView { DataContext = context },
        MatchHistoryViewModel context => new PlayerAnalysisView { DataContext = context },
        GameAnalysisViewModel context => new GameAnalysisView { DataContext = context },
        TutorialViewModel context => new TutorialView { DataContext = context },
        BoardViewModel context => new BoardView { DataContext = context },
        ProfileViewModel context => new ProfileView { DataContext = context },
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
    };
}