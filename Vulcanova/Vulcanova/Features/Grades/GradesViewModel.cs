using System.Reactive;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Grades.Final;
using Vulcanova.Features.Grades.Summary;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Grades;

public class GradesViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> NextSemester { get; }
    public ReactiveCommand<Unit, Unit> PreviousSemester { get; }

    [Reactive] public GradesSummaryViewModel GradesSummaryViewModel { get; private set; }
    [Reactive] public FinalGradesViewModel FinalGradesViewModel { get; private set; }
    [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; private set; }

        
    [Reactive] public int SelectedViewModelIndex { get; set; }
    [Reactive] public PeriodResult PeriodInfo { get; private set; }

    public GradesViewModel(
        GradesSummaryViewModel gradesSummaryViewModel,
        FinalGradesViewModel finalGradesViewModel,
        AccountAwarePageTitleViewModel accountViewModel,
        AccountContext accountContext,
        IPeriodService periodService,
        INavigationService navigationService) : base(navigationService)
    {
        GradesSummaryViewModel = gradesSummaryViewModel;
        FinalGradesViewModel = finalGradesViewModel;
        AccountViewModel = accountViewModel;

        var setCurrentPeriod =
            ReactiveCommand.CreateFromTask(async (int accountId) =>
                PeriodInfo = await periodService.GetCurrentPeriodAsync(accountId));

        accountContext.WhenAnyValue(ctx => ctx.Account)
            .WhereNotNull()
            .Select(acc => acc.Id)
            .InvokeCommand(setCurrentPeriod);
            
        NextSemester = ReactiveCommand.CreateFromTask(async () =>
        {
            PeriodInfo =
                await periodService.ChangePeriodAsync(accountContext.Account.Id, PeriodChangeDirection.Next);
        });

        PreviousSemester = ReactiveCommand.CreateFromTask(async () =>
        {
            PeriodInfo =
                await periodService.ChangePeriodAsync(accountContext.Account.Id, PeriodChangeDirection.Previous);
        });
    }
}