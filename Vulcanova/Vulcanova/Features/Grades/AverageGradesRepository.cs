using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;

namespace Vulcanova.Features.Grades;

public class AverageGradesRepository : IAverageGradesRepository
{
    private readonly LiteDatabaseAsync _db;

    public AverageGradesRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AverageGrade>> GetAverageGradesForPupilAsync(int accountId, int pupilId, int periodId)
    {
        return await _db.GetCollection<AverageGrade>()
            .FindAsync(g => g.PupilId == pupilId && g.AccountId == accountId && g.PeriodId == periodId);
    }

    public async Task UpdatePupilAverageGradesAsync(IEnumerable<AverageGrade> newGrades)
    {
        await _db.GetCollection<AverageGrade>().UpsertAsync(newGrades);
    }
}