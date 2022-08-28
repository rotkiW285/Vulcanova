using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Features.Auth.Accounts;

namespace Vulcanova.Features.Shared;

public class AccountContext : ReactiveObject
{
    [Reactive] public Account Account { get; set; }
}