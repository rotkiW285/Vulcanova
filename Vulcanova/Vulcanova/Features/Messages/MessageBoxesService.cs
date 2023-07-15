using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public class MessageBoxesService : UonetResourceProvider, IMessageBoxesService
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IMapper _mapper;
    private readonly IMessageBoxesRepository _messageBoxesRepository;

    public MessageBoxesService(
        IApiClientFactory apiClientFactory,
        IMapper mapper,
        IMessageBoxesRepository messageBoxesRepository)
    {
        _apiClientFactory = apiClientFactory;
        _mapper = mapper;
        _messageBoxesRepository = messageBoxesRepository;
    }

    public IObservable<IEnumerable<MessageBox>> GetMessageBoxesByAccountId(Account account, bool forceSync = false)
    {
        return Observable.Create<IEnumerable<MessageBox>>(async observer =>
        {
            var accountId = account.Id;
            var resourceKey = GetResourceKey(accountId);

            var items = await _messageBoxesRepository.GetMessageBoxesForAccountAsync(accountId);
            
            observer.OnNext(items);

            if (ShouldSync(resourceKey) || forceSync)
            {
                var onlineBoxes = await FetchMessageBoxesAsync(account);

                await _messageBoxesRepository.UpdateMessageBoxesForAccountAsync(accountId, onlineBoxes);

                SetJustSynced(resourceKey);

                items = await _messageBoxesRepository.GetMessageBoxesForAccountAsync(accountId);

                observer.OnNext(items);
            }
            
            observer.OnCompleted();
        });
    }

    public async Task MarkMessageBoxAsSelectedAsync(MessageBox box)
    {
        var boxes = (await _messageBoxesRepository.GetMessageBoxesForAccountAsync(box.AccountId))
            .ToArray();

        foreach (var b in boxes)
        {
            b.IsSelected = b.Id == box.Id;
        }

        await _messageBoxesRepository.UpdateMessageBoxesForAccountAsync(box.AccountId, boxes);
    }

    private async Task<MessageBox[]> FetchMessageBoxesAsync(Account account)
    {
        var query = new GetMessageBoxesQuery();

        var client =  await _apiClientFactory.GetAuthenticatedAsync(account);

        var response = await client.GetAsync(GetMessageBoxesQuery.ApiEndpoint, query);

        var entries = response.Envelope.Select(_mapper.Map<MessageBox>).ToArray();

        foreach (var entry in entries)
        {
            entry.AccountId = account.Id;
        }

        return entries;
    }
    
    private static string GetResourceKey(int accountId)
        => $"MessageBoxes_{accountId}";

    protected override TimeSpan OfflineDataLifespan => TimeSpan.FromDays(7);
}