using System;
using System.Globalization;
using Vulcanova.Uonet.Api.Lessons;
using Xamarin.Forms;

namespace Vulcanova.Features.Attendance.Converters
{
    public class JustificationColorConverter : IValueConverter
    {
        private static readonly AttendanceColorConverter AttendanceColorConverter = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Lesson lesson)
            {
                if (lesson.PresenceType == null)
                {
                    return Color.Default;
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
                    return AttendanceColorConverter.Convert(lesson.PresenceType, targetType, parameter, culture);
                }

                var colorVariant = Application.Current.RequestedTheme == OSAppTheme.Dark
                    ? "Dark"
                    : "Light";

                if (Application.Current.Resources.TryGetValue($"{colorVariant}{baseColor}", out var color))
                {
                    return color;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}