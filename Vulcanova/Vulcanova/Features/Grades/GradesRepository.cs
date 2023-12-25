using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB.Async;
using Vulcanova.Features.Auth;

namespace Vulcanova.Features.Grades;

public class GradesRepository : IGradesRepository, IHasAccountRemovalCleanup
{
    private readonly LiteDatabaseAsync _db;

    public GradesRepository(LiteDatabaseAsync db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Grade>> GetGradesForPupilAsync(int accountId, int pupilId, int periodId)
    {
        return (await _db.GetCollection<Grade>()
                .FindAsync(g => g.PupilId == pupilId && g.Id.AccountId == accountId && g.Column.PeriodId == periodId))
            .OrderBy(g => g.Column.Subject.Name)
            .ThenBy(g => g.DateCreated);
    }

    public async Task UpdatePupilGradesAsync(IEnumerable<Grade> newGrades)
    {
        await _db.GetCollection<Grade>().UpsertAsync(newGrades);
    }

    async Task IHasAccountRemovalCleanup.DoPostRemovalCleanUpAsync(int accountId)
    {
        await _db.GetCollection<Grade>().DeleteManyAsync(g => g.Id.AccountId == accountId);
    }
}