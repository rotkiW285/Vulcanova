using System;
using System.Collections.Generic;
using System.Linq;
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
        try
        {
            var maxDate = await _liteDatabaseAsync.GetCollection<AttendanceReport>()
                .MaxAsync(r => r.DateGenerated);

            return await _liteDatabaseAsync.GetCollection<AttendanceReport>()
                .FindAsync(r => r.AccountId == accountId && r.DateGenerated == maxDate);
        }
        catch (LiteAsyncException e) when (e.InnerException is InvalidOperationException { Message: "Sequence contains no elements" })
        {
            return Array.Empty<AttendanceReport>();
        }
    }

    public async Task UpdateAttendanceReportsAsync(int accountId, ICollection<AttendanceReport> reports)
    {
        await _liteDatabaseAsync.GetCollection<AttendanceReport>()
            .UpsertAsync(reports);

        await _liteDatabaseAsync.GetCollection<AttendanceReport>()
            .DeleteManyAsync(x => x.AccountId == accountId && !reports.Select(r => r.Id).Contains(x.Id));
    }
}