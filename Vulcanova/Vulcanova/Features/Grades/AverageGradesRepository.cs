using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Grades;

public class AverageGradesRepository : IAverageGradesRepository, IHasAccountRemovalCleanup
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

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<AverageGrade>().DeleteManyAsync(g => g.AccountId == accountId);
    }
}