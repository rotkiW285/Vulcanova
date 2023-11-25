using System;
using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms.Xaml;
using Vulcanova.Core.Rx;

namespace Vulcanova.Features.Attendance.Report;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AttendanceReportView
{
    public AttendanceReportView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, vm => vm.ReportItems, v => v.ReportsList.ItemsSource, TimeSpan.FromMilliseconds(50))
                .DisposeWith(disposable);
        });
    }
}