using System;
using System.Reactive;
using Prism.Navigation;
using Prism.Services;
using ReactiveUI;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Shared;
using Vulcanova.Resources;
using Xamarin.Essentials;

namespace Vulcanova.Core.Layout;

public class ErrorHandlingNavigationPageViewModel : ViewModelBase
{
    protected ErrorHandlingNavigationPageViewModel(INavigationService navigationService,
        IPageDialogService dialogService,
        IAccountRepository accountRepository,
        AccountContext accountContext) : base(navigationService)
    {
        ShowErrorDetails = ReactiveCommand.CreateFromTask(async (Exception ex) =>
        {
            var result = await dialogService.DisplayAlertAsync(Strings.ErrorAlertTitle, ex.ToString(),
                Strings.ReportErrorButton, Strings.IgnoreErrorButton);

            if (result)
            {
                await Browser.OpenAsync("https://github.com/VulcanovaApp/Vulcanova/issues",
                    BrowserLaunchMode.SystemPreferred);
            }

            return Unit.Default;
        });

        ShowSignInAlert = ReactiveCommand.CreateFromTask(async () =>
        {
            await accountRepository.DeleteByIdAsync(accountContext.Account.Id);

            await navigationService.NavigateAsync("/OnboardingNavigationPage/IntroView");

            return Unit.Default;
        });
    }
    
    public ReactiveCommand<Exception, Unit> ShowErrorDetails { get; }
    public ReactiveCommand<Unit, Unit> ShowSignInAlert { get; }
}