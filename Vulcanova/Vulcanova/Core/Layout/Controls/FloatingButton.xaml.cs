using System;
using System.Windows.Input;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class FloatingButton
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FloatingButton));

    public ICommand Command
    {
        get => (ICommand) GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    
    public new static readonly BindableProperty IsVisibleProperty =
        BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(FloatingButton));

    public new bool IsVisible
    {
        get => (bool) GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }
    
    public static readonly BindableProperty SourceProperty =
        BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(FloatingButton));

    [TypeConverter(typeof (ImageSourceConverter))]
    public ImageSource Source
    {
        get => (ImageSource) GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public FloatingButton()
    {
        InitializeComponent();

        this.WhenAnyValue(v => v.IsVisible)
            .Subscribe(v =>
            {
                this.ScaleTo(v ? 1 : 0);
            });
    }
}