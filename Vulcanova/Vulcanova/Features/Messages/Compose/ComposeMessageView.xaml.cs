using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
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
                .Select(_ => true)
                .BindTo(this, v => v.AddressBookSuggestions.IsVisible)
                .DisposeWith(disposable);

            ToEntry.Events()
                .Unfocused
                .Select(_ => false)
                .BindTo(this, v => v.AddressBookSuggestions.IsVisible)
                .DisposeWith(disposable);
            
            AddressBookSuggestions.Events()
                .ItemSelected
                .Subscribe(e =>
                {
                    if (e.SelectedItem != null)
                        ViewModel.Recipient = (AddressBookEntry) e.SelectedItem;
            
                    var hide = new Animation(d => AddressBookSuggestions.HeightRequest = d,
                        AddressBookSuggestions.Height, 0);
                    
                    hide.Commit(this, "PickerHide", finished: (_, _) =>
                    {
                        ToEntry.Unfocus();
                        AddressBookSuggestions.IsVisible = false;
                        AddressBookSuggestions.SelectedItem = null;
                        AddressBookSuggestions.HeightRequest = -1;
                    });
                })
                .DisposeWith(disposable);
        });
    }
}