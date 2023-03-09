using System.Linq;
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
                    selector: v =>
                    {
                        var allPresences = v.Sum(x => x.Presence + x.Late);
                        var allNonPresence = v.Sum(x => x.Absence);

                        var percentage = (float) allPresences / (allPresences + allNonPresence) * 100;

                        return $"{percentage:F2}%";
                    })
                .DisposeWith(disposable);
        });
    }
}