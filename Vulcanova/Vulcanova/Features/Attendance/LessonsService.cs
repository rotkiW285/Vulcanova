using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Attendance.Report;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Features.Shared;
using Vulcanova.Uonet.Api;
using Vulcanova.Uonet.Api.Lessons;
using Vulcanova.Uonet.Api.Presence;
using Xamarin.Essentials;

namespace Vulcanova.Features.Attendance;

public class LessonsService : UonetResourceProvider, ILessonsService
{
    private readonly ILessonsRepository _changesRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IMapper _mapper;
    private readonly IAttendanceReportService _attendanceReportService;

    public LessonsService(ILessonsRepository changesRepository,
        IAccountRepository accountRepository,
        IApiClientFactory apiClientFactory,
        IMapper mapper,
        IAttendanceReportService attendanceReportService)
    {
        _changesRepository = changesRepository;
        _accountRepository = accountRepository;
        _apiClientFactory = apiClientFactory;
        _mapper = mapper;
        _attendanceReportService = attendanceReportService;
    }

    public IObservable<IEnumerable<Lesson>> GetLessonsByMonth(int accountId, DateTime monthAndYear, bool forceSync = false)
    {
        return Observable.Create<IEnumerable<Lesson>>(async observer =>
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            var resourceKey = GetTimetableResourceKey(account, monthAndYear);

            var items = await _changesRepository.GetLessonsForAccountAsync(account.Id, monthAndYear);

            observer.OnNext(items);

            if (ShouldSync(resourceKey) || forceSync)
            {
                DateTime from;
                DateTime to;

                var hasPerformedFullSyncKey = $"Lessons_{account.Id}_HasPerformedFullSync";

                var hasPerformedFullSync = Preferences.Get(hasPerformedFullSyncKey, false);

                if (!hasPerformedFullSync)
                {
                    (from, to) = account.GetSchoolYearDuration();
                }
                else
                {
                    from = new DateTime(monthAndYear.Year, monthAndYear.Month, 1);
                    to = new DateTime(monthAndYear.Year, monthAndYear.Month,
                        DateTime.DaysInMonth(from.Year, from.Month));
                }

                var onlineEntries = await FetchEntriesForMonthAndYear(account, from, to);

                await _changesRepository.UpsertLessonsForAccountAsync(onlineEntries, account.Id, monthAndYear);

                if (!hasPerformedFullSync)
                {
                    Preferences.Set(hasPerformedFullSyncKey, true);
                }

                SetJustSynced(resourceKey);

                items = await _changesRepository.GetLessonsForAccountAsync(account.Id, monthAndYear);

                observer.OnNext(items);

                await _attendanceReportService.InvalidateReportsAsync(account.Id);
            }

            observer.OnCompleted();
        });
    }

    private async Task<Lesson[]> FetchEntriesForMonthAndYear(Account account, DateTime from, DateTime to)
    {
        var query = new GetLessonsByPupilQuery(account.Pupil.Id, from, to, DateTime.MinValue);

        var client = await _apiClientFactory.GetAuthenticatedAsync(account);

        var response = client.GetAllAsync(GetLessonsByPupilQuery.ApiEndpoint, query);

        var lessons = await response.Select(_mapper.Map<Lesson>).ToArrayAsync();

        foreach (var lesson in lessons)
        {
            lesson.Id.AccountId = account.Id;
        }

        return lessons;
    }

    public async Task SubmitAbsenceJustification(int accountId, int lessonClassId, string message)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        var request = new JustifyLessonRequest(message, lessonClassId, account.Pupil.Id, account.Login.Id);

        var client = await _apiClientFactory.GetAuthenticatedAsync(account);

        await client.PostAsync(JustifyLessonRequest.ApiEndpoint, request);
    }

    private static string GetTimetableResourceKey(Account account, DateTime monthAndYear)
        => $"Lessons_{account.Id}_{monthAndYear.Month}_{monthAndYear.Year}";

    protected override TimeSpan OfflineDataLifespan => TimeSpan.FromHours(1);
}