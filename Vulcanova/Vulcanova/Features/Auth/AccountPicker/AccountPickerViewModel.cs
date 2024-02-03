using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Features.Auth.Intro;
using Unit = System.Reactive.Unit;

namespace Vulcanova.Features.Auth.AccountPicker;

public class AccountPickerViewModel : ViewModelBase, IInitializeAsync
{
    public ReactiveCommand<int, Unit> OpenAccount { get; }
    public ReactiveCommand<Unit, Unit> OpenAddAccountPage { get; }
    public ReactiveCommand<int, Unit> DeleteAccount { get; }

    [Reactive]
    public IReadOnlyCollection<Account> AvailableAccounts { get; private set; }

    private readonly IAccountRepository _accountRepository;
    private readonly AccountsManager _accountsManager;

    public AccountPickerViewModel(
        INavigationService navigationService,
        IAccountRepository accountRepository,
        AccountsManager accountsManager) : base(navigationService)
    {
        _accountRepository = accountRepository;
        _accountsManager = accountsManager;
        OpenAddAccountPage = ReactiveCommand.CreateFromTask<Unit>(
            async _ =>
            {
                await navigationService.NavigateAsync($"{nameof(OnboardingNavigationPage)}/{nameof(IntroView)}",
                    useModalNavigation: true);
            });
        
        OpenAccount = ReactiveCommand.CreateFromTask<int>(
            async accountId =>
            {
                await accountsManager.OpenAccountAndMarkAsCurrentAsync(accountId, false);

                await navigationService.GoBackAsync();
            });

        DeleteAccount = ReactiveCommand.CreateFromTask<int>(DeleteAccountAsync);
    }

    private async Task DeleteAccountAsync(int accountId)
    {
        await _accountsManager.DeleteAccountAsync(accountId);

        AvailableAccounts = await _accountRepository.GetAccountsAsync();
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async Task InitializeAsync(INavigationParameters parameters)
    {
        AvailableAccounts = await _accountRepository.GetAccountsAsync();
    }
}