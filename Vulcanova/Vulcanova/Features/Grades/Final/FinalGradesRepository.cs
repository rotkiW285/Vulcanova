using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Grades.Final;

public class FinalGradesRepository : IFinalGradesRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;

    public FinalGradesRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<FinalGradesEntry>> GetFinalGradesForPupilAsync(int accountId, int pupilId, int periodId)
    {
        return (await _db.GetCollection<FinalGradesEntry>()
                .FindAsync(g => g.PupilId == pupilId && g.AccountId == accountId && g.PeriodId == periodId))
            .OrderBy(g => g.Subject.Name);
    }

    public async Task UpdatePupilFinalGradesAsync(IEnumerable<FinalGradesEntry> newGrades)
    {
        await _db.GetCollection<FinalGradesEntry>().UpsertAsync(newGrades);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<FinalGradesEntry>().DeleteManyAsync(g => g.AccountId == accountId);
    }
}