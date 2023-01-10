using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Notes.NoteDetails;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class NoteDetailsView : INavigationAware
{
    public static readonly BindableProperty NoteProperty =
        BindableProperty.Create(nameof(Note), typeof(Note), typeof(NotesView));

    public NoteDetailsView()
    {
        InitializeComponent();
    }

    public Note Note
    {
        get => (Note)GetValue(NoteProperty);
        set => SetValue(NoteProperty, value);
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        Note = (Note)parameters["Note"];
    }
}