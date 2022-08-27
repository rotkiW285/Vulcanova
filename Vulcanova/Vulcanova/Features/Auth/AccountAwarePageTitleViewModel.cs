using System.Collections.Generic;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Layout;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Features.Auth.Intro;
using Vulcanova.Features.Shared;
using Xamarin.Forms;
using Unit = System.Reactive.Unit;

namespace Vulcanova.Features.Auth;

public class AccountAwarePageTitleViewModel : ReactiveObject
{
    [ObservableAsProperty]
    public Account Account { get; private set; }

    public ReactiveCommand<Unit, Unit> ShowAccountsDialog { get; }
    
    public ReactiveCommand<int, Account> LoadAccount { get; }
    
    public ReactiveCommand<Unit, Unit> OpenAddAccountPage { get; }
    
    public ReactiveCommand<int, Unit> OpenAccount { get; }

    [Reactive]
    public IReadOnlyCollection<Account> AvailableAccounts { get; private set; }

    // iOS only
    private ContentView _sheet;

    public AccountAwarePageTitleViewModel(
        AccountContext accountContext,
        IAccountRepository accountRepository,
        AccountsManager accountsManager,
        INavigationService navigationService,
        ISheetPopper popper = null)
    {
        LoadAccount = ReactiveCommand.CreateFromTask(async (int accountId) 
            => await accountRepository.GetByIdAsync(accountId));

        LoadAccount.ToPropertyEx(this, vm => vm.Account);

        accountContext.WhenAnyValue(ctx => ctx.AccountId)
            .WhereNotNull()
            .InvokeCommand(this, vm => vm.LoadAccount);

        ShowAccountsDialog = ReactiveCommand.CreateFromTask<Unit>(async _
            =>
        {
            AvailableAccounts = await accountRepository.GetAccountsAsync();

            if (popper != null)
            {
                _sheet = new AccountPickerView
                {
                    AvailableAccounts = AvailableAccounts,
                    AddAccountCommand = OpenAddAccountPage,
                    OpenAccountCommand = OpenAccount
                };

                popper.PushSheet(_sheet, hasCloseButton: false, useSafeArea: true);
            }
        });

        OpenAddAccountPage = ReactiveCommand.CreateFromTask<Unit>(
            async _ =>
            {
                await navigationService.NavigateAsync(nameof(IntroView), useModalNavigation: true);
            });
        
        OpenAccount = ReactiveCommand.CreateFromTask<int>(
            async accountId =>
            {
                popper?.PopSheet(_sheet);
                await accountsManager.OpenAccountAndMarkAsCurrentAsync(accountId, false);
            });
    }
}