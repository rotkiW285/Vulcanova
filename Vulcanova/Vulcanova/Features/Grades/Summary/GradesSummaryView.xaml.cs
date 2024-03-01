using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms.Xaml;
using Vulcanova.Core.Rx;

namespace Vulcanova.Features.Grades.Summary;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GradesSummaryView
{
    public GradesSummaryView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.BindForceRefresh(RefreshView, v => v.ViewModel.GetGrades, true)
                .DisposeWith(disposable);
        });
    }
}