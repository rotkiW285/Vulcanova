using System;
using System.Globalization;
using Vulcanova.Core.Layout;
using Xamarin.Forms;

namespace Vulcanova.Features.Timetable;

public class TimetableChangeColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values[0];

        var color = (value as TimetableListEntry.ChangeDetails).GetDisplayColor();

        var baseColor = color switch
        {
            ChangeDisplayColor.Red => "ErrorColor",
            ChangeDisplayColor.Yellow => "WarningColor",
            ChangeDisplayColor.Normal => "PrimaryTextColor",
            _ => "PrimaryTextColor"
        };

        return ThemeUtility.GetThemedColorByResourceKey(baseColor);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}