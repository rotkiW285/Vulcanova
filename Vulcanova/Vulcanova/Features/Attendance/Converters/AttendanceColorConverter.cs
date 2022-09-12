using System;
using System.Globalization;
using Vulcanova.Core.Layout;
using Xamarin.Forms;

namespace Vulcanova.Features.Attendance.Converters;

public class AttendanceColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values[0];
        
        if (value is PresenceType type)
        {
            if (type.AbsenceJustified)
            {
                return ThemeUtility.GetDefaultTextColor();
            }

            var baseColor = type switch
            {
                {AbsenceJustified: true} => "PrimaryColor",
                {Absence: true} => "ErrorColor",
                {Late: true} => "WarningColor",
                _ => null
            };

            if (baseColor == null)
            {
                return ThemeUtility.GetDefaultTextColor();
            }

            return ThemeUtility.GetThemedColorByResourceKey(baseColor);
        }

        return ThemeUtility.GetDefaultTextColor();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}