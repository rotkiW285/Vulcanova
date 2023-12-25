using Vulcanova.Core.Data;

namespace Vulcanova.Features.Grades;

public class AverageGrade
{
    public AccountEntityId Id { get; set; }
    public int PeriodId { get; set; }
    public int PupilId { get; set; }
    public int SubjectId { get; set; }
    public decimal Average { get; set; }
}