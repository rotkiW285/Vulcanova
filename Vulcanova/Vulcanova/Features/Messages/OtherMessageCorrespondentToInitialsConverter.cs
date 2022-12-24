using System;
using System.Globalization;
using Xamarin.Forms;

namespace Vulcanova.Features.Messages;

public class OtherMessageCorrespondentToInitialsConverter : IValueConverter
{
    private static readonly OtherCorrespondentNameConverter NameConverter = new();
    private static readonly CorrespondentToInitialsConverter CorrespondentToInitialsConverter = new();

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

        return CorrespondentToInitialsConverter.Convert(name, typeof(string), null, culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}