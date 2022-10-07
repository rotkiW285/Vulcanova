using Xamarin.Forms;

namespace Vulcanova.Core.Layout.PlatformSpecific.iOS;

public static class Sheet
{
    public static readonly BindableProperty MediumDetentProperty =
        BindableProperty.Create("MediumDetent",
            typeof(bool),
            typeof(Sheet),
            true);
    
    public static bool GetMediumDetent(BindableObject element)
    {
        return (bool) element.GetValue(MediumDetentProperty);
    }

    public static void SetMediumDetent(BindableObject element, bool value)
    {
        element.SetValue(MediumDetentProperty, value);
    }
    
    public static readonly BindableProperty LargeDetentProperty =
        BindableProperty.Create("LargeDetent",
            typeof(bool),
            typeof(Sheet),
            true);
    
    public static bool GetLargeDetent(BindableObject element)
    {
        return (bool) element.GetValue(LargeDetentProperty);
    }

    public static void SetLargeDetent(BindableObject element, bool value)
    {
        element.SetValue(LargeDetentProperty, value);
    }
    
    public static readonly BindableProperty PrefersGrabberVisibleProperty =
        BindableProperty.Create("PrefersGrabberVisible",
            typeof(bool),
            typeof(Sheet),
            true);
    
    public static bool GetPrefersGrabberVisible(BindableObject element)
    {
        return (bool) element.GetValue(PrefersGrabberVisibleProperty);
    }

    public static void SetPrefersGrabberVisible(BindableObject element, bool value)
    {
        element.SetValue(PrefersGrabberVisibleProperty, value);
    }
    
    public static readonly BindableProperty PrefersScrollingExpandsWhenScrolledToEdgeProperty =
        BindableProperty.Create("PrefersScrollingExpandsWhenScrolledToEdge",
            typeof(bool),
            typeof(Sheet),
            true);
    
    public static bool GetPrefersScrollingExpandsWhenScrolledToEdge(BindableObject element)
    {
        return (bool) element.GetValue(PrefersScrollingExpandsWhenScrolledToEdgeProperty);
    }

    public static void SetPrefersScrollingExpandsWhenScrolledToEdge(BindableObject element, bool value)
    {
        element.SetValue(PrefersScrollingExpandsWhenScrolledToEdgeProperty, value);
    }
    
    public static readonly BindableProperty HasCloseButtonProperty =
        BindableProperty.Create("HasCloseButton",
            typeof(bool),
            typeof(Sheet),
            true);
    
    public static bool GetHasCloseButton(BindableObject element)
    {
        return (bool) element.GetValue(HasCloseButtonProperty);
    }

    public static void SetHasCloseButton(BindableObject element, bool value)
    {
        element.SetValue(HasCloseButtonProperty, value);
    }
}