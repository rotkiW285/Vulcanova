using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Features.Attendance.Report;
using Vulcanova.Features.Auth.AccountPicker;

namespace Vulcanova.Features.Attendance;

public class AttendanceViewModel : ReactiveObject
{
    public AttendanceDetailedViewModel AttendanceDetailedViewModel { get; }
    public AttendanceReportViewModel AttendanceReportViewModel { get; }
    public AccountAwarePageTitleViewModel AccountViewModel { get; private set; }
    
    [Reactive] public int SelectedViewModelIndex { get; set; }

    public AttendanceViewModel(
        AttendanceDetailedViewModel attendanceDetailedViewModel,
        AttendanceReportViewModel attendanceReportViewModel,
        AccountAwarePageTitleViewModel accountViewModel)
    {
        AttendanceDetailedViewModel = attendanceDetailedViewModel;
        AttendanceReportViewModel = attendanceReportViewModel;
        AccountViewModel = accountViewModel;
    }
}