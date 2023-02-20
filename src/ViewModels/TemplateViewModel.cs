using System.Diagnostics.CodeAnalysis;
using ChessBoom.Models;
using ReactiveUI;

namespace ChessBoom.ViewModels;

[ExcludeFromCodeCoverage]
public class TemplateViewModel : BaseViewModel
{
    public Profile Profile { get; set; }
    public TemplateViewModel(IScreen hostScreen) : base(hostScreen)
    {
        string username = "MatteoGisondi";
        Profile = new Profile(username);

    }
}