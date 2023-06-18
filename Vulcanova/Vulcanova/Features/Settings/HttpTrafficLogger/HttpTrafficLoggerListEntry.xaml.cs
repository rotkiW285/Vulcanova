using Vulcanova.Core.Layout;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Settings.HttpTrafficLogger;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class HttpTrafficLoggerListEntry
{
    public static readonly BindableProperty LogEntryProperty = BindableProperty.Create(nameof(LogEntry),
        typeof(HttpTrafficLogEntry), typeof(HttpTrafficLoggerListEntry), propertyChanged: LogEntryChanged);

    private static void LogEntryChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (HttpTrafficLoggerListEntry)bindable;
        var newEntry = (HttpTrafficLogEntry)newValue;

        var statusCodeColor = (int?)newEntry?.ResponseMessage?.StatusCode switch
        {
            >= 200 and <= 299 => "Primary",
            <= 399 => "Warning",
            <= 599 => "Error",
            null => "SecondaryText",
            _ => "PrimaryText",
        };
        
        view.StatusCodeLabel.SetAppThemeColor(Label.TextColorProperty, ThemeUtility.GetColor($"Light{statusCodeColor}Color"),
            ThemeUtility.GetColor($"Dark{statusCodeColor}Color"));
    }

    public HttpTrafficLogEntry LogEntry
    {
        get => (HttpTrafficLogEntry)GetValue(LogEntryProperty);
        set => SetValue(LogEntryProperty, value);
    }

    public HttpTrafficLoggerListEntry()
    {
        InitializeComponent();
    }
}