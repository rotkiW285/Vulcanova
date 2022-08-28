using System;
using System.Globalization;
using Vulcanova.Resources;
using Xamarin.Forms;

namespace Vulcanova.Features.Auth;

public class LoginRoleDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value switch
        {
            string p => p == "Opiekun" ? $" – {Strings.ParentAccountLabel}" : string.Empty,
            { } => throw new ArgumentException($"Unsupported argument of type {value.GetType()}",
                nameof(value)),
            null => throw new ArgumentNullException(nameof(value))
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}