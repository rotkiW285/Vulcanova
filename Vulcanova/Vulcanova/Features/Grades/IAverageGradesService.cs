using System;
using System.Collections.Generic;

namespace Vulcanova.Features.Grades;

public interface IAverageGradesService
{
    IObservable<IEnumerable<AverageGrade>> GetAverageGrades(int accountId, int periodId, bool forceSync = false);
}