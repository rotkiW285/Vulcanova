using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Uonet.Api;
using Vulcanova.Uonet.Api.Notes;

namespace Vulcanova.Features.Notes;

public class NotesService : UonetResourceProvider, INotesService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ApiClientFactory _apiClientFactory;
    private readonly IMapper _mapper;
    private readonly INotesRepository _notesRepository;

    public NotesService(
        ApiClientFactory apiClientFactory,
        IMapper mapper,
        IAccountRepository accountRepository,
        INotesRepository notesRepository)
    {
        _apiClientFactory = apiClientFactory;
        _mapper = mapper;
        _accountRepository = accountRepository;
        _notesRepository = notesRepository;
    }

    protected override TimeSpan OfflineDataLifespan => TimeSpan.FromHours(1);

    public IObservable<IEnumerable<Note>> GetNotes(int accountId,
        bool forceSync = false)
    {
        return Observable.Create<IEnumerable<Note>>(async observer =>
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            var resourceKey = GetNotesResourceKey(account);

            var items = await _notesRepository.GetNotesByPupilAsync(account.Id, account.Pupil.Id);

            observer.OnNext(items);

            if (ShouldSync(resourceKey) || forceSync)
            {
                var onlineEntities = await FetchNotes(account, accountId);

                await _notesRepository.UpdateNoteEntriesAsync(onlineEntities, account.Id, account.Pupil.Id);

                SetJustSynced(resourceKey);

                items = await _notesRepository.GetNotesByPupilAsync(account.Id, account.Pupil.Id);

                observer.OnNext(items);
            }

            observer.OnCompleted();
        });
    }

    private async Task<Note[]> FetchNotes(Account account, int accountId)
    {
        var query = new GetNotesByPupilQuery(account.Pupil.Id, DateTime.MinValue);

        var client = await _apiClientFactory.GetAuthenticatedAsync(account);

        var response = client.GetAllAsync(GetNotesByPupilQuery.ApiEndpoint, query);

        var entries = await response.Select(_mapper.Map<Note>).ToArrayAsync();

        foreach (var entry in entries) entry.AccountId = accountId;

        return entries;
    }

    private static string GetNotesResourceKey(Account account)
    {
        return $"Notes_{account.Id}_{account.Pupil.Id}";
    }
}