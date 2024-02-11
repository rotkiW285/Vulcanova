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

    // https://github.com/xamarin/Xamarin.Forms/issues/13040
    private double _scrollY;
    private bool _scrollEnabled;

    private void EditorFocused(object sender, FocusEventArgs e)
    {
        _scrollY = EditorWrapperScrollView.ScrollY;
    }

    private async void EditorWrapperScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        if (!_scrollEnabled)
        {
            await EditorWrapperScrollView.ScrollToAsync(0, _scrollY, false);
        }
    }

    private void EditorKeyboardWillShow(object sender, EventArgs e)
    {
        _scrollEnabled = false;
    }

    private void EditorKeyboardDidShow(object sender, EventArgs e)
    {
        _scrollEnabled = true;
    }
}