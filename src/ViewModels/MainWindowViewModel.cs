using System.Diagnostics.CodeAnalysis;

namespace ChessBoom.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to ChessBoom!";
    }
}
