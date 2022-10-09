using Prism.Navigation;
using Vulcanova.Core.Mvvm;

namespace Vulcanova.Features.Attendance.LessonDetails;

public class LessonDetailsViewModel : ViewModelBase
{
    public LessonDetailsViewModel(INavigationService navigationService) : base(navigationService)
    {
    }
}