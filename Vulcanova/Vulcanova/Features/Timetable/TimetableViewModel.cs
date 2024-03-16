using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Core.Rx;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Timetable;

public class TimetableViewModel : ViewModelBase
{
    public ReactiveCommand<bool, IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>>>
        GetTimetableEntries { get; }

    [ObservableAsProperty]
    public IReadOnlyDictionary<DateTime, IReadOnlyCollection<TimetableListEntry>> Entries { get; }

    [Reactive] public IEnumerable<TimetableListEntry> CurrentDayEntries { get; private set; }
    [Reactive] public DateTime SelectedDay { get; set; } = DateTime.Today;

    public ReactiveCommand<TimetableListEntry, Unit> ShowEntryDetails { get; }

    [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; private set; }

    public TimetableViewModel(
        INavigationService navigationService,
        AccountContext accountContext,
        AccountAwarePageTitleViewModel accountViewModel,
        ITimetableProvider timetableProvider) : base(navigationService)
    {
        AccountViewModel = accountViewModel;

        GetTimetableEntries = ReactiveCommand.CreateFromObservable((bool forceSync) =>
            timetableProvider.GetEntries(accountContext.Account.Id, SelectedDay, forceSync)
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
}