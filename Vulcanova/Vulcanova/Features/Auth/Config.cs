using System.Linq;
using System.Reflection;
using DryIoc;
using Prism.Ioc;
using Vulcanova.Features.Auth.AccountPicker;
using Vulcanova.Features.Auth.Intro;
using Vulcanova.Features.Auth.ManualSigningIn;
using Vulcanova.Features.Auth.ScanningQrCode;

namespace Vulcanova.Features.Auth;

public static class Config
{
    public static void RegisterAuth(this IContainerRegistry container)
    {
        container.RegisterScoped<AccountsManager>();
        container.RegisterScoped<IAccountRepository, AccountRepository>();

        container.RegisterForNavigation<IntroView>();
        container.RegisterForNavigation<ManualSignInView>();
        container.RegisterForNavigation<QrScannerView>();
        container.RegisterForNavigation<EnterPinCodeView>();
        container.RegisterForNavigation<AccountPickerView>();

        container.RegisterSingleton<AccountAwarePageTitleViewModel>();
        container.RegisterScoped<IAccountSyncService, AccountSyncService>();

        container.RegisterScoped<IAuthenticationService, AuthenticationService>();

        foreach (var cleanUpHook in Assembly.GetExecutingAssembly().GetTypes()
                     .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IHasAccountRemovalCleanup))))
        {
            container.RegisterScoped(typeof(IHasAccountRemovalCleanup), cleanUpHook);
        }
    }
}