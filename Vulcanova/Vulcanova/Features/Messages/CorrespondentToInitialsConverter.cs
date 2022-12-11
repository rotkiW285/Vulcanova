using System;
using System.Globalization;
using System.Linq;
using Vulcanova.Uonet.Api.MessageBox;
using Xamarin.Forms;

namespace Vulcanova.Features.Messages;

public class CorrespondentToInitialsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value switch
        {
            Correspondent c => string.Join(string.Empty, c.Name.Split(' ')
                .Take(2)
                .Select(x => x[0])),
            { } => throw new ArgumentException($"Unsupported argument of type {value.GetType()}",
                nameof(value)),
            null => throw new ArgumentNullException(nameof(value))
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}