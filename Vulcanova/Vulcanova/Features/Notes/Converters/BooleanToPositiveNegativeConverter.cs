using System;
using System.Globalization;
using Vulcanova.Resources;
using Xamarin.Forms;

namespace Vulcanova.Features.Notes.Converters;

public class BooleanToPositiveNegativeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Strings.NotePositiveLabel : Strings.NoteNegativeLabel;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}