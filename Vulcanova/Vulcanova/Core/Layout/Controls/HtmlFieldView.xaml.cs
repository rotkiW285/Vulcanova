using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class HtmlFieldView
{
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(HtmlFieldView));

    public string Name
    {
        get => (string) GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }
        
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(HtmlFieldView));

    public string Value
    {
        get => (string) GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public HtmlFieldView()
    {
        InitializeComponent();
    }
}