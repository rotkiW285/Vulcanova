using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vulcanova.Core.Layout.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class FieldView
{
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(FieldView));

    public string Name
    {
        get => (string) GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }
        
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(FieldView));

    public string Value
    {
        get => (string) GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public static readonly BindableProperty OverrideValueProperty =
        BindableProperty.Create(nameof(OverrideValue), typeof(string), typeof(FieldView), propertyChanged: OverrideValueChanged);

    public string OverrideValue
    {
        get => (string) GetValue(OverrideValueProperty);
        set => SetValue(OverrideValueProperty, value);
    }

    public static readonly BindableProperty OverrideColorProperty =
        BindableProperty.Create(nameof(OverrideColor), typeof(Color), typeof(FieldView), propertyChanged: OverrideColorChanged);

    public Color OverrideColor
    {
        get => (Color) GetValue(OverrideColorProperty);
        set => SetValue(OverrideColorProperty, value);
    }
    
    public static readonly BindableProperty ShowEmptyOverrideProperty =
        BindableProperty.Create(nameof(ShowEmptyOverride), typeof(bool), typeof(FieldView),
            defaultValue: true, propertyChanged: ShowEmptyOverrideChanged);

    public bool ShowEmptyOverride
    {
        get => (bool) GetValue(ShowEmptyOverrideProperty);
        set => SetValue(ShowEmptyOverrideProperty, value);
    }

    public FieldView()
    {
        InitializeComponent();

        OverrideValueLabel.SetAppThemeColor(Label.TextColorProperty, ThemeUtility.GetColor("LightWarningColor"),
            ThemeUtility.GetColor("DarkWarningColor"));
    }

    private static void OverrideValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((FieldView) bindable).UpdateOverrideView();
    }

    private static void OverrideColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((FieldView) bindable).OverrideValueLabel.TextColor = (Color) newValue;
    }

    private static void ShowEmptyOverrideChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        ((FieldView) bindable).UpdateOverrideView();
    }

    private void UpdateOverrideView()
    {
        if (OverrideValue == null || (OverrideValue is "" && !ShowEmptyOverride))
        {
            OverrideValueLabel.IsVisible = false;
            OriginalValueLabel.TextDecorations = TextDecorations.None;

            return;
        }

        OverrideValueLabel.IsVisible = true;
        OriginalValueLabel.TextDecorations = TextDecorations.Strikethrough;
    }
}