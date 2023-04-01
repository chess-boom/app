using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ChessBoom.ViewModels;
using ReactiveUI;
using System.Diagnostics.CodeAnalysis;

namespace ChessBoom.Views;

[ExcludeFromCodeCoverage]
public partial class ProfileView : ReactiveUserControl<ProfileViewModel>
{
    public ProfileView()
    {
        this.WhenActivated(_ =>
        {
        });
        AvaloniaXamlLoader.Load(this);
        ToolTip.SetIsOpen(this, true);
    }
}