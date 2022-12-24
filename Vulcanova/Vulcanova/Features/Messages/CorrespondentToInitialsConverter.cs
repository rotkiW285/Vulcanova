using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Vulcanova.Features.Messages;

public class CorrespondentToInitialsConverter : IValueConverter
{
    private static readonly CorrespondentNameConverter NameConverter = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        
        if (value is not Message)
        {
            throw new ArgumentException($"Unsupported argument of type {value.GetType()}",
                nameof(value));
        }

        var name = (string) NameConverter.Convert(value, typeof(string), null, culture);

        return string.Join(string.Empty, name.Split(' ')
            .Take(2)
            .Select(x => x[0]));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}