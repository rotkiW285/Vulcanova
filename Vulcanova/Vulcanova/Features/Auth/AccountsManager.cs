using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using Vulcanova.Core.Layout;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Features.Auth.Intro;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Auth;

public sealed class AccountsManager
{
    private readonly IAccountRepository _accountRepository;
    private readonly AccountContext _accountContext;
    private readonly INavigationService _navigationService;
    private readonly IEnumerable<IHasAccountRemovalCleanup> _accountRemovalCleanupHooks;

    public AccountsManager(IAccountRepository accountRepository, INavigationService navigationService, AccountContext accountContext, IEnumerable<IHasAccountRemovalCleanup> accountRemovalCleanupHooks)
    {
        _accountRepository = accountRepository;
        _navigationService = navigationService;
        _accountContext = accountContext;
        _accountRemovalCleanupHooks = accountRemovalCleanupHooks;
    }

    public async Task OpenAccountAndMarkAsCurrentAsync(int accountId, bool navigateToHomePage = true)
    {
        var accounts = await _accountRepository.GetAccountsAsync();

        foreach (var account in accounts)
        {
            account.IsActive = account.Id == accountId;
        }

        await _accountRepository.UpdateAccountsAsync(accounts);

        _accountContext.Account = accounts.Single(acc => acc.IsActive);

        if (navigateToHomePage)
        {
            await _navigationService.NavigateAsync("/MainNavigationPage/HomeTabbedPage?selectedTab=GradesSummaryView");
        }
    }

    public async Task AddAccountsAsync(IEnumerable<Account> accounts)
    {
        await _accountRepository.AddAccountsAsync(accounts);
    }

    public async Task DeleteAccountAsync(int accountId)
    {
        var accounts = await _accountRepository.GetAccountsAsync();
        var accountToDelete = accounts.Single(x => x.Id == accountId);

        await _accountRepository.DeleteByIdAsync(accountToDelete.Id);

        try
        {
            var tasks = _accountRemovalCleanupHooks.Select(h => h.DoPostRemovalCleanUpAsync(accountToDelete.Id));
            await Task.WhenAll(tasks);
        }
        catch
        {
            // ignored
        }

        // account list contained only the account we just deleted
        if (accounts.Count == 1)
        {
            await _navigationService.NavigateAsync($"/{nameof(MainNavigationPage)}/{nameof(IntroView)}", useModalNavigation: false);
            _accountContext.Account = null;
            return;
        }

        if (!accountToDelete.IsActive)
        {
            return;
        }

        var newActiveAccount = accounts.First();

        newActiveAccount.IsActive = true;
        _accountContext.Account = newActiveAccount;
        await _accountRepository.UpdateAccountAsync(newActiveAccount);
    }
}