using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Settings.HttpTrafficLogger.Details;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class HttpRequestView
{
    public static readonly BindableProperty LogEntryProperty = BindableProperty.Create(nameof(LogEntry),
        typeof(HttpTrafficLogEntry), typeof(HttpRequestView), propertyChanged: LogEntryChanged);

    public HttpTrafficLogEntry LogEntry
    {
        get => (HttpTrafficLogEntry)GetValue(LogEntryProperty);
        set => SetValue(LogEntryProperty, value);
    }

    public HttpRequestView()
    {
        InitializeComponent();
    }

    private static void LogEntryChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var logEntry = (HttpTrafficLogEntry) newValue;
        var view = (HttpRequestView) bindable;

        var headerModels = logEntry?.RequestMessage.Headers
            .Select(h => new HeaderModel(h.Key, string.Join(", ", h.Value)))
            .ToArray() ?? Array.Empty<HeaderModel>();
        
        BindableLayout.SetItemsSource(view.HeadersView, headerModels);
    }
}