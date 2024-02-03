using System.Reactive;
using Prism.Navigation;
using ReactiveUI;
using Vulcanova.Core.Mvvm;
using Xamarin.Essentials;

namespace Vulcanova.Features.Auth.Intro;

public class IntroViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, INavigationResult> AddAccount { get; }
    public ReactiveCommand<Unit, Unit> OpenGitHubLink { get; }

    public IntroViewModel(INavigationService navigationService) : base(navigationService)
    {
        AddAccount = ReactiveCommand.CreateFromTask(() => 
            NavigationService.NavigateAsync(nameof(AddAccountView)));

        OpenGitHubLink = ReactiveCommand.CreateFromTask(() =>
            Browser.OpenAsync("https://github.com/VulcanovaApp/Vulcanova",
                BrowserLaunchMode.SystemPreferred));
    }
}