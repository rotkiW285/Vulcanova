using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Attendance.Report;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AttendanceReportView
{
    public AttendanceReportView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.Reports, v => v.ReportsList.ItemsSource)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.Reports, v => v.SummaryPercentageLabel.Text,
                    selector: v => $"{v.CalculateOverallAttendance():F2}%")
                .DisposeWith(disposable);
        });
    }
}