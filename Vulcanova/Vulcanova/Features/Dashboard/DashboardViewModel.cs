using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Prism.Navigation;
using Prism.Navigation.TabbedPages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Extensions;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Auth.Accounts;
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
        public ReactiveCommand<bool, DashboardModel> RefreshData { get; }
        public ReactiveCommand<string, INavigationResult> OpenTab { get; }

        [ObservableAsProperty] public DashboardModel DashboardModel { get; private set; }

        [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; set; }
        [Reactive] public DateTime SelectedDay { get; private set; }

        private readonly ILuckyNumberService _luckyNumberService;
        private readonly ITimetableService _timetableService;
        private readonly ITimetableChangesService _timetableChangesService;
        private readonly IExamsService _examsService;
        private readonly IGradesService _gradesService;
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
                var selectedPeriodIdNullable = accountContext.Account.Periods
                    .SingleOrDefault(p => p.Start <= SelectedDay && p.End >= SelectedDay)?.Id;

                if (selectedPeriodIdNullable is null)
                {
                    return Observable.Return(new DashboardModel());
                }

                var selectedPeriodId = selectedPeriodIdNullable.Value;

                return GetTimetableEntries(accountContext.Account.Id, SelectedDay, forceRefresh)
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .CombineLatest(GetExamEntries(accountContext.Account.Id, SelectedDay, forceRefresh),
                        GetHomeworkEntries(accountContext.Account.Id, selectedPeriodId, SelectedDay, forceRefresh),
                        GetGrades(accountContext.Account, SelectedDay, selectedPeriodId, forceRefresh),
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
                .CombineLatest(
                    this.WhenAnyValue(vm => vm.SelectedDay))
                .Select(_ => false)
                .InvokeCommand(RefreshData);

            Observable.Generate(
                    DateTime.Now,
                    _ => true,
                    date => date.Date.AddDays(1),
                    date => date,
                    dt => dt)
                .BindTo(this, vm => vm.SelectedDay);

            OpenTab = ReactiveCommand.CreateFromTask((string name) =>
                NavigationService.SelectTabAsync(name));
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
                    .Where(e => IsInTheNext7DaysFrom(date.Date, e.Deadline.Date))
                    .OrderBy(e => e.Deadline.Date)
                    .ToArray()));
        }

        private IObservable<IReadOnlyCollection<Exam>> GetExamEntries(int accountId, DateTime date,
            bool forceSync = false)
        {
            // load more entries, but try to hit cache
            var (firstDay, lastDay) = date.GetMondayOfFirstWeekAndSundayOfLastWeekOfMonth();

            return _examsService.GetExamsByDateRange(accountId, firstDay, lastDay, forceSync)
                .Select(list => list
                    .Where(e => IsInTheNext7DaysFrom(date.Date, e.Deadline.Date))
                    .OrderBy(e => e.Deadline.Date)
                    .ToList()
                    .AsReadOnly());
        }

        private IObservable<IReadOnlyCollection<Grade>> GetGrades(Account account, DateTime date, int periodId,
            bool forceSync)
        {
            return _gradesService.GetPeriodGrades(account, periodId, forceSync)
                .Select(list => list
                    .Where(e => (date.Date - e.DateModify.Date).TotalDays <= 7)
                    .OrderByDescending(e => e.DateModify.Date)
                    .ToList()
                    .AsReadOnly());
        }

        private static bool IsInTheNext7DaysFrom(DateTime from, DateTime? d2)
        {
            if (d2 == null) return false;

            var start = from.Date;
            var end = from.Date.AddDays(7);

            return d2.Value.Date >= start && d2.Value.Date <= end;
        }
    }
}