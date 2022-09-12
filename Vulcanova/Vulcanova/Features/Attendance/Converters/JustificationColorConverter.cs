using System;
using System.Globalization;
using Vulcanova.Core.Layout;
using Vulcanova.Uonet.Api.Lessons;
using Xamarin.Forms;

namespace Vulcanova.Features.Attendance.Converters;

public class JustificationColorConverter : IMultiValueConverter
{
    private static readonly AttendanceColorConverter AttendanceColorConverter = new();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values[0];

        if (value is Lesson lesson)
        {
            if (lesson.PresenceType == null)
            {
                return ThemeUtility.GetDefaultTextColor();
            }

            string baseColor;

            if (lesson.PresenceType.AbsenceJustified)
            {
                baseColor = "PrimaryColor";
            }
            else
            {
                baseColor = lesson.JustificationStatus switch
                {
                    JustificationStatus.Accepted => "PrimaryColor",
                    JustificationStatus.Rejected => "ErrorColor",
                    JustificationStatus.Requested => "WarningColor",
                    _ => null
                };
            }

            if (baseColor == null)
            {
                // since the parent converter will be triggered anyway when the theme changes
                // we can call the child converter with an arbitrary theme value of 0
                return AttendanceColorConverter.Convert(new object[] {lesson.PresenceType, 0}, targetType, parameter, culture);
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