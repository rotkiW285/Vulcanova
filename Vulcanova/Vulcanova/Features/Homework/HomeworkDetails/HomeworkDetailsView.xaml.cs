using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Homework.HomeworkDetails;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class HomeworkDetailsView : INavigationAware
{
    public static readonly BindableProperty HomeworkProperty =
        BindableProperty.Create(nameof(Homework), typeof(Homework), typeof(HomeworkDetailsView));

    public Homework Homework
    {
        get => (Homework) GetValue(HomeworkProperty);
        set => SetValue(HomeworkProperty, value);
    }

    public HomeworkDetailsView()
    {
        InitializeComponent();
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        Homework = (Homework) parameters[nameof(Homework)];
    }
}