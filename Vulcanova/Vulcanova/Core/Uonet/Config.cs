using Prism.Ioc;
using Vulcanova.Features.Shared;
using Vulcanova.Uonet.Api.Common;

namespace Vulcanova.Core.Uonet;

public static class Config
{
    public static void RegisterUonet(this IContainerRegistry container)
    {
        container.RegisterSingleton<IApiClientFactory, ApiClientFactory>();
        container.RegisterSingleton<IInstanceUrlProvider, InstanceUrlProvider>();
        container.RegisterSingleton<AccountContext>();
    }
}