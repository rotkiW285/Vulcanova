using System;
using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;
using Vulcanova.Core.Layout;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Auth;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AccountAwarePageTitleView
{
    public AccountAwarePageTitleView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.WhenAnyValue(v => v.ViewModel)
                .WhereNotNull()
                .Subscribe(_ =>
                {
                    var navigation = Application.Current.MainPage?.Navigation;
                    var currentPage = navigation?.NavigationStack.LastOrDefault();

                    TitleLabel.Text = currentPage switch
                    {
                        HomeTabbedPage p => p.CurrentPage?.Title,
                        _ => currentPage?.Title
                    };
                })
                .DisposeWith(disposable);
            
            // ReactiveUI binding API seems not to be working here on iOS in Release mode for some reason:
            // PropertyBinderImplementation: v.AvatarView.Text Binding received an Exception! - System.ArgumentException: Set Method not found for 'Text'
            // Workaround with standard Subscribe(...) applied:
            this.WhenAnyValue(v => v.ViewModel.Account)
                .WhereNotNull()
                .Subscribe(account =>
                {
                    AvatarView.IsVisible = true;
                    AvatarView.Text = $"{account.Pupil.FirstName[0]}{account.Pupil.Surname[0]}";
                })
                .DisposeWith(disposable);

            this.BindCommand(ViewModel, vm => vm.ShowAccountsDialog, v => v.AvatarTapRecognizer)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel, vm => vm.OpenAddAccountPage, v => v.AccountPicker)
                .DisposeWith(disposable);
            
            if (Device.RuntimePlatform != Device.iOS)
            {
                UiExtensions.WireUpNonNativeSheet(ViewModel, AccountPicker, Panel,
                        vm => vm.AvailableAccounts,
                        v => v.AvailableAccounts)
                    .DisposeWith(disposable);
            }
        });
    }
}