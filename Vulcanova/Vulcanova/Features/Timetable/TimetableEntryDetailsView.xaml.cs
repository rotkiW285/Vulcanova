using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Timetable;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TimetableEntryDetailsView : INavigationAware
{
    public static readonly BindableProperty TimetableEntryProperty =
        BindableProperty.Create(nameof(TimetableEntry), typeof(TimetableListEntry), typeof(TimetableEntryDetailsView),
            propertyChanged: TimetableEntryChanged);

    private static void TimetableEntryChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is TimetableListEntry e)
        {
            ((TimetableEntryDetailsView) bindable).LessonTimeLabel.IsVisible = e.Start.Override != null
                                                                               || e.End.Override != null
                                                                               || e.No.Override != null;
        }
    }

    public TimetableListEntry TimetableEntry
    {
        get => (TimetableListEntry) GetValue(TimetableEntryProperty);
        set => SetValue(TimetableEntryProperty, value);
    }

    public TimetableEntryDetailsView()
    {
        InitializeComponent();
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        TimetableEntry = (TimetableListEntry) parameters[nameof(TimetableEntry)];
    }
}