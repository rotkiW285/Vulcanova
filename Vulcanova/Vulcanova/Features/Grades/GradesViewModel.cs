using System.Collections.Generic;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Grades.Final;
using Vulcanova.Features.Grades.Summary;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Grades;

public class GradesViewModel : ViewModelBase
{
    [Reactive] public GradesSummaryViewModel GradesSummaryViewModel { get; private set; }
    [Reactive] public FinalGradesViewModel FinalGradesViewModel { get; private set; }
    [Reactive] public AccountAwarePageTitleViewModel AccountViewModel { get; private set; }

        
    [Reactive] public int SelectedViewModelIndex { get; set; }

    [Reactive] public IEnumerable<Period> Periods { get; private set; }
    [Reactive] public Period SelectedPeriod { get; private set; }

    public GradesViewModel(
        GradesSummaryViewModel gradesSummaryViewModel,
        FinalGradesViewModel finalGradesViewModel,
        AccountAwarePageTitleViewModel accountViewModel,
        AccountContext accountContext,
        INavigationService navigationService) : base(navigationService)
    {
        GradesSummaryViewModel = gradesSummaryViewModel;
        FinalGradesViewModel = finalGradesViewModel;
        AccountViewModel = accountViewModel;
        
        var accountSetObservable = accountContext.WhenAnyValue(ctx => ctx.Account)
            .WhereNotNull();
        
        accountSetObservable.Select(a => a.Periods)
            .BindTo(this, vm => vm.Periods);
        
        accountSetObservable.Select(a => a.Periods.CurrentOrLast())
            .BindTo(this, vm => vm.SelectedPeriod);
    }
}