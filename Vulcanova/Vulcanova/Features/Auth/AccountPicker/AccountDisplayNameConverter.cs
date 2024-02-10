using System;
using System.Globalization;
using Vulcanova.Features.Auth.Accounts;
using Vulcanova.Resources;
using Xamarin.Forms;

namespace Vulcanova.Features.Auth.AccountPicker;

public sealed class AccountDisplayNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Account account)
        {
            var name = $"{account.Pupil.FirstName} {account.Pupil.Surname}";
            
            if (account.PupilNumber != null)
            {
                name += $" ({account.PupilNumber})";
            }

            if (account.Login.LoginRole == "Opiekun")
            {
                name += $" – {Strings.ParentAccountLabel}";
            }

            return name;
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        throw new ArgumentException($"Unsupported argument of type {value.GetType()}",
            nameof(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}