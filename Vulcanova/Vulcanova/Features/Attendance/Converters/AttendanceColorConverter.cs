using System;
using System.Globalization;
using Xamarin.Forms;

namespace Vulcanova.Features.Attendance.Converters
{
    public class AttendanceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PresenceType type)
            {
                if (type.AbsenceJustified)
                {
                    return Color.Default;
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
                    return Color.Default;
                }

                var colorVariant = Application.Current.RequestedTheme == OSAppTheme.Dark
                    ? "Dark"
                    : "Light";

                if (Application.Current.Resources.TryGetValue($"{colorVariant}{baseColor}", out var color))
                {
                    return color;
                }
            }

            return Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}