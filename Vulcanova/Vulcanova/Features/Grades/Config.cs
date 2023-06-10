using Prism.Ioc;
using Vulcanova.Features.Grades.Final;
using Vulcanova.Features.Grades.SubjectDetails;

namespace Vulcanova.Features.Grades;

public static class Config
{
    public static void RegisterGrades(this IContainerRegistry container)
    {
        container.RegisterForNavigation<GradesView>();
        container.RegisterForNavigation<GradesSubjectDetailsView>();

        container.RegisterScoped<IGradesRepository, GradesRepository>();
        container.RegisterScoped<IGradesService, GradesService>();

        container.RegisterScoped<IFinalGradesRepository, FinalGradesRepository>();
        container.RegisterScoped<IFinalGradesService, FinalGradesService>();

        container.RegisterScoped<IAverageGradesRepository, AverageGradesRepository>();
        container.RegisterScoped<IAverageGradesService, AverageGradesService>();
    }
}