using System;
using System.Globalization;
using System.Linq;
using Vulcanova.Uonet.Api.MessageBox;
using Xamarin.Forms;

namespace Vulcanova.Features.Messages;

public class OtherCorrespondentNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value switch
        {
            Message { Folder: MessageBoxFolder.Sent } m => string.Join(", ", m.Receiver.Select(r => r.Name)),
            Message m => m.Sender.Name,
            { } => throw new ArgumentException($"Unsupported argument of type {value.GetType()}",
                nameof(value)),
            null => string.Empty
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}