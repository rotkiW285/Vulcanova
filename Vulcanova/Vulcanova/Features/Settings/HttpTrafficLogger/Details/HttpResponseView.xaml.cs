using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Settings.HttpTrafficLogger.Details;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class HttpResponseView
{
    public static readonly BindableProperty LogEntryProperty = BindableProperty.Create(nameof(LogEntry),
        typeof(HttpTrafficLogEntry), typeof(HttpResponseView), propertyChanged: LogEntryChanged);

    public HttpTrafficLogEntry LogEntry
    {
        get => (HttpTrafficLogEntry)GetValue(LogEntryProperty);
        set => SetValue(LogEntryProperty, value);
    }

    public HttpResponseView()
    {
        InitializeComponent();
    }

    private static void LogEntryChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var logEntry = (HttpTrafficLogEntry) newValue;
        var view = (HttpResponseView) bindable;

        var headerModels = logEntry?.ResponseMessage?.Headers
            .Select(h => new HeaderModel(h.Key, string.Join(", ", h.Value)))
            .ToArray() ?? Array.Empty<HeaderModel>();
        
        BindableLayout.SetItemsSource(view.HeadersView, headerModels);
    }
}