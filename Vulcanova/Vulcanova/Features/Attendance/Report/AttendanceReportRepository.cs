using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;

namespace Vulcanova.Features.Attendance.Report;

public class AttendanceReportRepository : IAttendanceReportRepository
{
    private readonly LiteDatabaseAsync _liteDatabaseAsync;

    public AttendanceReportRepository(LiteDatabaseAsync liteDatabaseAsync)
    {
        _liteDatabaseAsync = liteDatabaseAsync;
    }

    public async Task<IEnumerable<AttendanceReport>> GetAttendanceReportsAsync(int accountId)
    {
        return await _liteDatabaseAsync.GetCollection<AttendanceReport>()
            .FindAsync(r => r.AccountId == accountId);
    }

    public async Task UpdateAttendanceReportsAsync(int accountId, IEnumerable<AttendanceReport> reports)
    {
        await _liteDatabaseAsync.GetCollection<AttendanceReport>()
            .DeleteManyAsync(a => a.AccountId == accountId);

        await _liteDatabaseAsync.GetCollection<AttendanceReport>()
            .InsertAsync(reports);
    }
}