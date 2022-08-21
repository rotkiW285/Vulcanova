using System;
using System.Globalization;
using Xamarin.Forms;

namespace Vulcanova.Features.Auth.Accounts;

public class PupilToInitialsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value switch
        {
            Pupil p => $"{p.FirstName[0]}{p.Surname[0]}",
            { } => throw new ArgumentException($"Unsupported argument of type {value.GetType()}",
                nameof(value)),
            null => throw new ArgumentNullException(nameof(value))
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}