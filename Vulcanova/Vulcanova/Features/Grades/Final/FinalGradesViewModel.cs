using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Core.Rx;
using Vulcanova.Features.Settings;
using Vulcanova.Features.Shared;

namespace Vulcanova.Features.Grades.Final;

public class FinalGradesViewModel : ViewModelBase
{
    public ReactiveCommand<bool, IEnumerable<FinalGradesEntry>> GetFinalGrades { get; }

    [ObservableAsProperty] private IEnumerable<FinalGradesEntry> FinalGrades { get; set; }

    [Reactive] public int? PeriodId { get; set; }
    [Reactive] private decimal? FinalAverage { get; set; }
    
    [ObservableAsProperty] public IEnumerable<object> FinalGradeItems { get; }

    public FinalGradesViewModel(
        INavigationService navigationService,
        IFinalGradesService finalGradesService,
        AccountContext accountContext,
        AppSettings settings) : base(navigationService)
    {
        GetFinalGrades = ReactiveCommand.CreateFromObservable((bool forceSync) =>
            finalGradesService
                .GetPeriodGrades(accountContext.Account.Id, PeriodId!.Value, forceSync)
                .SubscribeOn(RxApp.TaskpoolScheduler));

        GetFinalGrades.ToPropertyEx(this, vm => vm.FinalGrades);

        this.WhenAnyValue(vm => vm.PeriodId)
            .WhereNotNull()
            .Subscribe(v => GetFinalGrades.Execute().SubscribeAndIgnoreErrors());

        var modifiersObservable = settings
            .WhenAnyValue(s => s.Modifiers.PlusSettings.SelectedValue, s => s.Modifiers.MinusSettings.SelectedValue)
            .WhereNotNull();

        this.WhenAnyValue(vm => vm.FinalGrades)
            .WhereNotNull()
            .CombineLatest(modifiersObservable)
            .Subscribe(values =>
            {
                FinalAverage = values.First.Average(settings.Modifiers);
            });
        
        this.WhenAnyValue(vm => vm.FinalGrades, vm => vm.FinalAverage)
            .Where(x => x is not { Item1: null })
            .Select(items => items.Item2 != null ? items.Item1.Cast<object>().Prepend(items.Item2.Value) : items.Item1)
            .ToPropertyEx(this, vm => vm.FinalGradeItems);
    }
}