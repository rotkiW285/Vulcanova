using System;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Data;
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
        public ReactiveCommand<bool, IReadOnlyCollection<Homework>> GetHomeworkEntries { get; }
        public ReactiveCommand<AccountEntityId, Unit> ShowHomeworkDetails { get; }
        
        [ObservableAsProperty] public IReadOnlyCollection<Homework> Entries { get; }

        [Reactive] public IReadOnlyCollection<Homework> CurrentWeekEntries { get; private set; }
        [Reactive] public DateTime SelectedDay { get; set; } = DateTime.Today;

        [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; private set; }

        private readonly IHomeworkService _homeworksService;

        public HomeworkViewModel(
            IHomeworkService homeworksService,
            AccountContext accountContext,
            AccountAwarePageTitleViewModel accountViewModel,
            INavigationService navigationService) : base(navigationService)
        {
            _homeworksService = homeworksService;

            AccountViewModel = accountViewModel;

            GetHomeworkEntries = ReactiveCommand.CreateFromTask(async (bool forceSync) =>
            {
                var currentPeriod = accountContext.Account.Periods.Single(x => x.Current);
                return await GetEntries(accountContext.Account.Id, currentPeriod.Id, forceSync);
            });

            GetHomeworkEntries.ToPropertyEx(this, vm => vm.Entries);

            ShowHomeworkDetails = ReactiveCommand.Create((AccountEntityId lessonId) =>
            {
                var homework = CurrentWeekEntries?.First(g => g.Id == lessonId);

                navigationService.NavigateAsync(nameof(HomeworkDetailsView),
                    (nameof(HomeworkDetailsViewModel.Homework), homework));

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
                .WhereNotNull()
                .CombineLatest(this.WhenAnyValue(vm => vm.SelectedDay))
                .Subscribe(tuple =>
                {
                    var (entries, selectedDay) = tuple;

                    var monday = selectedDay.LastMonday();
                    var sunday = selectedDay.NextSunday();

                    CurrentWeekEntries = Array.AsReadOnly(entries
                        .Where(e => e.Deadline.Date >= monday && e.Deadline.Date <= sunday)
                        .ToArray());
                });

            accountContext.WhenAnyValue(ctx => ctx.Account)
                .WhereNotNull()
                .Select(_ => false)
                .InvokeCommand(GetHomeworkEntries);
        }

        private IObservable<IReadOnlyCollection<Homework>> GetEntries(int accountId, int periodId,
            bool forceSync = false)
        {
            return _homeworksService.GetHomework(accountId, periodId, forceSync)
                .Select(e => Array.AsReadOnly(e.ToArray()));
        }
    }
}