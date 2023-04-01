using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Extensions;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Exams;
using Vulcanova.Features.Grades;
using Vulcanova.Features.Homework;
using Vulcanova.Features.LuckyNumber;
using Vulcanova.Features.Shared;
using Vulcanova.Features.Timetable;
using Vulcanova.Features.Timetable.Changes;

namespace Vulcanova.Features.Dashboard
{
    public class DashboardViewModel : ViewModelBase
    {
        public ReactiveCommand<bool, DashboardModel> RefreshData { get; private set; }
        
        private readonly ITimetableService _timetableService;
        private readonly ITimetableChangesService _timetableChangesService;
        private readonly IExamsService _examsService;
        private readonly IGradesService _gradesService;

        [ObservableAsProperty] public DashboardModel DashboardModel { get; private set; }

        [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; set; }
        [Reactive] public DateTime SelectedDay { get; private set; } = DateTime.Now;

        private readonly ILuckyNumberService _luckyNumberService;
        private readonly IHomeworkService _homeworksService;

        public DashboardViewModel(
            INavigationService navigationService,
            AccountContext accountContext,
            AccountAwarePageTitleViewModel accountViewModel,
            ILuckyNumberService luckyNumberService,
            ITimetableService timetableService,
            ITimetableChangesService timetableChangesService,
            IExamsService examsService,
            IGradesService gradesService,
            IHomeworkService homeworksService) : base(navigationService)
        {
            _luckyNumberService = luckyNumberService;
            _timetableService = timetableService;
            _timetableChangesService = timetableChangesService;
            _examsService = examsService;
            _gradesService = gradesService;
            _homeworksService = homeworksService;

            AccountViewModel = accountViewModel;

            RefreshData = ReactiveCommand.CreateFromObservable((bool forceRefresh) =>
            {
                var selectedPeriodId = accountContext.Account.Periods
                    .Single(p => p.Start <= SelectedDay && p.End >= SelectedDay).Id;

                return GetTimetableEntries(accountContext.Account.Id, SelectedDay, forceRefresh)
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .CombineLatest(GetExamEntries(accountContext.Account.Id, SelectedDay, forceRefresh),
                        GetHomeworkEntries(accountContext.Account.Id, selectedPeriodId, SelectedDay, forceRefresh),
                        GetGrades(accountContext.Account.Id, SelectedDay, selectedPeriodId, forceRefresh),
                        GetLuckyNumberAsync(accountContext.Account.Id, SelectedDay))
                    .Select(values => new DashboardModel
                    {
                        Timetable = values.First,
                        Exams = values.Second,
                        Homework = values.Third,
                        Grades = values.Fourth,
                        LuckyNumber = values.Fifth
                    });
            });

            RefreshData.ToPropertyEx(this, vm => vm.DashboardModel);

            accountContext.WhenAnyValue(ctx => ctx.Account)
                .WhereNotNull()
                .Select(_ => false)
                .InvokeCommand(RefreshData);
        }
        
        
        private IObservable<LuckyNumber.LuckyNumber> GetLuckyNumberAsync(int accountId, DateTime date)
        {
            return Observable.FromAsync(() => _luckyNumberService.GetLuckyNumberAsync(accountId, date));
        }
        
        private IObservable<IReadOnlyCollection<TimetableListEntry>> GetTimetableEntries(int accountId,
            DateTime date, bool forceSync = false)
        {
            var changes = _timetableChangesService.GetChangesEntriesByMonth(accountId, date.Date, forceSync);

            return _timetableService.GetPeriodEntriesByMonth(accountId, date.Date, forceSync)
                .CombineLatest(changes)
                .Select(items =>
                {
                    var timetable = TimetableBuilder.BuildTimetable(items.First.ToArray(), items.Second.ToArray());

                    if (timetable.TryGetValue(date.Date, out var entries))
                    {
                        return entries;
                    }

                    return null;
                });
        }

        private IObservable<IReadOnlyCollection<Homework.Homework>> GetHomeworkEntries(int accountId, int periodId,
            DateTime date, bool forceSync = false)
        {
            return _homeworksService.GetHomework(accountId, periodId, forceSync)
                .Select(list => Array.AsReadOnly(list
                    .Where(e => IsInTheSameWeekAs(date.Date, e.DateCreated.Date))
                    .ToArray()));
        }

        private IObservable<IReadOnlyCollection<Exam>> GetExamEntries(int accountId, DateTime date,
            bool forceSync = false)
        {
            // load more entries, but try to hit cache
            var (firstDay, lastDay) = date.GetMondayOfFirstWeekAndSundayOfLastWeekOfMonth();

            return _examsService.GetExamsByDateRange(accountId, firstDay, lastDay, forceSync)
                .Select(list => list.Where(e => IsInTheSameWeekAs(date.Date, e.Deadline.Date)).ToList().AsReadOnly());
        }

        private IObservable<IReadOnlyCollection<Grade>> GetGrades(int accountId, DateTime date, int periodId,
            bool forceSync)
        {
            return _gradesService.GetPeriodGrades(accountId, periodId, forceSync)
                .Select(list => list
                    .Where(e => IsInTheSameWeekAs(date.Date, e.DateCreated?.Date))
                    .ToList()
                    .AsReadOnly());
        }

        private static bool IsInTheSameWeekAs(DateTime d1, DateTime? d2)
        {
            if (d2 == null) return false;

            var monday = d1.Date.LastMonday();
            var sunday = d1.Date.NextSunday();

            return d2.Value.Date >= monday && d2.Value.Date <= sunday;
        }
    }
}