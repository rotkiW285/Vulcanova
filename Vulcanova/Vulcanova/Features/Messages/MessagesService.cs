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

namespace Vulcanova.Features.Messages;

public class MessagesService : UonetResourceProvider, IMessagesService
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IAccountRepository _accountRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IMapper _mapper;

    public MessagesService(
        IApiClientFactory apiClientFactory,
        IAccountRepository accountRepository,
        IMapper mapper,
        IMessagesRepository messagesRepository)
    {
        _apiClientFactory = apiClientFactory;
        _accountRepository = accountRepository;
        _mapper = mapper;
        _messagesRepository = messagesRepository;
    }

    public IObservable<IEnumerable<Message>> GetMessagesByBox(
        int accountId, Guid messageBoxId, MessageBoxFolder folder, bool forceSync = false)
    {
        return Observable.Create<IEnumerable<Message>>(async observer =>
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            var resourceKey = GetResourceKey(messageBoxId, folder);

            var items = await _messagesRepository.GetMessagesByBoxAsync(messageBoxId, folder);

            observer.OnNext(items);

            if (ShouldSync(resourceKey) || forceSync)
            {
                var onlineEntries = await FetchMessagesByBoxAsync(account, messageBoxId, folder);

                await _messagesRepository.UpsertMessagesForBoxAsync(messageBoxId, onlineEntries);

                SetJustSynced(resourceKey);

                items = await _messagesRepository.GetMessagesByBoxAsync(messageBoxId, folder);

                observer.OnNext(items);
            }

            observer.OnCompleted();
        });
    }

    private async Task<Message[]> FetchMessagesByBoxAsync(Account account, Guid messageBoxId, MessageBoxFolder folder)
    {
        var lastSync = GetLastSync(GetResourceKey(messageBoxId, folder));

        var query = new GetMessagesByMessageBoxQuery(messageBoxId, folder, lastSync,
            PageSize: int.MaxValue);

        var apiClient = await _apiClientFactory.GetAuthenticatedAsync(account);

        var response = await apiClient.GetAsync(GetMessagesByMessageBoxQuery.ApiEndpoint, query);

        var messages = response.Envelope
            .Select(_mapper.Map<Message>)
            .ToArray();

        foreach (var message in messages)
        {
            message.MessageBoxId = messageBoxId;
            message.Folder = folder;
        }

        return messages;
    }
    
    private static string GetResourceKey(Guid messageBoxId, MessageBoxFolder folder)
        => $"Messages_{messageBoxId}_{folder}";

    protected override TimeSpan OfflineDataLifespan => TimeSpan.FromHours(1);
}