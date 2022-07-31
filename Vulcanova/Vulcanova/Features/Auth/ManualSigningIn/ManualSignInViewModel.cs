using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Vulcanova.Core.Mvvm;
using Vulcanova.Uonet.Api.Common;
using System;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Vulcanova.Resources;

namespace Vulcanova.Features.Auth.ManualSigningIn;

public class ManualSignInViewModel : ViewModelBase, IValidatableViewModel
{
    public ReactiveCommand<Unit, Unit> AddDevice { get; }

    [Reactive] public string Token { get; set; }

    [Reactive] public string Symbol { get; set; }

    [Reactive] public string Pin { get; set; }

    private readonly IAuthenticationService _authenticationService;
    private readonly AccountsManager _accountsManager;

    private readonly IInstanceUrlProvider _instanceUrlProvider;

    public ManualSignInViewModel(
        INavigationService navigationService,
        IInstanceUrlProvider instanceUrlProvider,
        IAuthenticationService authenticationService,
        AccountsManager accountsManager) : base(navigationService)
    {
        _authenticationService = authenticationService;
        _accountsManager = accountsManager;
        _instanceUrlProvider = instanceUrlProvider;
        
        this.ValidationRule(
            viewModel => viewModel.Token,
            token => !string.IsNullOrWhiteSpace(token),
            "Token cannot be empty");
        
        this.ValidationRule(
            viewModel => viewModel.Symbol,
            symbol => !string.IsNullOrWhiteSpace(symbol),
            "Symbol cannot be empty");
        
        this.ValidationRule(
            viewModel => viewModel.Pin,
            pin => !string.IsNullOrWhiteSpace(pin),
            "PIN cannot be empty");

        AddDevice = ReactiveCommand.CreateFromTask(_ => AddDeviceAsync(Token, Symbol, Pin),
            ValidationContext.Valid);
    }

    private async Task<Unit> AddDeviceAsync(string token, string symbol, string pin)
    {
        var instanceUrl = await _instanceUrlProvider.GetInstanceUrlAsync(token, symbol);

        if (string.IsNullOrEmpty(instanceUrl))
        {
            throw new ArgumentException(Strings.ErrorInvalidToken, nameof(token));
        }

        var accounts = await _authenticationService.AuthenticateAsync(token, pin, instanceUrl);

        await _accountsManager.AddAccountsAsync(accounts);
        await _accountsManager.OpenAccountAndMarkAsCurrentAsync(accounts.First().Id);

        return Unit.Default;
    }

    public ValidationContext ValidationContext { get; } = new();
}