using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ReactiveUI;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Uonet.Api.Schedule;

namespace Vulcanova.Features.Timetable;

public class TimetableEntryService : UonetResourceProvider, ITimetableEntryService
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepository;
    private readonly ITimetableEntryRepository _timetableEntryRepository;

    public TimetableEntryService(IApiClientFactory apiClientFactory, IMapper mapper,
        IAccountRepository accountRepository, ITimetableEntryRepository timetableEntryRepository)
    {
        _apiClientFactory = apiClientFactory;
        _mapper = mapper;
        _accountRepository = accountRepository;
        _timetableEntryRepository = timetableEntryRepository;
    }

    public IObservable<IEnumerable<TimetableEntry>> GetEntries(int accountId, DateTime from, DateTime to,
        bool forceSync = false)
    {
        return Observable.Create<IEnumerable<TimetableEntry>>(async observer =>
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            var resourceKey = GetTimetableResourceKey(account, from, to);

            var items = await _timetableEntryRepository.GetEntriesForPupilAsync(account.Id, account.Pupil.Id,
                from, to);

            observer.OnNext(items);

            if (ShouldSync(resourceKey) || forceSync)
            {
                var onlineEntries = await FetchEntriesForMonthAndYear(account, from.Date, to.Date);

                await _timetableEntryRepository.UpdatePupilEntriesAsync(account.Id, account.Pupil.Id, onlineEntries,
                    from.Date, to.Date);

                SetJustSynced(resourceKey);

                items = await _timetableEntryRepository.GetEntriesForPupilAsync(account.Id, account.Pupil.Id,
                    from.Date, to.Date);

                observer.OnNext(items);

                MessageBus.Current.SendMessage(new TimetableUpdatedEvent(accountId));
            }

            observer.OnCompleted();
        });
    }

    private async Task<TimetableEntry[]> FetchEntriesForMonthAndYear(Account account, DateTime from, DateTime to)
    {
        var query = new GetScheduleEntriesByPupilQuery(account.Pupil.Id, from, to, DateTime.MinValue);

        var client = await _apiClientFactory.GetAuthenticatedAsync(account);

        var response = await client.GetAsync(GetScheduleEntriesByPupilQuery.ApiEndpoint, query);

        var entries = response.Envelope.Select(_mapper.Map<TimetableEntry>).ToArray();

        foreach (var entry in entries)
        {
            entry.Id.AccountId = account.Id;
            entry.PupilId = account.Pupil.Id;
        }

        return entries;
    }

    private static string GetTimetableResourceKey(Account account, DateTime from, DateTime to)
        =>
            $"Timetable_{account.Id}_{account.Pupil.Id}_{from.Day}_{from.Month}_{from.Year}_{to.Day}_{to.Month}_{to.Year}";

    protected override TimeSpan OfflineDataLifespan => TimeSpan.FromHours(1);
}

public sealed record TimetableUpdatedEvent(int AccountId) : UonetDataUpdatedEvent;