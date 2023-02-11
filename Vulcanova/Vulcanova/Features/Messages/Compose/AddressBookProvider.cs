using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages.Compose;

public class AddressBookProvider : UonetResourceProvider, IAddressBookProvider
{
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepository;
    private readonly IAddressBookEntryRepository _addressBookEntryRepository;
    private readonly IApiClientFactory _apiClientFactory;

    public AddressBookProvider(
        IMapper mapper,
        IAccountRepository accountRepository,
        IAddressBookEntryRepository addressBookEntryRepository,
        IApiClientFactory apiClientFactory)
    {
        _mapper = mapper;
        _accountRepository = accountRepository;
        _addressBookEntryRepository = addressBookEntryRepository;
        _apiClientFactory = apiClientFactory;
    }

    public IObservable<IEnumerable<AddressBookEntry>> GetAddressBookEntriesForBox(int accountId, Guid messageBoxId,
        bool forceSync = false)
    {
        return Observable.Create<IEnumerable<AddressBookEntry>>(async observer =>
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            var resourceKey = GetResourceKey(messageBoxId);

            var items = await _addressBookEntryRepository.GetBookEntriesForBoxAsync(messageBoxId);

            observer.OnNext(items);

            if (ShouldSync(resourceKey) || forceSync)
            {
                var onlineEntries = await FetchEntriesAsync(account, messageBoxId);

                await _addressBookEntryRepository.UpsertEntriesAsync(onlineEntries);

                SetJustSynced(resourceKey);

                items = await _addressBookEntryRepository.GetBookEntriesForBoxAsync(messageBoxId);

                observer.OnNext(items);
            }

            observer.OnCompleted();
        }); 
    }
    
    
    private async Task<AddressBookEntry[]> FetchEntriesAsync(Account account, Guid messageBoxId)
    {
        var query = new GetMessageBoxAddressBookQuery(messageBoxId);

        var client =  await _apiClientFactory.GetAuthenticatedAsync(account);

        var response = await client.GetAsync(GetMessageBoxAddressBookQuery.ApiEndpoint, query);

        var entries = response.Envelope.Select(_mapper.Map<AddressBookEntry>).ToArray();

        foreach (var entry in entries)
        {
            entry.MessageBoxId = messageBoxId;
        }

        return entries;
    }

    private static string GetResourceKey(Guid messageBoxId)
        => $"AddressBook_{messageBoxId}";

    protected override TimeSpan OfflineDataLifespan => TimeSpan.FromDays(1);
}