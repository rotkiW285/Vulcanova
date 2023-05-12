using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Core.Rx;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Shared;
using Vulcanova.Features.Timetable.Changes;

namespace Vulcanova.Features.Timetable;

public class TimetableViewModel : ViewModelBase
{
    public ReactiveCommand<bool, IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>>> GetTimetableEntries { get; }

    [ObservableAsProperty] public IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>> Entries { get; }

    [Reactive] public IEnumerable<TimetableListEntry> CurrentDayEntries { get; private set; }
    [Reactive] public DateTime SelectedDay { get; set; } = DateTime.Today;
    
    public ReactiveCommand<TimetableListEntry, Unit> ShowEntryDetails { get; }

    [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; private set; }

    private readonly ITimetableService _timetableService;
    private readonly ITimetableChangesService _timetableChangesService;

    public TimetableViewModel(
        INavigationService navigationService,
        ITimetableService timetableService,
        AccountContext accountContext,
        AccountAwarePageTitleViewModel accountViewModel,
        ITimetableChangesService timetableChangesService) : base(navigationService)
    {
        _timetableService = timetableService;
        _timetableChangesService = timetableChangesService;

        AccountViewModel = accountViewModel;

        GetTimetableEntries = ReactiveCommand.CreateFromObservable((bool forceSync) =>
            GetEntries(accountContext.Account.Id, SelectedDay, forceSync)
                .SubscribeOn(RxApp.TaskpoolScheduler));

        GetTimetableEntries.ToPropertyEx(this, vm => vm.Entries);

        this.WhenAnyValue(vm => vm.SelectedDay)
            .Subscribe((d) =>
            {
                if (Entries == null || !Entries.TryGetValue(SelectedDay.Date, out _))
                {
                    GetTimetableEntries.Execute(false).SubscribeAndIgnoreErrors();
                }
            });

        this.WhenAnyValue(vm => vm.Entries)
            .CombineLatest(this.WhenAnyValue(vm => vm.SelectedDay))
            .Subscribe(tuple =>
            {
                var (entries, selectedDay) = tuple;

                if (entries != null && entries.TryGetValue(selectedDay, out var values))
                {
                    CurrentDayEntries = values;
                    return;
                }

                CurrentDayEntries = null;
            });
        
        ShowEntryDetails = ReactiveCommand.CreateFromTask(async (TimetableListEntry entry) =>
        {
            await navigationService.NavigateAsync(nameof(TimetableEntryDetailsView),
                (nameof(TimetableEntryDetailsView.TimetableEntry), entry));

            return Unit.Default;
        });

        accountContext.WhenAnyValue(ctx => ctx.Account)
            .WhereNotNull()
            .Select(_ => false)
            .InvokeCommand(GetTimetableEntries);
    }

    private IObservable<IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>>> GetEntries(int accountId,
        DateTime monthAndYear, bool forceSync = false)
    {
        var changes = _timetableChangesService.GetChangesEntriesByMonth(accountId, monthAndYear, forceSync);

        return _timetableService.GetPeriodEntriesByMonth(accountId, monthAndYear, forceSync)
            .CombineLatest(changes)
            .Select(items => ToDictionary(items.First, items.Second));
    }

    private static IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>> ToDictionary(
        IEnumerable<TimetableEntry> lessons, IEnumerable<TimetableChangeEntry> changes)
    {
        // avoid multiple enumerations
        var timetableChangeEntries = changes as TimetableChangeEntry[] ?? changes.ToArray();

        return TimetableBuilder.BuildTimetable(lessons.ToArray(), timetableChangeEntries);
    }
}