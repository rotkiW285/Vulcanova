using System.Collections.Generic;
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

    public AccountPickerView()
    {
        InitializeComponent();
    }
}