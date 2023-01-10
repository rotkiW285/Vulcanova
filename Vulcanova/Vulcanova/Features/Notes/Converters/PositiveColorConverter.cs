using System;
using System.Globalization;
using Vulcanova.Core.Layout;
using Xamarin.Forms;

namespace Vulcanova.Features.Notes.Converters;

public class PositiveColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values[0];
        
        if (value is bool isPositive)
        {
            var baseColor = isPositive
                ? "PrimaryColor"
                : "ErrorColor";

            return ThemeUtility.GetThemedColorByResourceKey(baseColor);
        }
        
        return ThemeUtility.GetDefaultTextColor();
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}