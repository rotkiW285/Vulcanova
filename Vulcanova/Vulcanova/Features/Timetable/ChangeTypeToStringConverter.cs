using System;
using System.Globalization;
using Vulcanova.Resources;
using Vulcanova.Uonet.Api.Schedule;
using Xamarin.Forms;

namespace Vulcanova.Features.Timetable;

public class ChangeTypeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            ChangeType.Exemption => Strings.CommonChangeExemption,
            ChangeType.Substitution => Strings.CommonChangeSubstitution,
            ChangeType.Rescheduled => Strings.CommonChangeRescheduled,
            ChangeType.ClassAbsence => Strings.CommonChangeClassAbsence,
            not null => throw new ArgumentException($"Unsupported argument of type {value.GetType()}",
                nameof(value)),
            null => throw new ArgumentNullException(nameof(value))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}