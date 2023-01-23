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
    public SolidColorBrush HomeBackground  { get; set; } = new SolidColorBrush(Colors.Transparent);
    // public SolidColorBrush ol GameAnalysisBackground { get; set; } = new SolidColorBrush(Colors.Transparent);
    // public SolidColorBrush  VariantBackground { get; set; } = new SolidColorBrush(Colors.Transparent);
    internal ReactiveCommand<Unit, IRoutableViewModel> GoHome { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTutorial { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoTemplate { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoAnalysis { get; }
    internal ReactiveCommand<Unit, IRoutableViewModel> GoVariant { get; }

    // The command that navigates a user back.
    public ReactiveCommand<Unit, Unit> GoBack => Router.NavigateBack;

    // private void MenuItem_Click(object sender, RoutedEventArgs e)
    // {
    //     var menuItem = (MenuItem)sender;
    //     if (menuItem.Header.ToString() == "TUTORIALS")
    //     {
    //         HomeBackground  = new SolidColorBrush(Colors.LightBlue);
    //     }
    //     else if (menuItem.Header.ToString() == "About")
    //     {
    //          TutorialBackground = new SolidColorBrush(Colors.LightGreen);
    //     }
    // }
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
        GoTemplate = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new TemplateViewModel(this))
        );
        GoAnalysis = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new GameAnalysisViewModel(this))
        );
        GoVariant = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new VariantAnalysisViewModel(this))
        );
    }
}