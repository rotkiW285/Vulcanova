using System.Collections.Generic;
using System.Windows.Input;
using Vulcanova.Features.Auth.Accounts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Auth;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AccountPickerView
{
    public static readonly BindableProperty AvailableAccountsProperty = BindableProperty.Create(
        nameof(AvailableAccounts),
        typeof(IEnumerable<Account>),
        typeof(AccountPickerView));

    public IEnumerable<Account> AvailableAccounts
    {
        get => (IEnumerable<Account>) GetValue(AvailableAccountsProperty);
        set => SetValue(AvailableAccountsProperty, value);
    }
    
    public static readonly BindableProperty AddAccountCommandProperty = BindableProperty.Create(
        nameof(AddAccountCommand),
        typeof(ICommand),
        typeof(AccountPickerView));

    public ICommand AddAccountCommand
    {
        get => (ICommand) GetValue(AddAccountCommandProperty);
        set => SetValue(AddAccountCommandProperty, value);
    }
    
    public static readonly BindableProperty OpenAccountCommandProperty = BindableProperty.Create(
        nameof(OpenAccountCommand),
        typeof(ICommand),
        typeof(AccountPickerView));

    public ICommand OpenAccountCommand
    {
        get => (ICommand) GetValue(OpenAccountCommandProperty);
        set => SetValue(OpenAccountCommandProperty, value);
    }

    public AccountPickerView()
    {
        InitializeComponent();
    }
}