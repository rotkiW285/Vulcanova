using System.Threading.Tasks;
using Vulcanova.Core.NativeWidgets;
using Vulcanova.Core.Uonet;

namespace Vulcanova.Features.Attendance.Report.NativeWidget;

public sealed class AttendanceReportWidgetUpdater : IWidgetUpdater<AttendanceReportUpdatedEvent>
{
    private readonly INativeWidgetProxy _nativeWidgetProxy;

    public AttendanceReportWidgetUpdater(INativeWidgetProxy nativeWidgetProxy)
    {
        _nativeWidgetProxy = nativeWidgetProxy;
    }

    public Task Handle(AttendanceReportUpdatedEvent message)
    {
        _nativeWidgetProxy.UpdateWidgetState(INativeWidgetProxy.NativeWidget.AttendanceStats,
            new AttendanceReportWidgetData
            {
                TotalPercentage = float.IsNormal(message.OverallAttendancePercentage)
                    ? message.OverallAttendancePercentage
                    : null
            });

        return Task.CompletedTask;
    }
}