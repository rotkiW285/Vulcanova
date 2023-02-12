using System.Collections.Generic;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Layout;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Features.Auth.Intro;
using Unit = System.Reactive.Unit;

namespace Vulcanova.Features.Auth.AccountPicker;

public class AccountPickerViewModel : ViewModelBase, INavigationAware
{
    public ReactiveCommand<int, Unit> OpenAccount { get; }

    public ReactiveCommand<Unit, Unit> OpenAddAccountPage { get; }
    
    [Reactive]
    public IReadOnlyCollection<Account> AvailableAccounts { get; private set; }

    public AccountPickerViewModel(INavigationService navigationService,
        AccountsManager accountsManager) : base(navigationService)
    {
        OpenAddAccountPage = ReactiveCommand.CreateFromTask<Unit>(
            async _ =>
            {
                await navigationService.NavigateAsync($"{nameof(MainNavigationPage)}/{nameof(IntroView)}",
                    useModalNavigation: true);
            });
        
        OpenAccount = ReactiveCommand.CreateFromTask<int>(
            async accountId =>
            {
                await accountsManager.OpenAccountAndMarkAsCurrentAsync(accountId, false);

                await navigationService.GoBackAsync();
            });
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        if (parameters.TryGetValue(nameof(AvailableAccounts), out IReadOnlyCollection<Account> availableAccounts))
        {
            AvailableAccounts = availableAccounts;
        }
    }
}