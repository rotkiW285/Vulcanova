using System;
using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Attendance.Justification;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class JustifyAbsenceView : IInitializeAsync
{
    public static readonly BindableProperty LessonProperty =
        BindableProperty.Create(nameof(Lesson), typeof(Lesson), typeof(JustifyAbsenceView));

    public Lesson Lesson
    {
        get => (Lesson) GetValue(LessonProperty);
        set => SetValue(LessonProperty, value);
    }

    public JustifyAbsenceView()
    {
        InitializeComponent();
    }

    public Task InitializeAsync(INavigationParameters parameters)
    {
        Lesson = (Lesson) parameters["Lesson"];

        return Task.CompletedTask;
    }
}