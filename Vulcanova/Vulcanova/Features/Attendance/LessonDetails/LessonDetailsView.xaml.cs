using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Attendance.LessonDetails;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LessonDetailsView : INavigationAware
{
    public static readonly BindableProperty LessonProperty =
        BindableProperty.Create(nameof(Lesson), typeof(Lesson), typeof(LessonDetailsView));

    public Lesson Lesson
    {
        get => (Lesson) GetValue(LessonProperty);
        set => SetValue(LessonProperty, value);
    }

    public LessonDetailsView()
    {
        InitializeComponent();
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        Lesson = (Lesson) parameters["Lesson"];
    }
}