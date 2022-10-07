using Prism.Ioc;
using Vulcanova.Features.Exams.ExamDetails;

namespace Vulcanova.Features.Exams;

public static class Config
{
    public static void RegisterExams(this IContainerRegistry container)
    {
        container.RegisterForNavigation<ExamDetailsView>();

        container.RegisterScoped<IExamsRepository, ExamsRepository>();
        container.RegisterScoped<IExamsService, ExamsService>();
    }
}