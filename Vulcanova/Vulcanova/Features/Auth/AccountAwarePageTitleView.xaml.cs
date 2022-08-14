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

            this.OneWayBind(ViewModel, vm => vm.Initials, v => v.AvatarView.Text)
                .DisposeWith(disposable);
        });
    }
}