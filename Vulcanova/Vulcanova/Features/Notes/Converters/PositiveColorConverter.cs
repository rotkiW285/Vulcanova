using System;
using System.Globalization;
using Vulcanova.Core.Layout;
using Xamarin.Forms;

namespace Vulcanova.Features.Notes.Converters;

public class PositiveColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values[0] switch
        {
            true => ThemeUtility.GetThemedColorByResourceKey("PrimaryColor"),
            false => ThemeUtility.GetThemedColorByResourceKey("ErrorColor"),
            _ => ThemeUtility.GetDefaultTextColor()
        };

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}