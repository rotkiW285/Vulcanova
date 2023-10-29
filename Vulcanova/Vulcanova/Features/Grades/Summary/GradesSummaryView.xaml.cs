using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using System;
using Vulcanova.Core.Rx;

namespace Vulcanova.Features.Grades.Summary;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GradesSummaryView
{
    public static readonly BindableProperty PeriodIdProperty = BindableProperty.Create(
        nameof(PeriodId), typeof(int?), typeof(GradesSummaryView));

    public int? PeriodId
    {
        get => (int?) GetValue(PeriodIdProperty);
        set => SetValue(PeriodIdProperty, value);
    }

    public GradesSummaryView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.BindForceRefresh(RefreshView, v => v.ViewModel.GetGrades, true)
                .DisposeWith(disposable);

            this.WhenAnyValue(v => v.PeriodId)
                .WhereNotNull()
                .Subscribe((val) => ViewModel!.PeriodId = val!.Value)
                .DisposeWith(disposable);
        });
    }
}