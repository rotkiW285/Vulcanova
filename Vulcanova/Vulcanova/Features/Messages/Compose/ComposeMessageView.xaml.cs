using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Vulcanova.Core.Layout;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Features.Messages.Compose;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ComposeMessageView
{
    public ComposeMessageView()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.Bind(ViewModel, vm => vm.RecipientFilter, v => v.ToEntry.Text)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.FilteredAddressBookEntries, v => v.AddressBookSuggestions.ItemsSource)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.Subject, v => v.SubjectEntry.Text)
                .DisposeWith(disposable);

            this.Bind(ViewModel, vm => vm.Content, v => v.ContentEdior.Text)
                .DisposeWith(disposable);

            this.OneWayBind(ViewModel, vm => vm.Send, v => v.SendTapGestureRecognizer.Command)
                .DisposeWith(disposable);

            ToEntry.Events()
                .Focused
                .Subscribe(_ => ViewModel.IsPickingRecipient = true)
                .DisposeWith(disposable);

            ToEntry.Events()
                .Unfocused
                .Subscribe(_ => ViewModel.IsPickingRecipient = false)
                .DisposeWith(disposable);
            
            this.WhenAnyValue(v => v.ViewModel.IsPickingRecipient)
                .Subscribe(v =>
                {
                    if (v)
                    {
                        AddressBookSuggestions.IsVisible = true;
                        return;
                    }

                    var hide = new Animation(d => AddressBookSuggestions.HeightRequest = d,
                        AddressBookSuggestions.Height, 0);

                    hide.Commit(this, "PickerHide", finished: (_, _) =>
                    {
                        ToEntry.Unfocus();
                        AddressBookSuggestions.IsVisible = false;
                        AddressBookSuggestions.HeightRequest = -1;
                    });
                })
                .DisposeWith(disposable);

            ViewModel.Send.CanExecute
                .Select(x => ThemeUtility.GetThemedColorByResourceKey(x ? "PrimaryColor" : "BorderColor"))
                .BindTo(this, v => v.SendImage.TintColor)
                .DisposeWith(disposable);
        });
    }
}