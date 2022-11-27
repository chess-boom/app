using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ChessBoom.ViewModels;
using ChessBoom.Views;
using System.IO;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;


namespace ChessBoom;

[ExcludeFromCodeCoverage]
public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        SetWorkingDirectory();
    }

    public static void SetWorkingDirectory()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        const string getSrcFromAssembly = "../../../..";
        if (assemblyPath is not null)
        {
            Directory.SetCurrentDirectory(assemblyPath + getSrcFromAssembly);
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}