using System;
using System.Globalization;
using Xamarin.Forms;

namespace Vulcanova.Core.Layout.Controls;

public class TextToHtmlConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value as string)?.Replace("\n", "<br />");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
