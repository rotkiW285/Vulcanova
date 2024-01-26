using Prism.Ioc;
using Vulcanova.Features.Auth;
using Vulcanova.Features.Shared;
using Vulcanova.Uonet.Api.Common;

namespace Vulcanova.Core.Uonet;

public static class Config
{
    public static void RegisterUonet(this IContainerRegistry container)
    {
        container.RegisterSingleton<IApiClientFactory, ApiClientFactory>();
        container.RegisterSingleton<InstanceUrlProvider>();
        container.RegisterSingleton<IInstanceUrlProvider, FebeInstanceUrlProviderDecorator>();
        container.RegisterSingleton<AccountContext>(ctx =>
        {
            var activeAccount = ctx.Resolve<IAccountRepository>().GetActiveAccountAsync().Result;
            return new AccountContext { Account = activeAccount };
        });
    }
}