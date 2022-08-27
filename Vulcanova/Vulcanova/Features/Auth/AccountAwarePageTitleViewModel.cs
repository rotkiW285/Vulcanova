using System.Collections.Generic;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Layout;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Features.Auth.Intro;
using Vulcanova.Features.Shared;
using Unit = System.Reactive.Unit;

namespace Vulcanova.Features.Auth;

public class AccountAwarePageTitleViewModel : ReactiveObject
{
    [ObservableAsProperty]
    public Account Account { get; private set; }

    public ReactiveCommand<Unit, Unit> ShowAccountsDialog { get; }
    
    public ReactiveCommand<int, Account> LoadAccount { get; }
    
    public ReactiveCommand<Unit, Unit> OpenAddAccountPage { get; }

    [Reactive]
    public IReadOnlyCollection<Account> AvailableAccounts { get; private set; }

    public AccountAwarePageTitleViewModel(
        AccountContext accountContext,
        IAccountRepository accountRepository,
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
                var popup = new AccountPickerView
                {
                    AvailableAccounts = AvailableAccounts,
                    AddAccountCommand = OpenAddAccountPage
                };

                popper.PopSheet(popup, hasCloseButton: false, useSafeArea: true);
            }
        });

        OpenAddAccountPage = ReactiveCommand.Create<Unit>(_ => navigationService.NavigateAsync(nameof(IntroView), useModalNavigation: true));
    }
}