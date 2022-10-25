using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ChessBoom.ViewModels;
using ChessBoom.Views;
using System.IO;
using System.Reflection;

namespace ChessBoom
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            SetWorkingDirectory();
        }

        public void SetWorkingDirectory()
        {
            string? assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string getSRCFromAssembly = "..\\..\\..\\..";
            if (assemblyPath != null) {
                Directory.SetCurrentDirectory(assemblyPath + getSRCFromAssembly);
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}