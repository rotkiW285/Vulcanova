using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vulcanova.Features.Grades;

public interface IAverageGradesRepository
{
    Task<IEnumerable<AverageGrade>> GetAverageGradesForPupilAsync(int accountId, int pupilId, int periodId);
    Task UpdatePupilAverageGradesAsync(IEnumerable<AverageGrade> newGrades);
}