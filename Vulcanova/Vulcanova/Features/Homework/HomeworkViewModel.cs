using System;
using System.Reactive;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Core.Rx;
using Vulcanova.Extensions;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Homework.HomeworkDetails;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Homework
{
    public class HomeworkViewModel : ViewModelBase
    {
        public ReactiveCommand<bool, ImmutableArray<Homework>> GetHomeworkEntries { get; }
        public ReactiveCommand<int, Unit> ShowHomeworkDetails { get; }
        
        [ObservableAsProperty] public ImmutableArray<Homework> Entries { get; }

        [Reactive] public IReadOnlyCollection<Homework> CurrentWeekEntries { get; private set; }
        [Reactive] public DateTime SelectedDay { get; set; } = DateTime.Today;

        [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; private set; }

        private readonly IHomeworkService _homeworksService;

        public HomeworkViewModel(
            IHomeworkService homeworksService,
            AccountContext accountContext,
            AccountAwarePageTitleViewModel accountViewModel,
            INavigationService navigationService,
            IPeriodService periodService) : base(navigationService)
        {
            _homeworksService = homeworksService;

            AccountViewModel = accountViewModel;

            GetHomeworkEntries = ReactiveCommand.CreateFromTask(async (bool forceSync) =>
            {
                var periodInfo = await periodService.GetCurrentPeriodAsync(accountContext.Account.Id);
                return await GetEntries(accountContext.Account.Id, periodInfo.CurrentPeriod.Id, forceSync);
            });

            GetHomeworkEntries.ToPropertyEx(this, vm => vm.Entries);

            ShowHomeworkDetails = ReactiveCommand.Create((int lessonId) =>
            {
                var homework = CurrentWeekEntries?.First(g => g.Id == lessonId);

                navigationService.NavigateAsync(nameof(HomeworkDetailsView),
                    (nameof(HomeworkDetailsView.Homework), homework));

                return Unit.Default;
            });

            var lastDate = SelectedDay;

            this.WhenAnyValue(vm => vm.SelectedDay)
                .Subscribe((d) =>
                {
                    if (Entries == null || lastDate.Month != d.Month)
                    {
                        GetHomeworkEntries.Execute(false).SubscribeAndIgnoreErrors();
                    }

                    lastDate = d;
                });

            this.WhenAnyValue(vm => vm.Entries)
                .CombineLatest(this.WhenAnyValue(vm => vm.SelectedDay))
                .Subscribe(tuple =>
                {
                    var (entries, selectedDay) = tuple;

                    var monday = selectedDay.LastMonday();
                    var sunday = selectedDay.NextSunday();

                    CurrentWeekEntries = entries.Where(e => e.Deadline >= monday && e.Deadline < sunday)
                        .ToImmutableList();
                });

            accountContext.WhenAnyValue(ctx => ctx.Account)
                .WhereNotNull()
                .Select(_ => false)
                .InvokeCommand(GetHomeworkEntries);
        }

        private IObservable<ImmutableArray<Homework>> GetEntries(int accountId, int periodId,
            bool forceSync = false)
        {
            return _homeworksService.GetHomework(accountId, periodId, forceSync)
                .Select(e => e.ToImmutableArray());
        }
    }
}