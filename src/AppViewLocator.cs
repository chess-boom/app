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
        GameAnalysisViewModel context => new GameAnalysisView { DataContext = context },
        VariantAnalysisViewModel context => new VariantAnalysisView { DataContext = context },
        TutorialViewModel context => new TutorialView { DataContext = context },
        AtomicViewModel context => new AtomicView { DataContext = context },
        BoardViewModel context => new BoardView { DataContext = context },
        TemplateViewModel context => new TemplateView { DataContext = context },
        _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
    };
}
