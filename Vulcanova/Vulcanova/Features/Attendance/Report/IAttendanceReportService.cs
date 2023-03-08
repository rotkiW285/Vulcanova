using System.Threading.Tasks;

namespace Vulcanova.Features.Attendance.Report;

public interface IAttendanceReportService
{
    Task InvalidateReportsAsync(int accountId);
}