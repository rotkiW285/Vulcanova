using Xamarin.Forms;

namespace Vulcanova.Features.Attendance.Report;

public sealed class AttendanceReportTemplateSelector : DataTemplateSelector
{
    public DataTemplate TotalAttendance { get; set; }
    public DataTemplate Entry { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item is float ? TotalAttendance : Entry;
    }
}