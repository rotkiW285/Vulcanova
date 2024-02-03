using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Auth.Intro;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AddAccountView : ReactiveContentPage<AddAccountViewModel>
{
    public AddAccountView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.BindCommand(ViewModel, x => x.ScanQrCode, x => x.ScanQrButton)
                .DisposeWith(disposable);

            this.BindCommand(ViewModel, x => x.SignInManually, x => x.ManualSignInButton)
                .DisposeWith(disposable);
        });
    }
}