using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vulcanova.Features.Attendance;

public interface ILessonsRepository
{
    Task<IEnumerable<Lesson>> GetLessonsForAccountAsync(int accountId, DateTime monthAndYear);

    Task<IEnumerable<Lesson>> GetLessonsBetweenAsync(int accountId, DateTime start, DateTime end);

    Task UpsertLessonsForAccountAsync(IEnumerable<Lesson> entries, int accountId, DateTime monthAndYear);
}