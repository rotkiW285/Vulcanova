using System.Reactive;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Essentials;

namespace Vulcanova.Features.Settings.HttpTrafficLogger.Details;

public sealed class HttpTrafficLoggerEntryDetailsViewModel : ReactiveObject, IInitialize
{
    [Reactive]
    public HttpTrafficLogEntry Entry { get; private set; }
    
    [Reactive] public int SelectedTabIndex { get; set; }
    
    public ReactiveCommand<Unit, Unit> CopyDetailsToClipboard { get; }

    public HttpTrafficLoggerEntryDetailsViewModel()
    {
        CopyDetailsToClipboard = ReactiveCommand.CreateFromTask(async (Unit _) =>
        {
            var content = Entry!.ToString();
            await Clipboard.SetTextAsync(content);

            return Unit.Default;
        });
    }

    public void Initialize(INavigationParameters parameters)
    {
        Entry = parameters.GetValue<HttpTrafficLogEntry>(nameof(Entry));
    }
}