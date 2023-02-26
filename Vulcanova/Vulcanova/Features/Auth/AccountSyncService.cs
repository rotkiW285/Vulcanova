using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Shared;
using Vulcanova.Uonet.Api.Auth;
using Period = Vulcanova.Features.Shared.Period;

namespace Vulcanova.Features.Auth;

public class AccountSyncService : UonetResourceProvider, IAccountSyncService
{
    private readonly AccountContext _accountContext;
    private readonly IApiClientFactory _apiClientFactory;
    private readonly AccountRepository _accountRepository;
    private readonly IMapper _mapper;

    private const string ResourceKey = "AccountsSync";

    public AccountSyncService(AccountContext accountContext,
        IApiClientFactory apiClientFactory,
        AccountRepository accountRepository,
        IMapper mapper)
    {
        _accountContext = accountContext;
        _apiClientFactory = apiClientFactory;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task SyncAccountsIfRequiredAsync()
    {
        if (!ShouldSync(ResourceKey)) return;

        var accountsGroupedByLoginId = (await _accountRepository.GetAccountsAsync())
            .GroupBy(x => x.Login.Id);

        foreach (var accountsGroup in accountsGroupedByLoginId)
        {
            var client = await _apiClientFactory.GetAuthenticatedAsync(accountsGroup.First());
            
            var newAccounts = await client.GetAsync(
                // when querying the unit API contrary to the instance API,
                // the /api prefix has to be omitted, thus [4..] to omit the "api/" prefix
                RegisterHebeClientQuery.ApiEndpoint[4..],
                new RegisterHebeClientQuery());
            
            foreach (var acc in accountsGroup)
            {
                var newAccount = newAccounts.Envelope.SingleOrDefault(x => x.Login.Id == acc.Login.Id);

                if (newAccount == null) continue;

                var currentPeriodsIds = acc.Periods.Select(y => y.Id);

                var periodsChanged = newAccount.Periods.Any(x => !currentPeriodsIds.Contains(x.Id));

                if (!periodsChanged)
                {
                    var newCurrentPeriod = newAccount.Periods.Single(x => x.Current);
                    var oldCurrentPeriod = acc.Periods.Single(x => x.Current);

                    periodsChanged = newCurrentPeriod.Id != oldCurrentPeriod.Id;
                }

                if (periodsChanged)
                {
                    acc.Periods = newAccount.Periods.Select(_mapper.Map<Period>).ToList();
                }

                await _accountRepository.UpdateAccountAsync(acc);

                // is it the active account?
                if (_accountContext.Account.Id == acc.Id)
                {
                    _accountContext.Account = acc;
                }
            }
        }

        SetJustSynced(ResourceKey);
    }

    protected override TimeSpan OfflineDataLifespan => TimeSpan.FromDays(1);
}