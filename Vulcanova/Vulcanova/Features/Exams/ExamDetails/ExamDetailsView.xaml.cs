using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Exams.ExamDetails;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ExamDetailsView : INavigationAware
{
    public static readonly BindableProperty ExamProperty =
        BindableProperty.Create(nameof(Exam), typeof(Exam), typeof(ExamDetailsView));

    public Exam Exam
    {
        get => (Exam) GetValue(ExamProperty);
        set => SetValue(ExamProperty, value);
    }

    public ExamDetailsView()
    {
        InitializeComponent();
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        Exam = (Exam) parameters["Exam"];
    }
}