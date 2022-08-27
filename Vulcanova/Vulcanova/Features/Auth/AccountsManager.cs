using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Navigation;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Auth;

public class AccountsManager
{
    private readonly IAccountRepository _accountRepository;
    private readonly AccountContext _accountContext;
    private readonly INavigationService _navigationService;

    public AccountsManager(IAccountRepository accountRepository, INavigationService navigationService, AccountContext accountContext)
    {
        _accountRepository = accountRepository;
        _navigationService = navigationService;
        _accountContext = accountContext;
    }

    public async Task OpenAccountAndMarkAsCurrentAsync(int accountId, bool navigateToHomePage = true)
    {
        var accounts = await _accountRepository.GetAccountsAsync();

        foreach (var account in accounts)
        {
            account.IsActive = account.Id == accountId;
        }

        await _accountRepository.UpdateAccountsAsync(accounts);
            
        _accountContext.AccountId = accountId;

        if (navigateToHomePage)
        {
            await _navigationService.NavigateAsync("/MainNavigationPage/HomeTabbedPage?selectedTab=GradesSummaryView");
        }
    }

    public async Task AddAccountsAsync(IEnumerable<Account> accounts)
    {
        await _accountRepository.AddAccountsAsync(accounts);
    }
}