using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Layout;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Features.Auth.Intro;
using Vulcanova.Features.Shared;
using Unit = System.Reactive.Unit;

namespace Vulcanova.Features.Auth.AccountPicker;

public class AccountAwarePageTitleViewModel : ReactiveObject
{
    [ObservableAsProperty]
    public Account Account { get; private set; }

    public ReactiveCommand<Unit, Unit> ShowAccountsDialog { get; }
    
    public ReactiveCommand<int, Account> LoadAccount { get; }
    
    public ReactiveCommand<Unit, Unit> OpenAddAccountPage { get; }
    
    public ReactiveCommand<int, Unit> OpenAccount { get; }

    public AccountAwarePageTitleViewModel(
        AccountContext accountContext,
        IAccountRepository accountRepository,
        AccountsManager accountsManager,
        INavigationService navigationService)
    {
        LoadAccount = ReactiveCommand.CreateFromTask(async (int accountId) 
            => await accountRepository.GetByIdAsync(accountId));

        LoadAccount.ToPropertyEx(this, vm => vm.Account);

        accountContext.WhenAnyValue(ctx => ctx.Account)
            .WhereNotNull()
            .Select(acc => acc.Id)
            .InvokeCommand(this, vm => vm.LoadAccount);

        ShowAccountsDialog = ReactiveCommand.CreateFromTask<Unit>(async _
            =>
        {
            await navigationService.NavigateAsync(nameof(AccountPickerView));
        });

        OpenAddAccountPage = ReactiveCommand.CreateFromTask<Unit>(
            async _ =>
            {
                await navigationService.NavigateAsync($"{nameof(OnboardingNavigationPage)}/{nameof(IntroView)}", useModalNavigation: true);
            });
        
        OpenAccount = ReactiveCommand.CreateFromTask<int>(
            async accountId =>
            {
                await accountsManager.OpenAccountAndMarkAsCurrentAsync(accountId, false);
            });
    }
}