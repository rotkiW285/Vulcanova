using System.Collections.ObjectModel;
using Prism.Navigation;
using ReactiveUI;
using Vulcanova.Features.Settings.HttpTrafficLogger.Details;

namespace Vulcanova.Features.Settings.HttpTrafficLogger;

public sealed class HttpTrafficLoggerViewModel : ReactiveObject
{
    public ReadOnlyObservableCollection<HttpTrafficLogEntry> LogEntries => LoggingHttpMessageHandler.Instance.Requests;
    
    public ReactiveCommand<HttpTrafficLogEntry, INavigationResult> ShowEntryDetails { get; }

    public HttpTrafficLoggerViewModel(INavigationService navigationService)
    {
        ShowEntryDetails = ReactiveCommand.CreateFromTask((HttpTrafficLogEntry entry) =>
            navigationService.NavigateAsync(nameof(HttpTrafficLoggerEntryDetailsView),
                new NavigationParameters
                {
                    { nameof(HttpTrafficLoggerEntryDetailsViewModel.Entry), entry }
                })
            );
    }
}