using Prism.Ioc;
using Vulcanova.Features.Settings.HttpTrafficLogger.Details;

namespace Vulcanova.Features.Settings.HttpTrafficLogger;

public static class Config
{
    public static void RegisterHttpTrafficLogger(this IContainerRegistry container)
    {
        container.RegisterForNavigation<HttpTrafficLoggerView>();
        container.RegisterForNavigation<HttpTrafficLoggerEntryDetailsView>();
    }
}