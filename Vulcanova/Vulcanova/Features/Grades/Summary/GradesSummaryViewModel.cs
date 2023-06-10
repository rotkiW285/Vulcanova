using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Features.Shared;
using Vulcanova.Core.Rx;
using Vulcanova.Features.Grades.SubjectDetails;
using Vulcanova.Features.Settings;
using Vulcanova.Uonet.Api.Auth;
using Xamarin.Forms;
using Unit = System.Reactive.Unit;

namespace Vulcanova.Features.Grades.Summary;

public class GradesSummaryViewModel : ViewModelBase
{
    public ReactiveCommand<bool, GradesResult> GetGrades { get; }

    public ReactiveCommand<int, Unit> ShowSubjectGradesDetails { get; }

    [Reactive] public IEnumerable<SubjectGrades> Grades { get; private set; }

    [ObservableAsProperty] public bool IsSyncing { get; }
        
    [Reactive] public int? PeriodId { get; set; }

    [Reactive] public SubjectGrades CurrentSubject { get; private set; }

    [ObservableAsProperty] private GradesResult RawGrades { get; }
    [Reactive] public decimal? PartialGradesAverage { get; private set; }

    private IObservable<ModifiersSettings> ModifiersSettings { get; }

    public GradesSummaryViewModel(
        INavigationService navigationService,
        AccountContext accountContext,
        IGradesService gradesService,
        IAverageGradesService averageGradesService,
        AppSettings settings) : base(navigationService)
    {
        GetGrades = ReactiveCommand.CreateFromObservable((bool forceSync) =>
        {
            var grades = gradesService
                .GetPeriodGrades(accountContext.Account.Id, PeriodId!.Value, forceSync);

            var averageGrades =
                accountContext.Account.Capabilities.Contains(AccountCapabilities.GradesAverageEnabled)
                    ? averageGradesService
                        .GetAverageGrades(accountContext.Account.Id, PeriodId!.Value, forceSync)
                    : Observable.Return(Array.Empty<AverageGrade>());

            return grades.CombineLatest(averageGrades).Select(x => new GradesResult(x.First, x.Second));
        });

        GetGrades.ToPropertyEx(this, vm => vm.RawGrades);

        GetGrades.IsExecuting.ToPropertyEx(this, vm => vm.IsSyncing);

        ShowSubjectGradesDetails = ReactiveCommand.Create((int subjectId) =>
        {
            CurrentSubject = Grades?.First(g => g.SubjectId == subjectId);

            if (Device.RuntimePlatform == Device.iOS)
            {
                navigationService.NavigateAsync(nameof(GradesSubjectDetailsView), 
                    ("Subject", CurrentSubject));
            }

            return Unit.Default;
        });

        this.WhenAnyValue(vm => vm.PeriodId)
            .WhereNotNull()
            .Subscribe(v =>
            {
                GetGrades.Execute(false).SubscribeAndIgnoreErrors();
            });

        var modifiersObservable = settings
            .WhenAnyValue(s => s.Modifiers.PlusSettings.SelectedValue, s => s.Modifiers.MinusSettings.SelectedValue)
            .WhereNotNull();

        this.WhenAnyValue(vm => vm.RawGrades)
            .WhereNotNull()
            .CombineLatest(modifiersObservable)
            .Subscribe(values =>
            {
                var (grades, _) = values;
                Grades = ToSubjectGrades(grades.Grades, grades.AverageGrades, settings.Modifiers);
            });
        
        this.WhenAnyValue(vm => vm.RawGrades)
            .WhereNotNull()
            .CombineLatest(modifiersObservable)
            .Subscribe(values =>
            {
                PartialGradesAverage = values.First.Grades.Average(settings.Modifiers);
            });
    }

    private static IEnumerable<SubjectGrades> ToSubjectGrades(IEnumerable<Grade> grades, IEnumerable<AverageGrade> averageGrades, ModifiersSettings modifiers)
        => grades.GroupBy(g => new
            {
                g.Column.Subject.Id,
                g.Column.Subject.Name
            })
            .Select(g => new SubjectGrades
            {
                SubjectId = g.Key.Id,
                SubjectName = g.Key.Name,
                Average = averageGrades
                    .SingleOrDefault(x => x.SubjectId == g.Key.Id)?.Average ?? g.Average(modifiers),
                Grades = new ObservableCollection<Grade>(g.ToArray())
            });

    public sealed record GradesResult(IEnumerable<Grade> Grades, IEnumerable<AverageGrade> AverageGrades);
}
